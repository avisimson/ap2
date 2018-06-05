using Communication.Connection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Communication.Event;
using Communication.Enums;

namespace WEB.Models
{
    public class ConfigModel
    {
        private IClientConnection client;
        private List<string> handlers;
        private bool requested;
        
        public ConfigModel()
        {
            client = ClientConnection.Instance;
            handlers = new List<string>();
            this.client.DataReceived += NotifyChange;
            //don't get request to send config
            this.requested = false;
        }

        public void SendConfigRequest()
        {
            if (!requested)
            {
                CommandReceivedEventArgs request = new CommandReceivedEventArgs((int)CommandEnum.GetConfigCommand, null, null);
                this.client.Initialize(request);
                requested = true;
            }
        }

        public List<string> Handlers
        {
            get
            {
                return this.handlers;
            }
        }
        public void RemoveHandler(string handlerToRemove)
        {
            try
            {
                string[] args = { handlerToRemove };
                CommandReceivedEventArgs eventArgs = new CommandReceivedEventArgs((int)CommandEnum.RemoveHandlerCommand, args, null);
                client.Write(eventArgs);
                client.Read();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void NotifyChange(object sender, CommandReceivedEventArgs message)
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
                            this.handlers.Add(item);
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
                          this.handlers.Remove(message.Args[0]); 
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