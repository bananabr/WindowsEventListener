using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EventLogListener.Filters.Strategies
{
    public interface IEventFilterStrategy
    {
        EntryWrittenEventArgs Filter(IEnumerable<IEventFilter> filters, EntryWrittenEventArgs e);
    }
}
