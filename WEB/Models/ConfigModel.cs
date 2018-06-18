using Communication.Connection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Communication.Event;
using Communication.Enums;
using System.Threading;

namespace WEB.Models
{
    public class ConfigModel
    {
        object locker = new object();
        private IClientConnection client; //the connection to server.
        private List<string> handlers; //the directorys
        private bool requested; //if config request sent or not.
        /*
         * constructor.
         */
        public ConfigModel()
        {
            client = WebClient.Instance;
            handlers = new List<string>();
            //every time that data recieved to Web Client-NotifyChange method is activated.
            this.client.DataReceived += OnDataRecieved;
            //don't get request to send config
            this.requested = false;
        }
        /*
         * function sends a request to the server to get the configuration from him.
         */
        public void SendConfigRequest()
        {
            if (!requested)
            {
                CommandReceivedEventArgs request = new CommandReceivedEventArgs((int)CommandEnum.GetConfigCommand, null, null);
                this.client.Initialize(request);
                this.client.Read();
                requested = true;
            }
        }
        /*
         * returns the list of directories.
         */
        public List<string> Handlers
        {
            get
            {
                return this.handlers;
            }
        }
        /*
         * function gets handler and send a commend to server to remove him from listening to pics,
         * then wait for an answer from server.
         * param name = handlerToRemove. the handler that client selected to be removed.
         */
        public void RemoveHandler(string handlerToRemove)
        {
            try
            {
                string[] args = { handlerToRemove };
                CommandReceivedEventArgs eventArgs = new CommandReceivedEventArgs((int)CommandEnum.RemoveHandlerCommand, args, null);
                //send command to remove to server.
                client.Write(eventArgs);
                //get the answer of removing from server and invoke all listeners.
                client.Read(); //read to skip log command.
                client.Read();
            }
            catch (Exception e)
            {//case of read/write failure.
                Console.WriteLine(e.Message);
            }
        }
        /*
         * this function is activated when a data is recieved to client.
         * param name = sender, the object that sent the msg.
         * param name = message, the data message being read.
         */
        public void OnDataRecieved(object sender, CommandReceivedEventArgs message)
        {
            if (message.CommandID.Equals((int)CommandEnum.GetConfigCommand))
            {
                try
                {
                    this.OutputDirectory = message.Args[1];
                    this.SourceName = message.Args[2];
                    this.LogName = message.Args[3];
                    this.ThumbnailSize = Convert.ToInt32(message.Args[4]);
                    string[] array = message.Args[0].Split(';');
                    foreach (var item in array)
                    {
                        this.Handlers.Add(item);
                    }

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
                        this.Handlers.Remove(message.Args[0]);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        public bool Requested
        {
            get
            {
                return this.requested;
            }
            set
            {
                this.requested = value;
            }
        }

        //All fields of configuration.
        [Required]
        [Display(Name = "Output Directory")]
        public string OutputDirectory { get; set; }


        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Source Name")]
        public string SourceName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Log Name")]
        public string LogName { get; set; }
        [Required]
        [Display(Name = "Thumbnail Size")]
        public int ThumbnailSize { get; set; }
    }
}