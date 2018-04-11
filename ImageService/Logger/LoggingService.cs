
using ImageService.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageService.Logging
{
    public class LoggingService : ILoggingService
    {
        // an event that handles messages that are being recieved to the logger.
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        /*the function recieves a message and invoke the logger recieving mechanism (to write down the message).
         * param name = message is a message to logger
         * param name = type is a type of message
         */
        public void Log(string message, MessageTypeEnum type)
        {
            MessageRecievedEventArgs msg = new MessageRecievedEventArgs();
            msg.Message = message;
            msg.Status = type;
            MessageRecieved?.Invoke(this, msg);
        }
    }
}
