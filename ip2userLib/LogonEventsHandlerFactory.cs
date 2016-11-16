using EventLogListener;
using EventLogListener.Filters;
using EventLogListener.Filters.Strategies;
using System.Collections.Generic;

namespace WindowsEventLogger
{
    public class NetworkLogonEventsHandlerFactory
    {
        public static EventLogHandler Build(int os_version, string domain_name = @".*")
        {
            long[] valid_logon_events;
            int domain_index;
            EventLogHandler handler = new EventLogHandler();
            if (os_version > 5)
            {
                valid_logon_events = new long[1] { 4624 };
                domain_index = 6;
            }
            else
            {
                valid_logon_events = new long[1] { 540 };
                domain_index = 1;
            }
            IEventFilter idFilter = new EventCodeFilter(valid_logon_events);
            handler.RegisterFilter(idFilter);
            IEventFilter domainFilter = new ReplacementStringFilter(new Dictionary<int, string>() { { domain_index, domain_name } });
            handler.RegisterFilter(domainFilter);
            handler.SetFilterStrategy(new AllMustMatch());
            return handler;
        }
    }
}
