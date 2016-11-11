using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EventLogListener
{
    public interface IEventLogger
    {
        void log(EntryWrittenEventArgs e);
    }
}
