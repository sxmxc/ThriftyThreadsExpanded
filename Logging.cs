namespace ThriftyThreadsExpanded;

internal static class Log
{
    public static void Message(string message) => Core.Instance?.LoggerInstance.Msg(message);
    public static void Warning(string message) => Core.Instance?.LoggerInstance.Warning(message);
    public static void Error(string message) => Core.Instance?.LoggerInstance.Error(message);
}
