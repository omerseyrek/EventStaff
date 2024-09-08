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
using EventStaf.Models;
using StackExchange.Redis;
using EventStaf.Infra.Cache;
using Swashbuckle.AspNetCore.Filters;
using EventStaf.Infra.Constants;


var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Environment.IsDevelopment()
	? builder.Configuration.GetConnectionString("DevelopmentConnection")
	: builder.Configuration.GetConnectionString("DefaultConnection");


Console.WriteLine("=========================" + builder.Environment.EnvironmentName+ "==========================");

var redisConnectionString = builder.Configuration.GetConnectionString(Constants.Redis);
var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService<AppUser>, UserService>();


SetSwagger(builder);

// Add services to the container.
builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddHealthChecks()
	.AddSqlServer(connectionString)
    .AddRedis(redisConnectionString);


builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = redisConnectionString;
	options.InstanceName = Constants.EventStafCache;
});
builder.Services.AddScoped<ICacheService, CacheService>();



// Configure OpenTelemetry
SetTracing(builder, redisConnection);

builder.Services.AddCors(options =>
{
	options.AddPolicy(Constants.AllowAll, builder =>
	{
		builder.AllowAnyOrigin()
			   .AllowAnyMethod()
			   .AllowAnyHeader();
	});
});



var app = builder.Build();

await InitMigrateAndSeed(app);

app.UseCors(Constants.AllowAll);
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





static void SetSwagger(WebApplicationBuilder builder)
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
				ValidIssuer = builder.Configuration[Constants.JwtIssuerKey],
				ValidAudience = builder.Configuration[Constants.JwtAudienceKey],
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration[Constants.JwtKeyKey]))
			};
		});

	builder.Services.AddSwaggerGen(c =>
	{
		c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Web API", Version = "v1" });

		c.EnableAnnotations();
		c.UseInlineDefinitionsForEnums();

		// Use reflection to set examples
		//c.SchemaFilter<ExampleSchemaFilter>();
		//c.OperationFilter<AddRequestExamplesFilter>();

		c.ExampleFilters();

		c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
		{
			Description = "JWT Authorization header using the Bearer scheme.",
			Name = "Authorization",
			In = ParameterLocation.Header,
			Type = SecuritySchemeType.Http,
			Scheme = "bearer"
		});
		c.AddSecurityRequirement(new OpenApiSecurityRequirement
		{
			{
				new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference
					{
						Type = ReferenceType.SecurityScheme,
						Id = "Bearer"
					}
				},
				new string[] { }
			}
		});
	});

	builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

}

static void SetTracing(WebApplicationBuilder builder, IConnectionMultiplexer redisCon)
{
	builder.Services.AddOpenTelemetry()
		.WithTracing(tracerProviderBuilder =>
			tracerProviderBuilder
				.AddSource("EventStaf-Api")
				.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("EventStaf-Api"))
				.AddAspNetCoreInstrumentation()
				.AddRedisInstrumentation(redisCon)
				.AddSqlClientInstrumentation(options =>
				{
					options.SetDbStatementForText = true;
					options.RecordException = true;
				})
				.AddOtlpExporter(options =>
				{
					options.Endpoint = new Uri("http://jaeger:4317"); // OTLP gRPC endpoint
				}))
		.WithMetrics(meterProviderBuilder =>
			meterProviderBuilder
				.AddAspNetCoreInstrumentation()
				.AddOtlpExporter(options =>
				{
					options.Endpoint = new Uri("http://jaeger:4318"); // OTLP gRPC endpoint
				}));
}

static async Task InitMigrateAndSeed(WebApplication app)
{
	using (var scope = app.Services.CreateScope())
	{
		var services = scope.ServiceProvider;
		var logger = services.GetRequiredService<ILogger<Program>>();


		try
		{
			logger.LogInformation("Attempting to ensure database is created and up to date.");
			var context = services.GetRequiredService<ApplicationDbContext>();

			// Check if the database exists
			bool dbExists = context.Database.CanConnect();

			if (!dbExists)
			{
				logger.LogInformation("Database does not exist. Creating database...");
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
				logger.LogInformation("=====before migrating");
				//context.Database.Migrate();
				logger.LogInformation("=====after migrating");
			}

			// Seed data
			var _userService = services.GetService<IUserService<AppUser>>();
			if (!_userService.AnyAsync().Result.Value)
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
		return;
		
	}
}