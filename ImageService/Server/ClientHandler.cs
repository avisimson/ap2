﻿using Communication.Enums;
using Communication.Event;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ImageService.Controller;
using Communication;
namespace ImageService.Server
{
    public class ClientHandler : IClientHandler
    {
        IImageController controller; //controller of all commands in ImageService solution.
        public static Mutex GlobMutex { get; set; }

        /*
         * constructor for client handler.
         * param name=imageController, the controller of all commands in image service solution.
         */
        public ClientHandler(IImageController imageController)
        {
            this.controller = imageController;
        }

        /*
         * function handles GUI Client and his commands until he disconnects. after disconnection we remove it from clients list.
         * param name=client, the current tcp client being handled.
         * param name=clients, the list of all current connected clients.
         */
        public void HandleClient(TcpClient client, List<TcpClient> clients)
        {
            //new thread to handle client.
            new Task(() =>
            {
                try
                {
                    //stream of connection between service and GUI Client.
                    NetworkStream stream = client.GetStream();
                    BinaryReader reader = new BinaryReader(stream);
                    BinaryWriter writer = new BinaryWriter(stream);
                    while (true) //get commands from client until client is no longer connected.
                    {
                        string desrializedCommands = reader.ReadString();
                        CommandRecievedEventArgs commandRecievedEventArgs = JsonConvert.DeserializeObject<CommandRecievedEventArgs>(desrializedCommands);
                        if (commandRecievedEventArgs.CommandID == (int)CommandEnum.CloseGUI)
                        {//if close gui is pressed then client handling is over.
                            clients.Remove(client);
                            client.Close();
                            break;
                        }
                        bool resultCommand;
                        string commandAnswer = this.controller.ExecuteCommand((int)commandRecievedEventArgs.CommandID,
                            commandRecievedEventArgs.Args, out resultCommand);
                        GlobMutex.WaitOne();
                        writer.Write(commandAnswer);
                        GlobMutex.ReleaseMutex();
                    }
                }
                catch
                {//if failure occurrs.
                    clients.Remove(client);
                    client.Close();
                }
            }).Start();
        }
    }
}