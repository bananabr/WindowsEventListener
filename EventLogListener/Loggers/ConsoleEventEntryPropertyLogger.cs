using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EventLogListener.Loggers
{
    public class ConsoleEventEntryPropertyLogger : IEventLogger
    {
        private string _propertyName;

        public ConsoleEventEntryPropertyLogger(string propertyName)
        {
            _propertyName = propertyName;
        }
        public void log(EntryWrittenEventArgs e)
        {
            Console.WriteLine(e.Entry.GetType().GetProperty(_propertyName).GetValue(e.Entry,null).ToString());
        }
    }
}
