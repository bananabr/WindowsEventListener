using EventLogListener;
using EventLogListener.Filters;
using EventLogListener.Filters.Strategies;
using EventLogListener.Handlers;
using EventLogListener.Loggers;
using LoggingUtils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using System.Threading;

namespace WindowsEventLogger
{
    class Program
    {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        static string logName;
        static List<long> validLogonEvents = new List<long>();
        static ILogger logger = new ConsoleLogger();
        static IEnumerable<string> codesToWatch;
        static PipeLineEventLogHandler handler;

        static void Init()
        {
            logName = Properties.Settings.Default["LogName"].ToString();
            codesToWatch = Properties.Settings.Default["EventCodesToWatch"].ToString().Split(',');
            handler = new PipeLineEventLogHandler();
            IEventLogger consoleLogger = new ConsoleEventEntryPropertyLogger("Message");
            IEventFilter idFilter = new EventCodeFilter(validLogonEvents);
            handler.RegisterLogger(consoleLogger);
            handler.RegisterFilter(idFilter);
            handler.SetFilterStrategy(new AllMustMatch());
        }

        static int Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            foreach (var code in codesToWatch)
            {
                validLogonEvents.Add(long.Parse(code));
            }

            try
            {
                Init();
                WindowsEventLogListener listener = new WindowsEventLogListener(logName, handler);
            }
            catch (SecurityException)
            {
                logger.Log(LogLevels.Error, String.Format(@"Permission denied when trying to access the '{0}' log.", logName));
                return 1;
            }
            catch (Exception e)
            {
                logger.Log(LogLevels.Error, FomattingHelpers.FormatExceptionMessage(e,@"Something bad hapenned, do something about it!",null));
                return 1;
            }
            _quitEvent.WaitOne();
            return 0;
        }
    }
}