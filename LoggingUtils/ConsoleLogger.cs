using System;

namespace LoggingUtils
{
    public class ConsoleLogger : ILogger
    {
        public void Log(LogLevels level, string message)
        {
            switch (level)
            {
                case LogLevels.Information:
                    Console.WriteLine(String.Format(@"{0},INFO:{1}",DateTime.UtcNow.ToString("o"), message));
                    break;
                case LogLevels.Warning:
                    Console.WriteLine(String.Format(@"{0},WARN:{1}", DateTime.UtcNow.ToString("o"), message));
                    break;
                case LogLevels.Error:
                    Console.WriteLine(String.Format(@"{0},ERR:{1}", DateTime.UtcNow.ToString("o"), message));
                    break;
                default:
                    break;
            }
        }
    }
}
