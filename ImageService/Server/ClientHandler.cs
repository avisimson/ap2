using Communication.Enums;
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
using ImageService.Logging;
namespace ImageService.Server
{
    public class ClientHandler : IClientHandler
    {
        IImageController controller; //controller of all commands in ImageService solution.
        ILoggingService logger;
        public Mutex GlobMutex { get; set; }

        /*
         * constructor for client handler.
         * param name=imageController, the controller of all commands in image service solution.
         */
        public ClientHandler(IImageController imageController, ILoggingService logger1)
        {
            GlobMutex = new Mutex();
            this.controller = imageController;
            this.logger = logger1;
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
                    logger.Log("GUI CLIENT HAS CONNECTED " + client.ToString(), Communication.Modal.MessageTypeEnum.INFO);
                    BinaryReader reader = new BinaryReader(stream);
                    BinaryWriter writer = new BinaryWriter(stream);
                    while (true) //get commands from client until client is no longer connected.
                    {
                        string desrializedCommands = reader.ReadString();
                        CommandReceivedEventArgs commandRecievedEventArgs = JsonConvert.DeserializeObject<CommandReceivedEventArgs>(desrializedCommands);
                        if (commandRecievedEventArgs.CommandID == (int)CommandEnum.CloseGUI)
                        {//if close gui is pressed then client handling is over.
                            clients.Remove(client);
                            client.Close();
                            break;
                        }
                        bool resultCommand;
                        string commandAnswer = this.controller.ExecuteCommand((int)commandRecievedEventArgs.CommandID,
                            commandRecievedEventArgs.Args, out resultCommand);
                        if(commandRecievedEventArgs.CommandID == (int)CommandEnum.RemoveHandlerCommand)
                        {
                            foreach(TcpClient client1 in clients)
                            {
                                NetworkStream stream1 = client1.GetStream();
                                GlobMutex.WaitOne();
                                BinaryWriter writer1 = new BinaryWriter(stream1);
                                writer1.Write(commandAnswer);
                                GlobMutex.ReleaseMutex();
                            }
                        } else
                        {
                            GlobMutex.WaitOne();
                            writer.Write(commandAnswer);
                            GlobMutex.ReleaseMutex();
                        }
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