using EventLogListener.Filters.Strategies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using LoggingUtils;
using EventLogListener.Loggers;

namespace EventLogListener.Handlers
{
    public abstract class EventLogHandlerBase : IEventLogHandler
    {
        // This member is used to wait for events.
        private AutoResetEvent signal;
        public void RegisterSignal(AutoResetEvent _signal)
        {
            signal = _signal;
        }
        public void HandleEntryWritten(object source, EntryWrittenEventArgs e)
        {
            signal.Set();
            _handleEntryWritten(source,e);
        }
        protected abstract void _handleEntryWritten(object source, EntryWrittenEventArgs e);
    }
}
