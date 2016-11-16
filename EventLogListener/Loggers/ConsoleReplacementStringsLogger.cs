using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EventLogListener.Loggers
{
    public class ConsoleReplacementStringsLogger : IEventLogger
    {
        public void log(EntryWrittenEventArgs e)
        {
            var rs = e.Entry.ReplacementStrings.Select((s,i) => String.Format(@"{0}: {1}",i,s));
            Console.WriteLine(string.Join(@",",rs));
        }
    }
}
