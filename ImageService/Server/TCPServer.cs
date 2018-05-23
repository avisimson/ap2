using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Communication;
using Communication.Modal;
using System.IO;
using Communication.Event;
using Communication.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageService.Server
{
    public class TCPServer : IServer
    {
        private int port; //port of connection in cp.
        private TcpListener listener;
        private IClientHandler clientHandler;
        private List<TcpClient> clientsList; //list of all connected clients.
        /*constructor for server communication.
         * param name=port1, the port we listen to.
         * param name=clientHandler, variable of the class that handles all gui service.
        */
        public TCPServer(int port1, IClientHandler clientHandler1)
        {
            this.port = port1;
            this.clientHandler = clientHandler1;
            this.clientsList = new List<TcpClient>();
            Start();
        }
        /*
         * 
         */
        public void Start()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port); //new communication channel for app.
            listener = new TcpListener(ep);
            //start listen to new GUI Clients.
            //new client has entered. opening new thread to handle it.
            Task task = new Task(() => {
                listener.Start();
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        Console.WriteLine("Got new connection");
                        this.clientsList.Add(client); //add newest client to clients list.
                        clientHandler.HandleClient(client, clientsList); //handling all new clients requests and commands.
                    }
                    catch (SocketException)
                    { //couldnt accept new client-kill thread.
                        break;
                    }
                }
                Console.WriteLine("Server stopped");
            });
            task.Start();
        }
        public void sendUpdatedLog(Object sender, MessageReceivedEventArgs e)
        {
            foreach(TcpClient client in clientsList)
            {
                NetworkStream stream = client.GetStream();
                BinaryReader reader = new BinaryReader(stream);
                BinaryWriter writer = new BinaryWriter(stream);
                string newLogMsg = JsonConvert.SerializeObject(e);
                string[] arr = new string[1];
                arr[0] = newLogMsg;
                CommandReceivedEventArgs command = new CommandReceivedEventArgs((int)CommandEnum.LogCommand, arr,null);
                string jSon = JsonConvert.SerializeObject(command);
                writer.Write(jSon);
            }
        }
        //stop function makes server stop listening to new clients to enter the service.
        public void Stop()
        {
            listener.Stop();
        }
    }
}