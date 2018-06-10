using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Communication.Enums;
using Communication.Event;
using Communication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Communication.Connection
{
    public class WebClient : IClientConnection
    {
        public event EventHandler<CommandReceivedEventArgs> DataReceived;
        private static WebClient clientInstance;
        private TcpClient client;
        private IPEndPoint ep;
        private static Mutex m_mutex = new Mutex();

        NetworkStream stream;
        private bool isConnected;

        private WebClient()
        { //construction that can be called only from this class.
            this.isConnected = this.Connect();
            CommandReceivedEventArgs request = new CommandReceivedEventArgs((int)CommandEnum.GetConfigCommand, null, null);
        }
        /*
         * param name=request- the command from client to the service.
         * Function send command to service after first connection,
         * and gets return message and invoke its listeners.
         */
        public void Initialize(CommandReceivedEventArgs request)
        {
            try
            {
                {
                    this.Write(request);
                    stream = client.GetStream();
                    BinaryReader reader = new BinaryReader(stream);
                    string jSonString = reader.ReadString();
                    CommandReceivedEventArgs msg = JsonConvert.DeserializeObject<CommandReceivedEventArgs>(jSonString);
                    this.DataReceived?.Invoke(this, msg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //singelton constructor implementation for client gui.
        public static WebClient Instance
        {
            //singleton implementation
            get
            {
                if (clientInstance == null)
                {
                    clientInstance = new WebClient();
                }
                return clientInstance;
            }
        }

        //IsConnected- boolean variable of the class that says if client is connected to service or not.
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
                CommandReceivedEventArgs eventArgs = new CommandReceivedEventArgs((int)CommandEnum.CloseGUI, null, null);
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
            try
            {
                stream = client.GetStream();
                BinaryReader reader = new BinaryReader(stream);
                string jSonString = reader.ReadString();
                CommandReceivedEventArgs msg = JsonConvert.DeserializeObject<CommandReceivedEventArgs>(jSonString);
                this.DataReceived?.Invoke(this, msg);
            }
            catch (Exception e)
            {//case of read failure or parse json failure.
                Console.WriteLine(e.Message);
            }
        }
        /* function writes commands to server.
         * param name=e, the command to be sent to service from GUI Client.
         */
        public void Write(CommandReceivedEventArgs e)
        {
            //create new thread to write to server.
            try
            {
                stream = client.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);
                string toSend = JsonConvert.SerializeObject(e);
                m_mutex.WaitOne();
                writer.Write(toSend);
                m_mutex.ReleaseMutex();
            }
            catch (Exception exception)
            {//writing to server failure or serialize failure.
                Console.WriteLine(exception.Message);
            }
        }
    }
}