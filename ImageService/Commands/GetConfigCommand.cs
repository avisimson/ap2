using Communication.Enums;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Communication.Event;

namespace ImageService.Commands
{
    public class GetConfigCommand : ICommand
    {
        private int sizeOfArray = 5;
        /*
       * function that execute the command accodring to "ImageServiceModel"
         * param name =  args is the args to the command (the getconfig).
         * param name = result is a out bool var gets true if command executed successfuly and false otherwise.
         * return the the JsonConvert of config if successful or exception if failed.
         */
        public string Execute(string[] args, out bool result)
        {
            try
            {
                result = true;
                string[] configInfo = new string[sizeOfArray];
                configInfo[0] = ConfigurationManager.AppSettings.Get("Handler");
                configInfo[1] = ConfigurationManager.AppSettings.Get("OutputDir");
                configInfo[2] = ConfigurationManager.AppSettings.Get("SourceName");
                configInfo[3] = ConfigurationManager.AppSettings.Get("LogName");
                configInfo[4] = ConfigurationManager.AppSettings.Get("ThumbnailSize");
                CommandReceivedEventArgs config = new CommandReceivedEventArgs((int)CommandEnum.GetConfigCommand, configInfo, "");
                return JsonConvert.SerializeObject(config); //serialize config to string and return it.
            }
            catch (Exception e) { //if serialization or going to AppConfig file throws exception.
                result = false;
                return e.ToString();
            }
        }
    }
}