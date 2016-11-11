using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EventLogListener.Filters
{
    public class ReplacementStringFilter : IEventFilter
    {
        public  Dictionary<int,string> ValidStrings { get; set; }

        public ReplacementStringFilter(){}
        public ReplacementStringFilter(Dictionary<int, string> strings)
        {
            ValidStrings = strings;
        }
        public EntryWrittenEventArgs Filter(EntryWrittenEventArgs e)
        {
            foreach (var str in ValidStrings)
            {
                try
                {
                    if(e.Entry.ReplacementStrings[str.Key] != str.Value)
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return e;
        }
    }
}
