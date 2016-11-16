using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EventLogListener.Filters
{
    public class NOT_EventFilter : IEventFilter
    {
        IEventFilter _a;

        public NOT_EventFilter(IEventFilter filter)
        {
            _a = filter;
        }
        public EntryWrittenEventArgs Filter(EntryWrittenEventArgs e)
        {
            if (_a.Filter(e) == e)
            {
                return null;
            }
            return e;
        }
    }
}
