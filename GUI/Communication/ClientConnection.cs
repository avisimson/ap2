using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
        public event EventHandler<CommandReceivedEventArgs> DataReceived;
        private static ClientConnection clientInstance;
        private TcpClient client;
        private IPEndPoint ep;
        private static Mutex m_mutex = new Mutex();

        NetworkStream stream;
        private bool isConnected;

        private ClientConnection()
        { //construction that can be called only from this class.
            this.isConnected = this.Connect();
            CommandReceivedEventArgs request = new CommandReceivedEventArgs((int)CommandEnum.GetConfigCommand, null, null);
        }
        /// <summary>
        /// Initializes the specified request.
        /// </summary>
        public void Initialize(CommandReceivedEventArgs request)
        {
            try
            {
                {
                    this.Write(request);
                    stream = client.GetStream();
                    BinaryReader reader = new BinaryReader(stream);
                    string jSonString = reader.ReadString();
                    CommandMessage msg = CommandMessage.ParseJSON(jSonString);
                    this.DataReceived?.Invoke(this, msg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        //singelton constructor implementation for client gui.
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static ClientConnection Instance
        {
            //singleton implementation
            get
            {
                if (clientInstance == null)
                {
                    clientInstance = new ClientConnection();
                }
                return clientInstance;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
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
            //new thread.
            Task task = new Task(() =>
            {
                try
                {
                    while (this.isConnected)
                    {
                        stream = client.GetStream();
                        BinaryReader reader = new BinaryReader(stream);
                        string jSonString = reader.ReadString();
                        CommandMessage msg = CommandMessage.ParseJSON(jSonString);
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
        public void Write(CommandReceivedEventArgs e)
        {
            //create new thread to write to server.
            Task task = new Task(() =>
            {
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

            });
            task.Start();
        }
    }
}