using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Reflection;

namespace EventStaf.Infra.Swagger
{


	public class DescriptionSchemaFilter : ISchemaFilter
	{
		public void Apply(OpenApiSchema schema, SchemaFilterContext context)
		{
			if (context.MemberInfo != null)
			{
				var description = context.MemberInfo.GetCustomAttribute<SwaggerSchemaAttribute>()?.Description;
				if (description != null)
				{
					schema.Description = description;
					return;
				}
			}
		}
	}


}
