using Microsoft.Extensions.Logging;

namespace NHibernate.AspNetCore.DataProtection
{
    internal static partial class LoggingExtensions
    {
        [LoggerMessage(1, LogLevel.Debug, "Reading data with key '{FriendlyName}', value '{Value}'.", EventName = "ReadKeyFromElement")]
        public static partial void ReadingXmlFromKey(this ILogger logger, string? friendlyName, string? value);

        [LoggerMessage(2, LogLevel.Debug, "Saving key '{FriendlyName}' to Session.", EventName = "SavingKeyToSession")]
        public static partial void LogSavingKeyToSession(this ILogger logger, string friendlyName);
    }
}
