using EventStaf.Models;
using EventStaf.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
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
		[SwaggerResponse(200, "Login successful", typeof(Result<LoginResultModel>))]
		[SwaggerResponse(400, "Invalid username or password", typeof(Result<LoginResultModel>))]
		public ActionResult<Result<LoginResultModel>> Login([FromBody] LoginModel model)
		{
			// TODO: Validate user credentials against your database
			if (IsValidUser(model.Username, model.Password))
			{
				var token = GenerateJwtToken(model.Username);
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
			return _userService.FindAsync(u => u.Username == username && u.PasswordHash == hashedPwd).Result.Value.Any();
		}

		private string GenerateJwtToken(string username)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, username),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(120),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}

}
