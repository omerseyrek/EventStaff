using EventStaf.Entities;
using EventStaf.Infra.Configuration;
using EventStaf.Infra.Constants;
using EventStaf.Infra.Result;
using EventStaf.Models;
using EventStaf.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventStaf.Controllers
{

    [ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly IUserService<AppUser> _userService;
		

		public AuthController(IConfiguration configuration, IUserService<AppUser> userService)
		{
			_configuration = configuration;
			_userService = userService;
		}


		[HttpPost("login")]
		[SwaggerOperation(
			Summary = "Authenticate user and return JWT token",
			Description = "This endpoint validates the user credentials and returns a JWT token if successful.")]
		[SwaggerResponse(StatusCodes.Status200OK, "Login successful", typeof(Result<LoginResultModel>))]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid username or password", typeof(Result<LoginResultModel>))]
		[SwaggerResponseExample(StatusCodes.Status200OK, typeof(LoginSuccesResultModelExample))]
		[SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(LoginFailureResultModelExample))]
		public ActionResult<Result<LoginResultModel>> Login([FromBody][SwaggerRequestBody("User information", Required = true)] LoginModel model)
		{
			// TODO: Validate user credentials against your database
			if (IsValidUser(model?.Username ?? string.Empty, model?.Password ?? string.Empty))
			{
				var token = GenerateJwtToken(model?.Username ?? string.Empty);
				LoginResultModel resultModel = new LoginResultModel()
				{
					JwtToken = token
				};
				return Ok(Result<LoginResultModel>.Success(resultModel));
			}

			return BadRequest(Result<LoginResultModel>.Failure("Invalid username or password.."));
		}


		private bool IsValidUser(string username, string password)
		{
			var hashedPwd = EventStaf.Data.DataSeeder.GetHashFromString(password);
			return _userService.FindAsync(u => u.Username == username && u.PasswordHash == hashedPwd).Result?.Value?.Any() ?? false;
		}

		private string GenerateJwtToken(string username)
		{
			var jwtConfiguration =  _configuration.GetSection(ConstantKeys.Jwt).Get<Jwt>();

			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration?.Key ?? string.Empty));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, username),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};

			var token = new JwtSecurityToken(
				issuer: jwtConfiguration?.Issuer ?? string.Empty,
				audience: jwtConfiguration?.Audience ?? string.Empty,
				claims: claims,
				expires: DateTime.Now.AddMinutes(120),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}

}
