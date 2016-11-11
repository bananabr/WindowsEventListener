using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EventLogListener.Filters.Strategies
{
    public class AnyMatch : IEventFilterStrategy
    {
        public EntryWrittenEventArgs Filter(IEnumerable<IEventFilter> filters, EntryWrittenEventArgs e)
        {
            foreach (var filter in filters)
            {
                if (filter.Filter(e) == e)
                {
                    return e;
                }
            }
            return null;
        }
    }
}
