using WEB.Communication;
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
        private IImageServiceClient client;
        private List<string> handlers;
        
        public ConfigModel()
        {
            client = ImageServiceClient.Instance;
            handlers = new List<string>();
            this.client.DataReceived += NotifyChange;
            CommandReceivedEventArgs request = new CommandReceivedEventArgs((int)CommandEnum.GetConfigCommand, null, null);
            this.client.Initialize(request);
        }

        public List<string> Handlers
        {
            get
            {
                return this.handlers;
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
        }

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