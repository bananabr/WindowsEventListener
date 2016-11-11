using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace EventLogListener
{
    public interface IEventLogHandler
    {
        void HandleEntryWritten(object source, EntryWrittenEventArgs e);
        void RegisterSignal(AutoResetEvent signal);
    }
}
