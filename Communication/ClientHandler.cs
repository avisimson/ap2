using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    class ClientHandler : IClientHandler
    {
        public void HandleClient(TcpClient client)
        {
            new Task(() =>
            {
                while (client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    StreamReader reader = new StreamReader(stream);
                    StreamWriter writer = new StreamWriter(stream);

                    string commandLine = reader.ReadLine();
                    Console.WriteLine("Got command: {0}", commandLine);
                    string result = ExecuteCommand(commandLine, client);
                    writer.Write(result);
                }
            }).Start();
        }
    }
}