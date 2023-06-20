using System;
using System.Text;

namespace Px.Net.Common.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Get a single message of the exception and it's descendants (InnerExceptions).
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="includeStackTrace"></param>
        /// <returns></returns>
        public static string GetFlattenedMessage(this Exception exception, bool includeStackTrace = false)
        {
            var builder = new StringBuilder();

            builder.AppendLine(exception.Message);

            if (exception.InnerException != null)
            {
                builder.AppendLine(exception.InnerException.GetFlattenedMessage(includeStackTrace: false));
            }

            if (includeStackTrace)
            {
                builder.AppendLine($"Stack trace: {exception.StackTrace}");
            }

            return builder.ToString();
        }
    }
}

