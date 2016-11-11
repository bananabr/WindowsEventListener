using EventLogListener.Filters.Strategies;

namespace EventLogListener
{
    public interface IEventFilterable
    {
        void RegisterFilter(IEventFilter filter);
        void SetFilterStrategy(IEventFilterStrategy stg);
    }
}
