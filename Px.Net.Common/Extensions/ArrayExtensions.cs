using System;
namespace Px.Net.Common.Extensions
{
	public static class ArrayExtensions
	{
		/// <summary>
		/// Get a readable representation of the array
		/// </summary>
		/// <typeparam name="TData"></typeparam>
		/// <param name="array"></param>
		/// <returns></returns>
		public static string Readable<TData>(this TData[]? array)
		{
			return array == null ? "[]" : $"[{string.Join(", ", array)}]";
		}
	}
}

