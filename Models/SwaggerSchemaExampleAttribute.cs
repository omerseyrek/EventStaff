namespace EventStaf.Models
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class SwaggerSchemaExampleAttribute : Attribute
	{
		public object Example { get; }

		public SwaggerSchemaExampleAttribute(object example)
		{
			Example = example;
		}
	}
}
