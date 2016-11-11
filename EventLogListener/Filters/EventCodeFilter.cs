using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EventLogListener.Filters
{
    public class EventCodeFilter : IEventFilter
    {
        public List<long> ValidCodes { get; set; }

        public EventCodeFilter(){}
        public EventCodeFilter(IEnumerable<long> codes)
        {
            ValidCodes = new List<long>(codes);
        }
        public EntryWrittenEventArgs Filter(EntryWrittenEventArgs e)
        {
            if (ValidCodes.Contains(e.Entry.InstanceId))
                return e;
            return null;
        }
    }
}
