using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using ImageService.Logging;
using ImageService.Server;
using ImageService.Logging.Modal;
using ImageService.Controller;
using ImageService.Modal;
//
namespace ImageService
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };
    public partial class ImageService : ServiceBase
    {
        
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
        private System.ComponentModel.IContainer components;
        private System.Diagnostics.EventLog eventLog1;
        private int eventId = 1;
        private ILoggingService logger;
        private ImageServer server;
        /*
         * param name = args - user can enter names for the program.
         * constructor initializes logger and server.
         */

        public ImageService(string[] args)
        {
            InitializeComponent();
            string eventSourceName = "Advanced Programming project";
            string logName = "MyNewLog";
            if (args.Count() > 0)
            {
                eventSourceName = args[0];
            }
            if (args.Count() > 1)
            {
                logName = args[1];
            }
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(eventSourceName, logName);
            }
            eventLog1.Source = eventSourceName;
            eventLog1.Log = logName;
            //create the server.
            this.logger = new LoggingService();
            this.server = new ImageServer((LoggingService)logger);
            //add messages to legger.
            this.logger.MessageRecieved += this.WriteMsg;
        }
        /*
         * param name = sender - the object that called method.
         * param name = args -the timer.
         * function handles timer.
         */
        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.  
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
        }
        /*
         * start the events program in the event manager.
         * handles messages to user.
         */
        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("In On  Start");
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 60 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }
        /*
         * send close to user and close server and listen to directories.
         */
        protected override void OnStop()
        {
            eventLog1.WriteEntry("In onStop, closing server.");
            //tell server to stop(the server will send to everyone else).
            server.CloseServer();
        }
        /*
         * inform user on continue.
         */
        protected override void OnContinue()
        {
            eventLog1.WriteEntry("In OnContinue.");
        }
        /*
         * param name = sender - the object that called the method.
         * param name = e - the message
         * function writes message
         */
        public void WriteMsg(Object sender, MessageRecievedEventArgs e)
        {
            eventLog1.WriteEntry(e.message, GetType(e.status));
        }
        /*
         * param name = type -the type of message.
         * returns - type of message to user.
         */
        private EventLogEntryType GetType(MessageTypeEnum type)
        {
            switch (type)
            {
                case MessageTypeEnum.WARNING:
                    return EventLogEntryType.Warning;
                case MessageTypeEnum.FAIL:
                    return EventLogEntryType.Error;
                case MessageTypeEnum.INFO:
                    return EventLogEntryType.Information;
                default:
                    return EventLogEntryType.Information;
            }
        }
    }
}
