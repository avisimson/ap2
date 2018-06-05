using Communication;
using Communication.Connection;
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
using System.Windows;

namespace GUI.Model
{
    class LogModel : ILogModel
    {
        private ObservableCollection<MessageReceivedEventArgs> logEntries;
        public event PropertyChangedEventHandler PropertyChanged;

        public LogModel()
        {
            //add the data received that get from logs.
            this.Connection.DataReceived += OnDataReceived;
            CommandReceivedEventArgs request = new CommandReceivedEventArgs((int)CommandEnum.LogCommand, null, null);
            //again read from the server          
            this.Connection.Initialize(request);
            this.Connection.Read();
        }
        /// <summary>
        /// Gets or sets the log entries.
        /// </summary>
        /// <value>
        /// The log entries.
        /// </value>
        public ObservableCollection<MessageReceivedEventArgs> LogEntries
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
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        public IClientConnection Connection
        {
            get
            {
                return ClientConnection.Instance;
            }
        }

        /// Notifies the property changed.
        private void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        /// <summary>
        /// Called when [data received].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        public void OnDataReceived(object sender, CommandReceivedEventArgs message)
        {
            if (message.CommandID.Equals((int)CommandEnum.LogCommand))
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        string args = message.Args[0];
                        if (logEntries != null)
                        {
                            MessageReceivedEventArgs msg = JsonConvert.DeserializeObject<MessageReceivedEventArgs>(args);
                            logEntries.Add(msg);
                        }
                        else
                        {//the first time.
                            logEntries = JsonConvert.DeserializeObject<ObservableCollection<MessageReceivedEventArgs>>(args);
                        }
                    }));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

    }
}