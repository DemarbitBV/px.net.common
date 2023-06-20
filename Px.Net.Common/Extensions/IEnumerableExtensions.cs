using System;
namespace Px.Net.Common.Extensions
{
	public static class IEnumerableExtensions
	{
		/// <summary>
		/// Get a readable representation of the IEnumerable set.
		/// </summary>
		/// <typeparam name="TData"></typeparam>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string Readable<TData>(this IEnumerable<TData>? data)
		{
			return data == null ? "[]" : $"[{string.Join(", ", data)}]";
		}

	}
}

