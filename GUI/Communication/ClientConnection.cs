using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Communication;
using Communication.Enums;
using Communication.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GUI.Communication
{
    public class ClientConnection : IClientConnection
    {
        public event EventHandler<CommandMessage> DataReceived;
        private static ClientConnection clientInstance;
        private TcpClient client;
        private IPEndPoint ep;
        NetworkStream stream;
        private bool isConnected;

        private ClientConnection()
        { //construction that can be called only from this class.
            this.isConnected = this.Connect();
        }
        //singelton constructor implementation for client gui.
        public static ClientConnection clientSingelton
        {
            get
            {
                if (clientInstance == null)
                {
                    clientInstance = new ClientConnection();
                    //clientInstance.IsConnected = Instance.Channel
                }
                return clientInstance;
            }
        }

        public bool IsConnected
        { //get and set implementation.
            get
            {
                return this.isConnected;
            }
            set
            {
                this.isConnected = value;
            }
        }
        /*
         * function connects GUI Client to its service.
         * returns-true if connection succeed/false if failed.
         */
        public bool Connect()
        {
            try
            {
                ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
                client = new TcpClient();
                client.Connect(ep);
                isConnected = true;
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        /*
         * function disconnects from service, and send closegui commands.
         * Returns-true is disconnect succeed/false if failed.
         */
        public void Disconnect()
        {
            try
            {
                CommandRecievedEventArgs eventArgs = new CommandRecievedEventArgs((int)CommandEnum.CloseGUI, null, null);
                this.Write(eventArgs);
                client.Close();
                isConnected = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /*
         * function reads string message from service and converts it back to json, then invokes all listeners.
         */
        public void Read()
        {
            //new thread.
            Task task = new Task(() =>
            {
                try
                {
                    while (this.isConnected)
                    {
                        stream = client.GetStream();
                        StreamReader reader = new StreamReader(stream);
                        string json = reader.ReadLine();
                        while (reader.Peek() > 0)
                        {//read the message
                            json += reader.ReadLine();
                        }
                        CommandMessage msg = CommandMessage.ParseJSON(json); //parse the json.
                        this.DataReceived?.Invoke(this, msg);
                    }
                }
                catch (Exception e)
                {//case of read failure or parse json failure.
                    Console.WriteLine(e.Message);
                }
            });
            task.Start();
        }
        /* function writes commands to server.
         * param name=e, the command to be sent to service from GUI Client.
         */
        public void Write(CommandRecievedEventArgs e)
        {
            //create new thread to write to server.
            Task task = new Task(() =>
            {
                try
                {
                    stream = client.GetStream();
                    StreamWriter writer = new StreamWriter(stream);
                    string msg = JsonConvert.SerializeObject(e); //serializing the command to json.
                    writer.WriteLine(msg); //writing it on writer.
                    writer.Flush(); //send the message.
                }
                catch (Exception exception)
                {//writing to server failure or serialize failure.
                    Console.WriteLine(exception.Message);
                }

            });
            task.Start();
        }
    }
}