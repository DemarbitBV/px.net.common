using System;
using System.Reflection;

namespace Px.Net.Common.Extensions
{
	public static class ClassExtensions
	{
		public static void Merge<TEntity>(this TEntity source, TEntity mergeWith)
			where TEntity : class
		{
			var writeableProperties = source.GetWriteableProperties();

			foreach (var property in writeableProperties)
			{
				if (property.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
					continue;

				property.SetValue(source, property.GetValue(mergeWith));
			}
		}

		public static void Merge<TEntity>(this TEntity source, Dictionary<string, object?> values)
			where TEntity : class
		{
			var writeableProperties = source.GetWriteableProperties();

			foreach (var pair in values)
			{
				var property = writeableProperties.FirstOrDefault(p => p.Name.Equals(pair.Key, StringComparison.OrdinalIgnoreCase));

                if (property == null || property.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                    continue;

                property.SetValue(source, pair.Value);
			}
		}

		public static PropertyInfo[] GetWriteableProperties<TEntity>(this TEntity source)
			where TEntity : class
		{
			return typeof(TEntity)
                .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
        }
	}
}

