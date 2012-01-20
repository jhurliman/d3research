using System;

namespace libdiablo3
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error
    }

    public delegate void LogHandler(LogLevel level, string message, Exception ex);

    public static class Log
    {
        public static event LogHandler OnLogMessage;

        public static void Debug(string message)
        {
            OnLogMessage(LogLevel.Debug, message, null);
        }

        public static void Info(string message)
        {
            OnLogMessage(LogLevel.Info, message, null);
        }

        public static void Warn(string message)
        {
            OnLogMessage(LogLevel.Warn, message, null);
        }

        public static void Error(string message)
        {
            OnLogMessage(LogLevel.Error, message, null);
        }

        public static void Error(string message, Exception ex)
        {
            OnLogMessage(LogLevel.Error, message, ex);
        }
    }
}
