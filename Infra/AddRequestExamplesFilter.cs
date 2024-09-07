using EventStaf.Models;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EventStaf.Infra
{
	public class AddRequestExamplesFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			if (operation.RequestBody == null)
				return;

			var requestBodyContent = operation.RequestBody.Content;

			foreach (var mediaType in requestBodyContent.Values)
			{
				if (context.ApiDescription.ActionDescriptor.Parameters.FirstOrDefault()?.ParameterType == typeof(LoginModel))
				{
					mediaType.Schema.Example = new OpenApiObject
					{
						["username"] = new OpenApiString("johndoe"),
						["password"] = new OpenApiString("password123")
					};
				}
			}
		}
	}
}
