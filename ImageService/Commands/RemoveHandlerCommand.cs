﻿using ImageService.Infrastructure.Enums;
using ImageService.Modal;
using ImageService.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    class RemoveHandlerCommand
    {
        ImageServer server;
        public RemoveHandlerCommand(ImageServer _server)
        {
            this.server = server;
        }
        /*
         *@param name = args, the path of directory.
         * @param name = result, to be initialized to true is successful excecution and false if fail.
         * return json conversion of the logs.
         */
        public string Execute(string[] args, out bool result)
        {
            try
            {
                int i;
                result = false;
                if (args == null || args.Length <= 0)
                {//path of args(to directory) is incorrect.
                    result = false;
                    throw new Exception("Bad path parameter to Remove Handler Command.");
                }
                string removeDir = args[0]; //directory set to be removed.
                string[] handlers = (ConfigurationManager.AppSettings.Get("Handler").Split(';')); //dirs we listen to now.
                StringBuilder newHandlersList = new StringBuilder();
                for (i = 0; i < handlers.Length; i++)
                {//get all handlers we listen to, to new string except the specified handler we remove.
                    if (handlers[i] != removeDir)
                    {
                        newHandlersList.Append(handlers[i] + ";");
                    }
                    else
                    {
                        result = true;
                    }
                }
                string updatedHandlers = newHandlersList.ToString().Trim().TrimEnd(';');
                //update app config file in project, change handlers, save and refresh to see the change.
                Configuration configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configFile.AppSettings.Settings.Remove("Handler");
                configFile.AppSettings.Settings.Add("Handler", updatedHandlers);
                configFile.Save(ConfigurationSaveMode.Minimal);
                ConfigurationManager.RefreshSection("appSettings");
                String[] arr = new String[1];
                CommandRecievedEventArgs commandSendArgs = new CommandRecievedEventArgs((int)CommandEnum.RemoveHandlerCommand, arr, "");
                if (this.server.CloseSpecificDir(removeDir))
                {
                    arr[0] = "closed";
                }
                else
                {
                    arr[0] = "notClosed";
                }
                return JsonConvert.SerializeObject(commandSendArgs);
            }
            catch (Exception e)
            {//excecution of removing handler failed.
                result = false;
                return e.ToString();
            }
        }
    }
}