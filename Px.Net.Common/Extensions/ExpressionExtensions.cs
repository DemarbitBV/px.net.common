using System;
using System.Linq.Expressions;
using Px.Net.Common.Visitors;

namespace Px.Net.Common.Extensions
{
	public static class ExpressionExtensions
	{
		/// <summary>
		/// Get a readable representation of the Expression
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static string Readable(this Expression? expression)
		{
			if (expression == null)
			{
				return "<none>";
			}

			var visitor = new ExpressionStringBuilderVisitor();
			visitor.Visit(expression);
			return visitor.ToString();
		}
	}
}

