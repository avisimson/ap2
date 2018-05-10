using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ImageService.Commands
{
    class LogCommand : ICommand
    {
        private ILoggingService _loggingService;
        public LogCommand(ILoggingService loggingService)
        {
            this._loggingService = loggingService;
        }
        public string Execute(string[] args, out bool result)
        {
            //מימשתי את זה וזה לא היה טוב אז אממש את זה שוב אחרי העבודה.
            //לזכור שצריך להשתמש באובזרבבל קולקשן שהוספתי בLOGGINGSERVICE
            result = true;
            return "";
        }

    }
}