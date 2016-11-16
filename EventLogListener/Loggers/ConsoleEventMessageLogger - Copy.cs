using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EventLogListener.Loggers
{
    public class ConsoleEventMessageLogger : IEventLogger
    {
        public void log(EntryWrittenEventArgs e)
        {
            Console.WriteLine(e.Entry.Message);
        }
    }
}
