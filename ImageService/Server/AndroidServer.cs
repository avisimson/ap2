using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ImageService.Logging;
using Communication.Modal;
using Communication;

namespace ImageService.Server
{
    class AndroidServer
    {
        private int port;
        private ILoggingService logger;
        private Boolean isStopped;
        private TcpListener tcpListener;
        private string path;

        public AndroidServer(ILoggingService logger, int port)
        {
            this.logger = logger;
            this.port = port;
            this.isStopped = false;
            this.Start();
            string[] handlers = ConfigurationManager.AppSettings["Handler"].Split(';');
            this.path = handlers[0];


        }
        public void Start()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            tcpListener = new TcpListener(ep);
            tcpListener.Start();
            logger.Log("Waiting for connections...", MessageTypeEnum.INFO);

            Task task = new Task(() =>
            {
                while (!isStopped)
                {
                    try
                    {
                        TcpClient client = tcpListener.AcceptTcpClient();
                        logger.Log("Client Connected", MessageTypeEnum.INFO);
                        try
                        {
                            while (true)
                            {
                                try
                                {
                                    NetworkStream stream = client.GetStream();
                                    byte[] bytes = new byte[4096];

                                    //gets the size of the picture.
                                    int bytesRead = stream.Read(bytes, 0, bytes.Length);
                                    string picSize = Encoding.ASCII.GetString(bytes, 0, bytesRead);

                                    if (picSize == "End\n") { break; }
                                    bytes = new byte[int.Parse(picSize)];

                                    //gets the name of the picture.
                                    bytesRead = stream.Read(bytes, 0, bytes.Length);
                                    string picName = Encoding.ASCII.GetString(bytes, 0, bytesRead);

                                    //gets the image.
                                    int bytesReadFirst = stream.Read(bytes, 0, bytes.Length);
                                    int tempBytes = bytesReadFirst;
                                    byte[] bytesCurrent;
                                    while (tempBytes < bytes.Length)
                                    {
                                        bytesCurrent = new byte[int.Parse(picSize)];
                                        bytesRead = stream.Read(bytesCurrent, 0, bytesCurrent.Length);
                                        TransferBytes(bytes, bytesCurrent, tempBytes);
                                        tempBytes += bytesRead;
                                    }

                                    //converts to an image file.
                                    ByteArrayToImage(bytes, picName);

                                }
                                catch (Exception e)
                                {
                                    this.logger.Log(e.Message, MessageTypeEnum.FAIL);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            this.logger.Log(e.Message, MessageTypeEnum.FAIL);
                        }
                    }
                    catch (SocketException e)
                    {
                        logger.Log(e.Message, MessageTypeEnum.FAIL);
                    }
                }
                logger.Log("Server Stopped", MessageTypeEnum.INFO);
            });
            task.Start();
        }

        public void ByteArrayToImage(byte[] byteArray, string picName)
        {
            using (var ms = new MemoryStream(byteArray))
            {
                //write all bytes to image.
                File.WriteAllBytes(path + "\\" + picName, byteArray);
            }

        }

        public void TransferBytes(byte[] source, byte[] forCopy, int start)
        {
            for (int i = start; i < source.Length; i++)
            {
                //send the byte from copy to source
                source[i] = forCopy[i - start];
            }
        }

    }
}