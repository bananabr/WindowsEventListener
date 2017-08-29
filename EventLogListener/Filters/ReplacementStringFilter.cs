using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace EventLogListener.Filters
{
    public class ReplacementStringFilter : IEventFilter
    {
        public  Dictionary<int,Regex> _validStrings { get; set; }
        private int _eventId;
        public ReplacementStringFilter(Dictionary<int, string> strings, int evt_id = 0, bool ignore_case=true)
        {
            RegexOptions ignoreCase = ignore_case ? RegexOptions.IgnoreCase : RegexOptions.None;
            _validStrings = new Dictionary<int, Regex>();
            _eventId = evt_id;
            foreach (var item in strings)
            {
                _validStrings.Add(item.Key, new Regex(item.Value, ignoreCase));
            }
        }

        public EntryWrittenEventArgs Filter(EntryWrittenEventArgs e)
        {
            foreach (var str in _validStrings)
            {
                if (_eventId <= 0 || e.Entry.InstanceId == _eventId)
                {
                    if (!str.Value.IsMatch(e.Entry.ReplacementStrings[str.Key]))
                    {
                        return null;
                    }
                }
            }
            return e;
        }
    }
}
