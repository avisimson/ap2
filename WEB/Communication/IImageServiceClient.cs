﻿using Communication;
using Communication.Event;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEB.Communication
{
    public interface IImageServiceClient
    {
        bool Connect(); //connect to server.
        void Disconnect(); //disconnect.
        void Write(CommandReceivedEventArgs e); //write to service a command
        void Read(); //read back from service
        event EventHandler<CommandReceivedEventArgs> DataReceived;
        bool IsConnected { get; set; } //true while the connection to service is on.
        void Initialize(CommandReceivedEventArgs request); //first command to service.
    }
}