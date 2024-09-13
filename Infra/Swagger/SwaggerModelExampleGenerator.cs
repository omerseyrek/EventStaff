using System.Reflection;

namespace EventStaf.Infra.Swagger
{
	public static class SwaggerModelExampleGenerator
	{
		
		public static List<string> names = new List<string>(){ "john", "dexter", "joe", "susan", "donalt", "bill" };
		public static List<string> lastNames = new List<string>(){ "morgan", "rock", "clinton", "eastwood", "grant", "regan" };
		public static List<string> domainNames = new List<string>(){ "gmail.com", "lycos.com", "mynet.com", "google.com" };

		public static T? GenerateExample<T>() where T : class
		{
			T? example = Activator.CreateInstance(typeof(T)) as T;
			PropertyInfo[] properties = example?.GetType()
									  .GetProperties() ?? new PropertyInfo[]{ };

			foreach (PropertyInfo? property in properties.Where(p => p != null).ToList())
			{
				SwaggerPropertyAttribute? schemaAttribute = property?.GetCustomAttributes<SwaggerPropertyAttribute>()?.FirstOrDefault() ?? null;
				if (schemaAttribute == null)
				{
					property?.SetValue(example, null);
				}

				if (property?.PropertyType?.IsByRef ?? false)
				{
					var propertyValue = CrateReferenceProperty(property.GetValue(example));
					property.SetValue(example, propertyValue);
				}
				else 
				{
					object? sampledData = SwaggerModelExampleGenerator.GenerateSamplePropertyData(schemaAttribute?.SwaggerPropertyType);
					property?.SetValue(example, sampledData);
				}
			}

			return example;
		}

		private static object? GenerateSamplePropertyData(SwaggerPropertyType? swaggerPropertyType)
		{
			object? result = null;
			Random random = new Random();
			switch (swaggerPropertyType)
			{
				case SwaggerPropertyType.Name:
					result = names[random.Next(0, names.Count - 1)];
					break;
				case SwaggerPropertyType.LastName:
					result = lastNames[random.Next(0, names.Count - 1)];
					break;
				case SwaggerPropertyType.UserName:
					result = $"{names[random.Next(0, names.Count - 1)]}_{lastNames[random.Next(0, lastNames.Count - 1)]}";
					break;
				case SwaggerPropertyType.Password:
				    result = "Test123";
					break;
				case SwaggerPropertyType.MaskedPasword:
					result = "*********";
					break;
				case SwaggerPropertyType.Email:
					result = $"{names[random.Next(0, names.Count - 1)]}_{lastNames[random.Next(0, lastNames.Count - 1)]}@{domainNames[random.Next(0, domainNames.Count - 1)]}";
					break;
				case SwaggerPropertyType.PhoneNumber:
					result = "5555555555";	
					break;
				case SwaggerPropertyType.BirthDate:
					result = DateTime.Now.AddDays(-1 * random.Next(18*365, 100*365));
					break;
				case SwaggerPropertyType.RegisterDate:
					result = DateTime.Now.AddDays(-0 * random.Next(0, 15*365));
					break;
				case SwaggerPropertyType.Salary:
					result = DateTime.Now.AddYears(-0 * random.Next(2000, 20000000));
					break;
				default:
					break;
			}
			return result;
		}


		public static object? CrateReferenceProperty(object? propertyItem)
		{
			if (propertyItem == null)
			{
				return null;
			}

			object? example = Activator.CreateInstance(propertyItem.GetType());
			if (example == null)
			{
				return null;
			}

			PropertyInfo[] properties = example?.GetType()?.GetProperties() ?? Array.Empty<PropertyInfo>();
			foreach (PropertyInfo property in properties.Where(p => p != null).ToList())
			{ 
				var schemaAttribute = property.GetCustomAttribute(typeof(SwaggerPropertyAttribute)) as SwaggerPropertyAttribute;
				if (schemaAttribute == null)
				{
					continue;
				}

				if (property.PropertyType.IsValueType)
				{
					var sampleValue = CrateReferenceProperty(property?.GetValue(example));
					property?.SetValue(example, sampleValue);
				}
				else 
				{
					var sampledObject = GenerateSamplePropertyData(schemaAttribute.SwaggerPropertyType);
					property?.SetValue(example, sampledObject);
				}
			}

			return example;
		}
	}


}
