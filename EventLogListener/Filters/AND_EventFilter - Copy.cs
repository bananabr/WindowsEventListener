using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EventLogListener.Filters
{
    public class AND_EventFilter : IEventFilter
    {
        IEventFilter _a;
        IEventFilter _b;

        public AND_EventFilter(IEventFilter a, IEventFilter b)
        {
            _a = a;
            _b = b;
        }
        public EntryWrittenEventArgs Filter(EntryWrittenEventArgs e)
        {
            if (_a.Filter(e) == e && _b.Filter(e) == e)
            {
                return e;
            }
            return null;
        }
    }
}
