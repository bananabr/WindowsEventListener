using EventLogListener;
using System.Diagnostics;
using System.Threading;

public class WindowsEventLogListener
{
    // This member is used to wait for events.
    static AutoResetEvent signal;

    public WindowsEventLogListener(string log, IEventLogHandler handler)
    {
        EventLog evtLog = new EventLog(log);
        evtLog.EntryWritten += new EntryWrittenEventHandler(handler.HandleEntryWritten);
        evtLog.EnableRaisingEvents = true;
        signal = new AutoResetEvent(false);
        handler.RegisterSignal(signal);
        signal.WaitOne();
    }
}
