﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging.Modal
{
    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageTypeEnum status { get; set; }
        public string message { get; set; }
        /*
         *param name = type - the type of of message.
         * param name = message - the message.
         * constructor. 
         */
        public MessageRecievedEventArgs(MessageTypeEnum type, string msg)
        {
            this.status = type;
            this.message = msg;
        }
    }
}
