using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EventLogListener.Filters
{
    public class ReplacementStringFilter : IEventFilter
    {
        public  Dictionary<int,Regex> _validStrings { get; set; }

        public ReplacementStringFilter(Dictionary<int, string> strings, bool ignore_case=true)
        {
            RegexOptions ignoreCase = ignore_case ? RegexOptions.IgnoreCase : RegexOptions.None;
            _validStrings = new Dictionary<int, Regex>();
            foreach (var item in strings)
            {
                _validStrings.Add(item.Key, new Regex(item.Value, ignoreCase));
            }
        }

        public EntryWrittenEventArgs Filter(EntryWrittenEventArgs e)
        {
            foreach (var str in _validStrings)
            {
                if(!str.Value.IsMatch(e.Entry.ReplacementStrings[str.Key]))
                {
                    return null;
                }
            }
            return e;
        }
    }
}
