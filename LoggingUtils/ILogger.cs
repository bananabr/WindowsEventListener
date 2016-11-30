namespace LoggingUtils
{
    public enum LogLevels
    {
        Information,
        Warning,
        Error
    }
    public interface ILogger
    {
        void Log(LogLevels level, string message);
        //void Log(LogLevels level, string fmt, params object[] vars);
        //void Log(LogLevels level, Exception exception, string fmt, params object[] vars);
    }
}
