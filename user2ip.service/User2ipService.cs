using EventLogListener;
using EventLogListener.Filters;
using EventLogListener.Loggers;
using LoggingUtils;
using System;
using System.Collections.Generic;
using System.Security;
using System.ServiceProcess;
using System.Threading;
using WindowsEventLogger;

namespace user2ip.service
{
    public partial class User2ipService : ServiceBase
    {
        ManualResetEvent _quitEvent = new ManualResetEvent(false);
        string redisConnectionString, domain;
        int redisTTL;
        ILogger logger = new WindowsEventLogLogger(@"User2ipService");
        EventLogHandler handler;
        int remote_network_address_index;
        int username_index;

        void Init()
        {
            redisConnectionString = Properties.Settings.Default["RedisServers"].ToString();
            domain = Properties.Settings.Default["WindowsDomainRegex"].ToString();
            int.TryParse(Properties.Settings.Default["RedisTTL"].ToString(), out redisTTL);
            var os_version = Environment.OSVersion.Version;
            handler = NetworkLogonEventsHandlerFactory.Build(os_version.Major, domain);
            IEventLogger consoleLogger = new ConsoleReplacementStringsLogger();
            if (os_version.Major > 5)
            {
                remote_network_address_index = 18;
                username_index = 5;
            }
            else
            {
                remote_network_address_index = 13;
                username_index = 0;
            }
            IEventLogger redisLogger = new RedisEventLogger(redisConnectionString, remote_network_address_index, username_index, false, redisTTL);
            IEventFilter usernameFilter = new NOT_EventFilter(new ReplacementStringFilter(new Dictionary<int, string>() { { username_index, @"^.*\$$" } }));
            handler.RegisterFilter(usernameFilter);
            handler.RegisterLogger(consoleLogger);
            handler.RegisterLogger(redisLogger);
            handler.SetExceptionLogger(logger);
        }

        public User2ipService()
        {
            InitializeComponent();
        }


        protected override void OnStart(string[] args)
        {
            try
            {
                logger.Log(LogLevels.Information, String.Format(@"Identified current OS version as {0}.", Environment.OSVersion.VersionString));
                Init();
                logger.Log(LogLevels.Information, @"Finished loading configuration.");
                WindowsEventLogListener listener = new WindowsEventLogListener("Security", handler);
                logger.Log(LogLevels.Information, @"Started to listen for events ...");
            }
            catch (SecurityException)
            {
                logger.Log(LogLevels.Error, @"Permission denied when trying to access the Security log.");
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                logger.Log(LogLevels.Error, String.Format("Something bad hapenned, do something about it!\nException details:\n{0}", e.Message));
                Environment.Exit(1);
            }
        }

        protected override void OnStop()
        {
            logger.Log(LogLevels.Information, @"User2ip was stopped.");
        }
    }
}
