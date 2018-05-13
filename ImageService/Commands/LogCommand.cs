using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Modal;


namespace ImageService.Commands
{
    class LogCommand : ICommand
    {
        ILoggingService log;

        public LogCommand()
        {

        }

        public string Execute(string[] args, out bool result)
        {
            LogHistory logHistory = LogHistory.CreateLogHistory();
            result = true;
            List<string[]> logList = logHistory.Logs;
            string[] answer = new string[logList.Count];
            int i = 0;
            foreach (string[] log in logList.ToArray())
            {
                answer[i] = log[0] + "," + log[1];
                i++;
            }
            // return the info converted to Json ready to be sent to client
            InfoEventArgs info = new InfoEventArgs((int)CommandEnum.LogCommand, answer);
            return JsonConvert.SerializeObject(info);
        }
    }
}
