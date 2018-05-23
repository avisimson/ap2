using Communication.Enums;
using ImageService.Logging;
using ImageService.Modal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication.Modal;
using Communication.Event;

namespace ImageService.Commands
{
    class LogCommand : ICommand
    {
        private ILoggingService _loggingService;
        /* constructor.
         *@param name = loggingService, the log class object.
        */
        public LogCommand(ILoggingService loggingService)
        {
            this._loggingService = loggingService;
        }
        /*
         *@param name = args, the path of directory.
         * @param name = result, to be initialized to true is successful excecution and false if fail.
         * return json conversion of the logs.
         */
        public string Execute(string[] args, out bool result)
        {
            ObservableCollection<MessageReceivedEventArgs> logs = _loggingService.getLogHistory();
            string allLogs = JsonConvert.SerializeObject(logs);
            result = true; // successful excecution of log.
            string[] logArr = new string[1];
            logArr[0] = allLogs;
            CommandReceivedEventArgs logsCommand = new CommandReceivedEventArgs((int)CommandEnum.LogCommand, logArr, "");
            return JsonConvert.SerializeObject(logsCommand);
        }
    }
}