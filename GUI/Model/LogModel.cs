using Communication;
using GUI.Communication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication.Enums;
using Communication.Event;
using Newtonsoft.Json.Linq;
using Communication.Modal;
using Newtonsoft.Json;

namespace GUI.Model
{
    class LogModel : ILogModel
    {
        private ObservableCollection<MessageRecievedEventArgs> logEntries;
        public event PropertyChangedEventHandler PropertyChanged;

        public LogModel()
        {
            this.Connection.DataReceived += OnDataReceived;
            CommandRecievedEventArgs request = new CommandRecievedEventArgs((int)CommandEnum.LogCommand, null, null);
            this.Connection.Write(request);
            this.Connection.Read();
        }

        public ObservableCollection<MessageRecievedEventArgs> LogEntries
        {
            get
            {
                return this.logEntries;
            }
            set
            {
                this.logEntries = value;
                NotifyPropertyChanged("LogEntries");
            }
        }

        public IClientConnection Connection
        {
            get
            {
                return ClientConnection.clientSingelton;
            }
        }


        private void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public void OnDataReceived(object sender, CommandMessage message)
        {
            try
            {
                if (message.CommandID.Equals((int)CommandEnum.LogCommand))
                {
                    string listOfEntries = (string)message.CommandArgs["LogEntries"];
                    ObservableCollection<MessageRecievedEventArgs> arr = JsonConvert.DeserializeObject<ObservableCollection<MessageRecievedEventArgs>>(listOfEntries);
                    this.LogEntries = arr;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}