using System;
namespace Px.Net.Common.Models
{
	public class MediatorResult
	{
		private readonly bool _succeeded;
		private readonly object? _data;

		private readonly Exception? _exception;
		private readonly string? _errorMessage;

		public bool Succeeded =>
			_succeeded;

		public object? Data =>
			_data;

		public string ErrorMessage =>
			_exception?.Message ?? _errorMessage ?? "?";

		private MediatorResult(bool succeeded, object? data = null, Exception? exception = null, string? errorMessage = null)
		{
			_succeeded = succeeded;
			_data = data;
			_exception = exception;
			_errorMessage = errorMessage;
		}

		public static MediatorResult HasFailed(Exception exception) =>
			new(false, exception: exception, errorMessage: exception.Message);

		public static MediatorResult HasFailed(string errorMessage) =>
			new(false, errorMessage: errorMessage);

		public static MediatorResult HasSucceeded(object? data = null)
			=> new(true, data);
	}
}

