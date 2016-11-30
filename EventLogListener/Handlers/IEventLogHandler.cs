using System.Diagnostics;
using System.Threading;

namespace EventLogListener
{
    public interface IEventLogHandler
    {
        void HandleEntryWritten(object source, EntryWrittenEventArgs e);
        void RegisterSignal(AutoResetEvent signal);
    }
}
