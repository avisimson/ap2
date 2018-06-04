﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Communication.Modal;

namespace WEB.Models
{
    public class LogsModel
    {
        private IImageServiceClient client;

        public LogsModel()
        {
            client = ImageServiceClient.Instance;
        }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Log Enteries")]
        public List<MessageReceivedEventArgs> LogEntries { get; set; }
    }
}