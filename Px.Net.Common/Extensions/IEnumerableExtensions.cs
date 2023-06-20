using System;
namespace Px.Net.Common.Extensions
{
	public static class IEnumerableExtensions
	{
		public static string Readable<TData>(this IEnumerable<TData>? data)
		{
			return data == null ? "[]" : $"[{string.Join(", ", data)}]";
		}

	}
}

