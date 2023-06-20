using System;
namespace Px.Net.Common.Extensions
{
	public static class ArrayExtensions
	{
		public static string Readable<TData>(this TData[]? array)
		{
			return array == null ? "[]" : $"[{string.Join(", ", array)}]";
		}
	}
}

