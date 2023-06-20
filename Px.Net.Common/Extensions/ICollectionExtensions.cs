using System;
namespace Px.Net.Common.Extensions
{
	public static class ICollectionExtensions
	{
		/// <summary>
		/// Get a readable representation of the ICollection set.
		/// </summary>
		/// <typeparam name="TData"></typeparam>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string Readable<TData>(this ICollection<TData>? data)
		{
			return data == null ? "[]" : $"[{string.Join(", ", data)}]";
		}
	}
}

