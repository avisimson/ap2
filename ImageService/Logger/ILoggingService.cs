using Communication.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace ImageService.Logging
{
    public interface ILoggingService
    {
        event EventHandler<MessageReceivedEventArgs> MessageRecieved;
        void Log(string message, MessageTypeEnum type);           // Logging the Message
        ObservableCollection<MessageReceivedEventArgs> getLogHistory(); //getting history of all logs.
    }
}
