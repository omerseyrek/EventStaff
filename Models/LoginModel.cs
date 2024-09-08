using EventStaf.Infra.Result;
using EventStaf.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace EventStaf.Models
{
	public class LoginModel
	{
		[Required]
		[SwaggerSchema(Description = "Username for authentication")]
		public string Username { get; set; }

		[Required]
		[SwaggerSchema(Description = "Username for authentication")]
		public string Password { get; set; }
	}

	public class LoginResultModel
	{
		public string JwtToken { get; set; }
	}
}


//public class BaseModelExample : Swashbuckle.AspNetCore.Filters.IExamplesProvider<LoginModel>
//{
//	public LoginModel GetExamples()
//	{
//		return new LoginModel()
//		{
//			Username = "omer_seyrek111",
//			Password = "Test123"
//		};
//	}
//}

public class LoginResultModelExample : Swashbuckle.AspNetCore.Filters.IExamplesProvider<Result<LoginResultModel>>
{
	public Result<LoginResultModel> GetExamples()
	{
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



public class LoginModelExample : Swashbuckle.AspNetCore.Filters.IExamplesProvider<LoginModel>
{
	public LoginModel GetExamples()
	{
		return new LoginModel() 
		{
			Username = "omer_seyrek111",
			Password = "Test123"
		};
	}	
}
