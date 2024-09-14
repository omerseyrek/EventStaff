namespace EventStaf.Infra
{
	public static class GenericMapper
	{
		public static TDestination Map<TSource, TDestination>(TSource source)
			where TDestination : new()
		{
			if (source == null)
				return default;

			var destination = new TDestination();
			var sourceProperties = typeof(TSource).GetProperties();
			var destinationProperties = typeof(TDestination).GetProperties();

			foreach (var sourceProperty in sourceProperties)
			{
				var destinationProperty = Array.Find(destinationProperties,
					p => p.Name == sourceProperty.Name && p.PropertyType == sourceProperty.PropertyType);

				if (destinationProperty != null && destinationProperty.CanWrite)
				{
					var value = sourceProperty.GetValue(source);
					destinationProperty.SetValue(destination, value);
				}
			}

			return destination;
		}

		public static TDestination MapTo<TDestination>(this object source)
			where TDestination : new()
		{
			return Map<object, TDestination>(source);
		}
	}
}
