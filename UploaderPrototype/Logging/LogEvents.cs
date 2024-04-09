namespace UploaderPrototype.Logging;

public static class LogEvents
{
    private static readonly Action<ILogger, string, Exception> _logError =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1, nameof(LogError)),
            "An error occurred: {Message}");

    public static void LogError(ILogger logger, Exception ex)
    {
        _logError(logger, ex.Message, ex);
    }
}