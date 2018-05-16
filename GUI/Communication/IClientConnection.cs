using Communication;
using Communication.Event;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Communication
{
    public interface IClientConnection
    {
        bool Connect(); //connect to server.
        void Disconnect(); //disconnect.
        void Write(CommandRecievedEventArgs e); //write to service a command
        void Read(); //read back from service
        event EventHandler<CommandMessage> DataReceived;
        bool IsConnected { get; set; }
    }
}