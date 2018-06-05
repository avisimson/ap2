
using Communication.Event;
using System;


namespace Communication.Connection
{
    public interface IClientConnection
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