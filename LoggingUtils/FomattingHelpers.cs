using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoggingUtils
{
    public class FomattingHelpers
    {
        public static string FormatExceptionMessage(Exception exception, string fmt, object[] vars)
        {
            // Simple exception formatting: for a more comprehensive version see 
            // http://code.msdn.microsoft.com/windowsazure/Fix-It-app-for-Building-cdd80df4
            var sb = new StringBuilder();
            sb.Append(string.Format(fmt, vars));
            sb.Append(" Exception details: ");
            sb.Append(exception.ToString());
            return sb.ToString();
        }
    }
}
