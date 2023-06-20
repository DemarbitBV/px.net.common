using System;
using System.Text;

namespace Px.Net.Common.Extensions
{
	public static class DictionaryExtensions
	{
		/// <summary>
		/// Get a readable representation of the Dictionary. 
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dict"></param>
		/// <param name="useFlatList">If <c>true</c> the dictionary is return on a single line. Otherwise it will be formatted over multiple lines.</param>
		/// <returns></returns>
		public static string Readable<TKey, TValue>(this Dictionary<TKey, TValue>? dict, bool useFlatList = true)
			where TKey : notnull
		{
			if (dict == null)
			{
				return "{}";
			}

			var items = new List<string>();

			foreach (var item in dict)
				items.Add($@"""{item.Key}"": {item.Value}");

			var sb = new StringBuilder();

			if (useFlatList)
			{
				sb.Append("{ ");
				sb.AppendJoin(", ", items);
				sb.Append(" }");
			} else
			{
				sb.Append("{ ");
				items.ForEach(item => sb.Append('\t').AppendLine(item));
				sb.Append(" }");
			}

			return sb.ToString();
		}
	}
}

