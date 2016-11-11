using EventLogListener;
using EventLogListener.Filters;
using EventLogListener.Filters.Strategies;
using EventLogListener.Handlers;
using EventLogListener.Loggers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace WindowsEventLogger
{
    class Program
    {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);
        private static IEventFilterStrategy filterStrategy;
        private static List<long> validLogonEvents = new List<long>();

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            foreach (var code in Properties.Settings.Default["EventCodesToWatch"].ToString().Split(','))
            {
                validLogonEvents.Add(long.Parse(code));
            }

            PipeLineEventLogHandler handler = new PipeLineEventLogHandler();
            IEventLogger consoleLogger = new ConsoleEventEntryPropertyLogger("Message");
            filterStrategy = Assembly.Load(@"EventLogListener").CreateInstance(
                string.Format(@"EventLogListener.Filters.Strategies.{0}", Properties.Settings.Default["FilterStrategy"].ToString())
                ) as IEventFilterStrategy;
            IEventFilter idFilter = new EventCodeFilter(validLogonEvents);

            handler.RegisterLogger(consoleLogger);
            handler.RegisterFilter(idFilter);
            handler.SetFilterStrategy(filterStrategy);

            WindowsEventLogListener listener = new WindowsEventLogListener(Properties.Settings.Default["LogName"].ToString(), handler);
            _quitEvent.WaitOne();
        }
    }
}