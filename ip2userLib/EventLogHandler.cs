using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using LoggingUtils;
using EventLogListener;
using EventLogListener.Filters.Strategies;
using EventLogListener.Handlers;
using EventLogListener.Loggers;

namespace WindowsEventLogger
{
    public class EventLogHandler : EventLogHandlerBase, IEventLogHandler, IEventFilterable, ILogable
    {
        // This member is used to wait for events.
        private List<IEventLogger> loggers;
        private IList<IEventFilter> filters;
        private IEventFilterStrategy filterStrategy;
        ILogger exlogger;

        public EventLogHandler()
        {
            loggers = new List<IEventLogger>();
            filters = new List<IEventFilter>();
            filterStrategy = new AllMustMatch();
        }

        public void RegisterFilter(IEventFilter filter)
        {
            filters.Add(filter);
        }

        public void SetFilterStrategy(IEventFilterStrategy stg)
        {
            filterStrategy = stg;
        }

        public void RegisterLogger(IEventLogger logger)
        {
            loggers.Add(logger);
        }

        public void SetExceptionLogger(ILogger logger)
        {
            exlogger = logger;
        }

        protected override void _handleEntryWritten(object source, EntryWrittenEventArgs e)
        {
            try
            {
                if (filterStrategy.Filter(filters, e) != null)
                {
                    loggers.ForEach(l => l.log(e));
                }
            }
            catch (Exception ex)
            {
                if (exlogger != null)
                {
                    exlogger.Log(LogLevels.Error, ex.Message);
                }
            }
        }
    }
}
