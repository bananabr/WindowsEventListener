using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventLogListener.Loggers
{
    public interface ILogable
    {
        void RegisterLogger(IEventLogger logger);
    }
}
