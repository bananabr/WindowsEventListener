using System;
using System.Diagnostics;
using System.Text;

namespace LoggingUtils
{
    public class TraceLogger : ILogger
    {
        public TraceLogger()
        {
        }
        public TraceLogger(params TraceListener[] listeners)
        {
            foreach (var l in listeners)
            {
                Trace.Listeners.Add(l);
            }
        }
        private void Information(string message)
        {
            Trace.TraceInformation(message);
        }

        private void Information(string fmt, params object[] vars)
        {
            Trace.TraceInformation(fmt, vars);
        }

        private void Information(Exception exception, string fmt, params object[] vars)
        {
            Trace.TraceInformation(FomattingHelpers.FormatExceptionMessage(exception, fmt, vars));
        }

        private void Warning(string message)
        {
            Trace.TraceWarning(message);
        }

        private void Warning(string fmt, params object[] vars)
        {
            Trace.TraceWarning(fmt, vars);
        }

        private void Warning(Exception exception, string fmt, params object[] vars)
        {
            Trace.TraceWarning(FomattingHelpers.FormatExceptionMessage(exception, fmt, vars));
        }

        private void Error(string message)
        {
            Trace.TraceError(message);
        }

        private void Error(string fmt, params object[] vars)
        {
            Trace.TraceError(fmt, vars);
        }

        private void Error(Exception exception, string fmt, params object[] vars)
        {
            Trace.TraceError(FomattingHelpers.FormatExceptionMessage(exception, fmt, vars));
        }

        private void TraceApi(string componentName, string method, TimeSpan timespan)
        {
            TraceApi(componentName, method, timespan, "");
        }

        private void TraceApi(string componentName, string method, TimeSpan timespan, string fmt, params object[] vars)
        {
            TraceApi(componentName, method, timespan, string.Format(fmt, vars));
        }
        private void TraceApi(string componentName, string method, TimeSpan timespan, string properties)
        {
            string message = String.Concat("Component:", componentName, ";Method:", method, ";Timespan:", timespan.ToString(), ";Properties:", properties);
            Trace.TraceInformation(message);
        }

        public void Log(LogLevels level, string message)
        {
            switch (level)
            {
                case LogLevels.Information:
                    this.Information(message);
                    break;
                case LogLevels.Warning:
                    this.Warning(message);
                    break;
                case LogLevels.Error:
                    this.Error(message);
                    break;
                default:
                    throw new MissingMethodException();
            }
            Trace.Flush();
        }

        public void Log(LogLevels level, string fmt, params object[] vars)
        {
            switch (level)
            {
                case LogLevels.Information:
                    this.Information(fmt, vars);
                    break;
                case LogLevels.Warning:
                    this.Warning(fmt, vars);
                    break;
                case LogLevels.Error:
                    this.Error(fmt, vars);
                    break;
                default:
                    throw new MissingMethodException();
            }
            Trace.Flush();
        }

        public void Log(LogLevels level, Exception exception, string fmt, params object[] vars)
        {
            switch (level)
            {
                case LogLevels.Information:
                    this.Information(exception, fmt, vars);
                    break;
                case LogLevels.Warning:
                    this.Warning(exception, fmt, vars);
                    break;
                case LogLevels.Error:
                    this.Error(exception, fmt, vars);
                    break;
                default:
                    throw new MissingMethodException();
            }
            Trace.Flush();
        }
    }
}
