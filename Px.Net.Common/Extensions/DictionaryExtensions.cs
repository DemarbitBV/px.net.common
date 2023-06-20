using System;
using System.Text;

namespace Px.Net.Common.Extensions
{
	public static class DictionaryExtensions
	{
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

