using EventStaf.Infra.Result;
using EventStaf.Infra.Swagger;
using EventStaf.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EventStaf.Models
{
	public class LoginModel
	{
		[Required]
		[SwaggerSchema(description: "UserName for authentication...")]
		[StringLength(20, MinimumLength = 6)]
		[SwaggerParameter("User's full name", Required = true)]
		[SwaggerProperty(SwaggerPropertyType.UserName)]
		public string Username { get; set; }

		[Required]
		[SwaggerSchema(description: "Password for authentication...")]
		[StringLength(20, MinimumLength = 6)]
		[SwaggerProperty(SwaggerPropertyType.Password)]
		public string Password { get; set; }
	}

	public class LoginResultModel
	{
		public string JwtToken { get; set; }

	}

	public class LoginSuccesResultModelExample : Swashbuckle.AspNetCore.Filters.IExamplesProvider<Result<LoginResultModel>>
	{
		public Result<LoginResultModel> GetExamples()
		{
			//SwaggerModelExampleGenerator.GenerateExample<Result<LoginResultModel>>()

			return new Result<LoginResultModel>()
			{
				IsSuccess = true,
				Value = new LoginResultModel()
				{
					JwtToken = "eyJhbGciOiAibm9uZSIsICJ0eXAiOiAiSldUIn0K.eyJ1c2VybmFtZSI6ImFkbWluaW5pc3RyYXRvciIsImlzX2FkbWluIjp0cnVlLCJpYXQiOjE1MTYyMzkwMjIsImV4cCI6MTUxNjI0MjYyMn0.",
				}
			};
		}
	}

	public class LoginFailureResultModelExample : Swashbuckle.AspNetCore.Filters.IExamplesProvider<Result<LoginResultModel>>
	{
		public Result<LoginResultModel> GetExamples()
		{
			return new Result<LoginResultModel>()
			{
				IsSuccess = false,
				Errors = new List<string>() { "Invalid username or password.." },
			};
		}
	}

	public class LoginModelExample : Swashbuckle.AspNetCore.Filters.IExamplesProvider<LoginModel>
	{
		public LoginModel GetExamples()
		{
			var model = SwaggerModelExampleGenerator.GenerateExample<LoginModel>();
			return model;
		}
	}

}
