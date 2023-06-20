using System;
namespace Px.Net.Common.Models
{
	/// <summary>
	/// Structured Dto for enums
	/// </summary>
	public class EnumDto
	{
		/// <summary>
		/// String representation of the Enum value using the <c>ToString()</c> method.
		/// </summary>
		public string Label { get; set; } = null!;

		/// <summary>
		/// Integer value representation of the Enum value
		/// </summary>
		public int Value { get; set; }
	}
}

