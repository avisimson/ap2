using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Communication.Modal;
using Communication.Connection;
using Communication.Event;
using Communication.Enums;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace WEB.Models
{
    public class LogsModel
    {
        private IClientConnection client;
        //constructor.
        public LogsModel()
        {
            client = ClientConnection.Instance;
            client.DataReceived += OnDataRecieved;
            SendLogRequest();
        }
        /*
         * send request to read logs from server and continue reading all time while connected.
         */
        public void SendLogRequest()
        {
            CommandReceivedEventArgs request = new CommandReceivedEventArgs((int)CommandEnum.LogCommand, null, null);
            this.client.Initialize(request);
            this.client.Read();
        }
        /*
         * this function is activated when a data is recieved to client.
         * param name = sender, the object that sent the msg.
         * param name = message, the data message being read.
         */
        public void OnDataRecieved(object sender, CommandReceivedEventArgs message)
        {
            if (message.CommandID.Equals((int)CommandEnum.LogCommand))
            {
                try
                {
                    string args = message.Args[0];
                    if (LogEntries != null)
                    {//change in existing log.
                        MessageReceivedEventArgs msg = JsonConvert.DeserializeObject<MessageReceivedEventArgs>(args);
                        LogEntries.Add(msg);
                    }
                    else
                    {//the first time.
                        LogEntries = JsonConvert.DeserializeObject<ObservableCollection<MessageReceivedEventArgs>>(args);
                    }
                }
                catch (Exception e)
                {//case of failure.
                    Console.WriteLine(e.Message);
                }
            }
        }
        //the fields for the web.
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Log Enteries")]
        public ObservableCollection<MessageReceivedEventArgs> LogEntries{ get; set; }
    }
}