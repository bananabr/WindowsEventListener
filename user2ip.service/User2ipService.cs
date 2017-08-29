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
        string redisConnectionString,
            domain;
        EventLogHandler logonEventsHandler,
            kerberosEventsHandler;
        int redisTTL,
            remote_network_address_index,
            remote_network_address_tgt_index,
            remote_network_address_ticket_index,
            krb_tgt_granted_ev_code,
            krb_service_ticket_granted_ev_code,
            username_index;
        bool parseKerberosEvents;

        ManualResetEvent _quitEvent = new ManualResetEvent(false);
        ILogger logger = new WindowsEventLogLogger(@"User2ipService");

        void Init()
        {
            redisConnectionString = Properties.Settings.Default["RedisServers"].ToString();
            domain = Properties.Settings.Default["WindowsDomainRegex"].ToString();
            parseKerberosEvents = Properties.Settings.Default["ParseKerberosEvents"].ToString().ToUpper() == "TRUE";
            int.TryParse(Properties.Settings.Default["RedisTTL"].ToString(), out redisTTL);
            var os_version = Environment.OSVersion.Version;

            //IEventLogger consoleLogger = new ConsoleReplacementStringsLogger();
            if (os_version.Major > 5)
            {
                if(parseKerberosEvents)
                    throw new Exception(@"Parsing kerberos events is not supported for this OS version yet");
                username_index = 5;
                remote_network_address_index = 18;
            }
            else
            {
                username_index = 0;
                remote_network_address_index = 13;
                remote_network_address_ticket_index = 6;
                remote_network_address_tgt_index = 9;
                krb_tgt_granted_ev_code = (int)EventLogListener.WindowsEventCodes.WIN2K3_KRB_TGT_GRANTED;
                krb_service_ticket_granted_ev_code = (int)EventLogListener.WindowsEventCodes.WIN2K3_KRB_SERVICE_TICKET_GRANTED;
            }
            // Exclude machine accounts
            IEventFilter usernameFilter = new NOT_EventFilter(new ReplacementStringFilter(new Dictionary<int, string>() { { username_index, @"^.*\$$" } }));

            //Account Logon Event Handler
            logonEventsHandler = NetworkLogonEventsHandlerFactory.Build(os_version.Major, domain);
            logonEventsHandler.RegisterFilter(usernameFilter);
            //logonEventsHandler.RegisterLogger(consoleLogger);
            logonEventsHandler.RegisterLogger(
                new RedisEventLogger(
                    redisConnectionString,
                    remote_network_address_index,
                    username_index,
                    0,
                    true,
                    redisTTL
                )
            );
            logonEventsHandler.SetExceptionLogger(logger);

            //Kerberos Ticket Request Event Handler
            kerberosEventsHandler = NetworkLogonEventsHandlerFactory.Build(os_version.Major, domain, ip2userLib.NetworkLogonEventSources.KERBEROS);
            kerberosEventsHandler.RegisterFilter(usernameFilter);
            //kerberosEventsHandler.RegisterLogger(consoleLogger);
            kerberosEventsHandler.RegisterLogger(
                new RedisEventLogger(
                    redisConnectionString,
                    remote_network_address_ticket_index,
                    username_index,
                    krb_service_ticket_granted_ev_code,
                    true,
                    redisTTL
                )
            );
            kerberosEventsHandler.RegisterLogger(
                new RedisEventLogger(
                    redisConnectionString,
                    remote_network_address_tgt_index,
                    username_index,
                    krb_tgt_granted_ev_code,
                    true,
                    redisTTL
                )
            );
            kerberosEventsHandler.SetExceptionLogger(logger);
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
                WindowsEventLogListener logonEventsListener = new WindowsEventLogListener("Security", logonEventsHandler);
                WindowsEventLogListener kerberosEventsListener = new WindowsEventLogListener("Security", kerberosEventsHandler);
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
