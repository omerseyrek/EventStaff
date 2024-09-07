using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace EventStaf.Models
{
	public class LoginModel
	{
		[Required]
		[SwaggerSchema(Description = "Username for authentication")]
		[SwaggerSchemaExample("johndoe")]
		public string Username { get; set; }

		[Required]
		[SwaggerSchema(Description = "Username for authentication")]
		[SwaggerSchemaExample("johndoe")]
		public string Password { get; set; }
	}

	public class LoginResultModel
	{
		public string JwtToken { get; set; }
	}
}
