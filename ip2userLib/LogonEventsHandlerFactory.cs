using EventLogListener;
using EventLogListener.Filters;
using EventLogListener.Filters.Strategies;
using ip2userLib;
using System.Collections.Generic;

namespace WindowsEventLogger
{
    public class NetworkLogonEventsHandlerFactory
    {
        public static EventLogHandler Build(int os_version, string domain_name = @".*", NetworkLogonEventSources source = NetworkLogonEventSources.LOGON)
        {
            long[] valid_logon_events;
            int domain_index;
            EventLogHandler handler = new EventLogHandler();
            IEventFilter idFilter, domainFilter;

            switch (source)
            {
                case NetworkLogonEventSources.LOGON:
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
                    break;
                case NetworkLogonEventSources.KERBEROS:
                    if (os_version > 5)
                    {
                        throw new System.Exception(@"Unsupported OS version for Kerberos source");
                    }
                    else
                    {
                        valid_logon_events = new long[2] { 672, 673 };
                        domain_index = 1;
                    }
                    break;
                default:
                    throw new System.Exception(@"Unknown source provided");
            }

            idFilter = new EventCodeFilter(valid_logon_events);
            domainFilter = new ReplacementStringFilter(new Dictionary<int, string>() { { domain_index, domain_name } });
            handler.RegisterFilter(idFilter);
            handler.RegisterFilter(domainFilter);
            handler.RegisterFilter(new ReplacementStringFilter(new Dictionary<int, string>() { { 6, @"-" } }, 672));
            handler.RegisterFilter(new ReplacementStringFilter(new Dictionary<int, string>() { { 7, @"-" } }, 673));
            handler.SetFilterStrategy(new AllMustMatch());
            return handler;
        }
    }
}
