using System;
namespace Px.Net.Common.Extensions
{
	public static class ICollectionExtensions
	{
		public static string Readable<TData>(this ICollection<TData>? data)
		{
			return data == null ? "[]" : $"[{string.Join(", ", data)}]";
		}
	}
}

