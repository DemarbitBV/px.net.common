using System;
using Px.Net.Common.Models;

namespace Px.Net.Common.Utilities
{
	public static class EnumUtils
	{
		public static IEnumerable<EnumDto> GetValues<TEnum>()
			where TEnum : Enum
		{
			return Enum.GetValues(typeof(TEnum))
				.Cast<TEnum>()
				.Select(v => new EnumDto { Label = v.ToString(), Value = Convert.ToInt32(v) });
		}
	}
}

