using System.Diagnostics;

namespace LoggingUtils
{
    public class WindowsEventLogLogger : ILogger
    {
        string _source { get; set; }
        string _log { get; set; }

        public WindowsEventLogLogger(string source, string log = @"Application")
        {
            _source = source;
            _log = log;
            if (!EventLog.SourceExists(_source))
                EventLog.CreateEventSource(_source, _log);
        }
        public void Log(LogLevels level, string message)
        {
            switch (level)
            {
                case LogLevels.Information:
                    EventLog.WriteEntry(_source, message,EventLogEntryType.Information);
                    break;
                case LogLevels.Warning:
                    EventLog.WriteEntry(_source, message, EventLogEntryType.Warning);
                    break;
                case LogLevels.Error:
                    EventLog.WriteEntry(_source, message, EventLogEntryType.Error);
                    break;
                default:
                    break;
            }
        }
    }
}
