using EventStaf.Data;
using EventStaf.Repositories;
using EventStaf.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using OpenTelemetry;
using System.Text;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using EventStaf.Infra.Cache;
using Swashbuckle.AspNetCore.Filters;
using EventStaf.Infra.Constants;
using EventStaf.Infra.Swagger;
using MassTransit;
using EventStaf.Entities;
using EventStaf.Infra.MessageQue;
using MassTransit.Logging;
using Newtonsoft.Json;
using MassTransit.RabbitMqTransport;
using static MassTransit.Logging.DiagnosticHeaders.Messaging;
using EventStaf.Infra.Configuration;
using Google.Protobuf.WellKnownTypes;

System.Threading.Thread.Sleep(15000);

var builder = WebApplication.CreateBuilder(args);

var applicationConfiguration = builder.Configuration.Get<ApplicationConfiguration>();

Console.WriteLine("=======================================BEGIN whole config");
Console.WriteLine(JsonConvert.SerializeObject(applicationConfiguration));
Console.WriteLine("=======================================END whole config");

var connectionString = builder.Environment.IsDevelopment()
	? applicationConfiguration?.ConnectionStrings?.DevelopmentConnection ?? string.Empty
	: applicationConfiguration?.ConnectionStrings?.DefaultConnection ?? string.Empty;


var redisConnection = ConnectionMultiplexer.Connect(applicationConfiguration?.ConnectionStrings?.Redis ?? string.Empty);
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService<AppUser>, UserService>();
builder.Services.AddScoped<IEventService, EventService>();

SetSwagger(builder, applicationConfiguration);

// Add services to the container.
builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHealthChecks()
	.AddSqlServer(connectionString)
	//.AddRedis("localhost:6379");
	.AddRedis(applicationConfiguration?.ConnectionStrings?.Redis ?? string.Empty);


builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = applicationConfiguration?.ConnectionStrings?.Redis ?? string.Empty;
	options.InstanceName = ConstantKeys.EventStafCache;
});
builder.Services.AddScoped<ICacheService, CacheService>();


builder.Services.AddScoped<IEventStafPublisher, EventStafPublisher>();


builder.Services.AddMassTransit(x =>
{
	x.UsingRabbitMq((context, cfg) =>
	{
		var rabbitMqHost = applicationConfiguration?.MassTransit?.Host?? string.Empty; 
		var rabbitMqVirtualHost = applicationConfiguration?.MassTransit?.VirtualHost ?? string.Empty;
		var rabbitMqUser = applicationConfiguration?.MassTransit?.Username ?? string.Empty;
        var rabbitMqPassword = applicationConfiguration?.MassTransit?.Username ?? string.Empty;

        cfg.Host(rabbitMqHost, rabbitMqVirtualHost, h =>
		{
			h.Username(rabbitMqUser);
			h.Password(rabbitMqPassword);
		});
	});
});

// Configure OpenTelemetry
SetTracing(builder, redisConnection, applicationConfiguration);

builder.Services.AddCors(options =>
{
	options.AddPolicy(ConstantKeys.AllowAll, builder =>
	{
		builder.AllowAnyOrigin()
			   .AllowAnyMethod()
			   .AllowAnyHeader();
	});
});



var app = builder.Build();

await InitMigrateAndSeed(app);

app.UseCors(ConstantKeys.AllowAll);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
//app.Run("http://0.0.0.0:80");





static void SetSwagger(WebApplicationBuilder builder, ApplicationConfiguration? appConfig)
{
	// Configure JWT authentication
	builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
		.AddJwtBearer(options =>
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = appConfig?.Jwt.Issuer ?? string.Empty,
				ValidAudience = appConfig?.Jwt.Audience ?? string.Empty,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfig?.Jwt.Key ?? string.Empty))
			};
		});

	builder.Services.AddSwaggerGen(c =>
	{
		c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Web API", Version = "v1" });

		c.EnableAnnotations();
		c.UseInlineDefinitionsForEnums();
		c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
		c.OperationFilter<AddResponseHeadersFilter>();
		c.SchemaFilter<DescriptionSchemaFilter>();
		c.ExampleFilters();

		c.AddSecurityDefinition(ConstantKeys.Bearer, new OpenApiSecurityScheme
		{
			Description = "JWT Authorization header using the Bearer scheme.",
			Name = ConstantKeys.Authorization,
			In = ParameterLocation.Header,
			Type = SecuritySchemeType.Http,
			Scheme = ConstantKeys.Bearer
		});
		c.AddSecurityRequirement(new OpenApiSecurityRequirement
		{
			{
				new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference
					{
						Type = ReferenceType.SecurityScheme,
						Id = ConstantKeys.Bearer
					}
				},
				new string[] { }
			}
		});
	});

	builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

}

static void SetTracing(WebApplicationBuilder builder, IConnectionMultiplexer redisCon, ApplicationConfiguration? appConfig)
{
	builder.Services.AddOpenTelemetry()
		.WithTracing(tracerProviderBuilder =>
			tracerProviderBuilder
				.AddSource(ConstantKeys.EventStafApi)
				.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ConstantKeys.EventStafApi))
				.AddAspNetCoreInstrumentation()
				.AddRedisInstrumentation(redisCon)
				//.AddSource(DiagnosticHeaders.DefaultListenerName)
				.AddSqlClientInstrumentation(options =>
				{
					options.SetDbStatementForText = true;
					options.RecordException = true;
				})
				.AddOtlpExporter(options =>
				{
					options.Endpoint = new Uri(appConfig?.ConnectionStrings?.RedisHttp ?? string.Empty); // OTLP gRPC endpoint
				}))
		.WithMetrics(meterProviderBuilder =>
			meterProviderBuilder
				.AddAspNetCoreInstrumentation()
				.AddOtlpExporter(options =>
				{
					options.Endpoint = new Uri(appConfig?.ConnectionStrings?.RedisGrpc ?? string.Empty); // OTLP gRPC endpoint
				}));

}

static Task InitMigrateAndSeed(WebApplication app)
{
	using (var scope = app.Services.CreateScope())
	{
		var services = scope.ServiceProvider;
		var logger = services.GetRequiredService<ILogger<Program>>();
		try
		{
			var context = services.GetRequiredService<ApplicationDbContext>();
			bool dbExists = context.Database.CanConnect();

			if (!dbExists)
			{
				// This will create the database without applying any migrations or creating tables
				//context.Database.EnsureCreated();
				logger.LogInformation("Database created successfully.");
			}
			else
			{
				logger.LogInformation("Database already exists.");

			}

			// Apply any pending migrations
			if (context.Database.GetPendingMigrations().Any())
			{
				context.Database.Migrate();
			}

			// Seed data
			var _userService = services.GetService<IUserService<AppUser>>();
			if (!_userService?.AnyAsync()?.Result?.Value ?? false)
			{
                logger.LogInformation("=====before seeding");
                DataSeeder.SeedData(context);
                logger.LogInformation("=====after seeding");
            }

			logger.LogInformation("Database is ready.");
		}
		catch (Exception ex)
		{
			logger.LogError(ex, $"An error occurred while setting up the database.{ex.Message}");
		}

		return Task.CompletedTask;
	}
}