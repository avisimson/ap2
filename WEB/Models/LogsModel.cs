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
        public string Filter { get; set; }

        public LogsModel()
        {
            client = ClientConnection.Instance;
            client.DataReceived += OnDataReceived;
        }

        public void SendLogRequest()
        {
            CommandReceivedEventArgs request = new CommandReceivedEventArgs((int)CommandEnum.LogCommand, null, null);
            this.client.Initialize(request);
        }
        public void OnDataReceived(object sender, CommandReceivedEventArgs message)
        {
            if (message.CommandID.Equals((int)CommandEnum.LogCommand))
            {
                try
                {
                   string args = message.Args[0];
                   if (LogEntries != null)
                   {
                        MessageReceivedEventArgs msg = JsonConvert.DeserializeObject<MessageReceivedEventArgs>(args);
                        LogEntries.Add(msg);
                   }
                   else
                   {//the first time.
                        LogEntries = JsonConvert.DeserializeObject<ObservableCollection<MessageReceivedEventArgs>>(args);
                   }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Log Enteries")]
        public ObservableCollection<MessageReceivedEventArgs> LogEntries
        {
            get
            {
                return this.LogEntries;
            }
            set
            {
                this.LogEntries = value;
            }
        }
    }
}