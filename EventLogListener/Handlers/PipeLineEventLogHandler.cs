using EventLogListener.Filters.Strategies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace EventLogListener.Handlers
{
    public class PipeLineEventLogHandler : IEventLogHandler, IEventFilterable
    {
        // This member is used to wait for events.
        private AutoResetEvent signal;
        private IList<IEventLogger> loggers;
        private IList<IEventFilter> filters;
        private IEventFilterStrategy filterStrategy;

        public PipeLineEventLogHandler()
        {
            loggers = new List<IEventLogger>();
            filters = new List<IEventFilter>();
            filterStrategy = new AllMustMatch();
        }

        public void HandleEntryWritten(object source, EntryWrittenEventArgs e)
        {
            signal.Set();
            if (filterStrategy.Filter(filters, e) != null) {
                foreach (var logger in loggers)
                {
                    logger.log(e);
                }
            }
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

        public void RegisterSignal(AutoResetEvent _signal)
        {
            signal = _signal;
        }
    }
}
