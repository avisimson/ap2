using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Communication;
using Communication.Enums;
using Communication.Connection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Communication.Event;
using Communication.Modal;
using System.Windows;
using System.Windows.Threading;
using System.Threading;

namespace GUI.Model
{
    class SettingsModel : ISettingsModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<string> handlers;
        private string outputDirectory;
        private string sourceName;
        private string logName;
        private int thumbnailSize;
        private string selectedHandler;
        //Initializes a new instance of settings model
        public SettingsModel()
        {
            handlers = new ObservableCollection<string>();
            this.Connection.DataReceived += OnDataReceived;
            CommandReceivedEventArgs request = new CommandReceivedEventArgs((int)CommandEnum.GetConfigCommand, null, null);
            this.Connection.Initialize(request);
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
        /// <summary>
        /// Gets or sets the handlers.
        /// </summary>
        /// <value>
        /// The handlers.
        /// </value>
        public ObservableCollection<string> Handlers
        {
            get
            {
                return this.handlers;
            }
            set
            {
                this.handlers = value;
                this.NotifyPropertyChanged("Handlers");
            }
        }

        /// <summary>
        /// Called when [data received].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        public void OnDataReceived(object sender, CommandReceivedEventArgs message)
        {
            if (message.CommandID.Equals((int)CommandEnum.GetConfigCommand))
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        
                        this.OutputDirectory = message.Args[1];
                        this.SourceName = message.Args[2];
                        this.LogName = message.Args[3];
                        this.ThumbnailSize = Convert.ToInt32(message.Args[4]);
                        string[] array = message.Args[0].Split(';');
                        foreach (var item in array)
                        {
                            this.handlers.Add(item);
                        }

                    }));

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            if (message.CommandID.Equals((int)CommandEnum.RemoveHandlerCommand))
            {
                try
                {
                    if (message.Args[1].Equals("closed")) //validation for closing the handler.
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            this.handlers.Remove(message.Args[0]);
                        }));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }
        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        /// <value>
        /// The output directory.
        /// </value>
        public string OutputDirectory
        {
            set
            {
                this.outputDirectory = value;
                this.NotifyPropertyChanged("OutputDirectory");
            }
            get
            {
                return this.outputDirectory;
            }
        }
        /// <summary>
        /// Gets or sets the name of the source.
        /// </summary>
        /// <value>
        /// The name of the source.
        /// </value>
        public string SourceName
        {
            set
            {
                this.sourceName = value;
                this.NotifyPropertyChanged("SourceName");
            }
            get
            {
                return this.sourceName;
            }
        }
        /// <summary>
        /// Gets or sets the name of the log.
        /// </summary>
        /// <value>
        /// The name of the log.
        /// </value>
        public string LogName
        {
            set
            {
                this.logName = value;
                this.NotifyPropertyChanged("LogName");
            }
            get
            {
                return this.logName;
            }
        }
        /// <summary>
        /// Gets or sets the size of the thumbnail.
        /// </summary>
        /// <value>
        /// The size of the thumbnail.
        /// </value>
        public int ThumbnailSize
        {
            set
            {
                this.thumbnailSize = value;
                this.NotifyPropertyChanged("ThumbnailSize");
            }
            get
            {
                return this.thumbnailSize;
            }
        }
        /// <summary>
        /// Gets or sets the selected handler.
        /// </summary>
        /// <value>
        /// The selected handler.
        /// </value>
        public string SelectedHandler
        {
            get
            {
                return this.selectedHandler;
            }
            set
            {
                selectedHandler = value;
                this.NotifyPropertyChanged("SelectedHandler");
            }
        }
    }
}