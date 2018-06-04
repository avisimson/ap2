using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Communication.Modal;
using WEB.Communication;
using Communication.Event;
using Communication.Enums;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace WEB.Models
{
    public class LogsModel
    {
        private IImageServiceClient client;
        public string Filter { get; set; }

        public LogsModel()
        {
            client = ImageServiceClient.Instance;
            //add the data received that get from logs.
            this.client.DataReceived += OnDataReceived;
            CommandReceivedEventArgs request = new CommandReceivedEventArgs((int)CommandEnum.LogCommand, null, null);
            //again read from the server          
            this.client.Initialize(request);
            this.client.Read();
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
        //need to implement function that send log list according to type log
     /*   public List<Log> LogsList
        {
            get
            {
                List<Log> filteredList = new List<Log>();
                foreach (Log log in logs)
                {
                    if (String.IsNullOrEmpty(this.Filter) || this.Filter.Equals(log.GetStatus))
                        filteredList.Add(log);
                }
                return filteredList;
            }
        }*/

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