using EventStaf.Models;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace EventStaf.Infra.Swagger
{
    public class ExampleSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null)
            {
                return;
            }

            foreach (var property in schema.Properties)
            {
                SwaggerSchemaExampleAttribute schemeAttribute = null;

                if (context.MemberInfo != null)
                {
                    schemeAttribute = context.MemberInfo.GetCustomAttributes<SwaggerSchemaExampleAttribute>().FirstOrDefault();
                    continue;
                }

                if (context.ParameterInfo != null)
                {
                    schemeAttribute = context.ParameterInfo.GetCustomAttributes<SwaggerSchemaExampleAttribute>().FirstOrDefault();
                    continue;
                }


                if (schemeAttribute != null)
                {
                    property.Value.Example = new OpenApiString(schemeAttribute.Example.ToString());
                }
            }
        }
    }
}
