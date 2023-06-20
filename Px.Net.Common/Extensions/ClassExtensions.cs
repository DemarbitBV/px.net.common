using System;
using System.Reflection;

namespace Px.Net.Common.Extensions
{
	public static class ClassExtensions
	{
		/// <summary>
		/// Merge two objects. If an Id property exists on the generic type, that property will be ignored.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="source">Object that should be updated</param>
		/// <param name="mergeWith">Object containing new values</param>
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

		/// <summary>
		/// Merge the values defined in the dictionary with the source object.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="source">Object that should be updated</param>
		/// <param name="values">A dictionary where the keys contain the property names</param>
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

		/// <summary>
		/// Return all writeable properties (Public, Instance , Declared) for a given type.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static PropertyInfo[] GetWriteableProperties<TEntity>(this TEntity source)
			where TEntity : class
		{
			return typeof(TEntity)
                .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
        }
	}
}

