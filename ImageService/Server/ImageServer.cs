using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using Communication.Enums;
using Communication.Modal;
using Communication.Event;
using ImageService.Logging;
using ImageService.Modal;
using System.Configuration;
using Communication;
using System.Threading;

namespace ImageService.Server
{
    public class ImageServer
    {
        private const int port = 8000;
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private List<IDirectoryHandler> directoriesHandler;
        #endregion
        #region Properties
        public event EventHandler<CommandReceivedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion
        /*
         * param name = logger - logger of service.
         * constructor
         */
        public ImageServer(ILoggingService logger)
        {
            Thread.Sleep(1000);
            this.m_logging = logger;
            //initilaize the directoryHandlers, and create controller for command inside.
            this.directoriesHandler = new List<IDirectoryHandler>();
            //create ImageModal service for controller and create controller of commands.
            int size = Int32.Parse(ConfigurationManager.AppSettings.Get("ThumbnailSize"));
            this.m_controller =
                new ImageController(new ImageServiceModal(ConfigurationManager.AppSettings.Get("OutputDir"), size), this.m_logging);
            this.m_controller.Server = this;
            CreateDirectoryHandlers();
            //create client handler to handle all clients that connect.
            IClientHandler clientHandler = new ClientHandler(this.m_controller, this.m_logging);
            //server communication ainteractively with the running service.
            IServer server = new TCPServer(port, clientHandler);
            //server communication with android app.
            AndroidServer androidServer = new AndroidServer(logger, 8600);
            //every time that log is added, we send it to clients.
            this.m_logging.MessageRecieved += server.sendUpdatedLog;
         }
        /*
         * the function create directory handlers.
         */
        public void CreateDirectoryHandlers()
        {
            //all directories contain the path that enter in App.config
            string allDirectories = ConfigurationManager.AppSettings["Handler"];
            //seperate bettween the paths that found in line of "Handler" in App.config
            string[] paths = allDirectories.Split(';');
            //loop for listen to all the paths that found in line of "Handler" in App.config.
            foreach (string path in paths)
            {
                //function that listen to directory of path
                try
                {
                    ListenToDirectory(path);
                } catch (Exception e) {
                    //m_logging.Log("The directory " + path + " doesn't exist.", MessageTypeEnum.FAIL);
                    m_logging.Log(e.Message.ToString(), MessageTypeEnum.FAIL);
                }
            }
        }
        /*
         * function that listen to Directory of the path.
         * <param name = path> - the source path.
         */
        public void ListenToDirectory(string path)
        {
            //create directoryhandler that contains the members of constructor
            IDirectoryHandler handler = new DirectoryHandler(m_controller, m_logging);
            //add the handler command received to the event
            this.CommandRecieved += handler.OnCommandRecieved;
            //add the handler command received to directory closed
            handler.DirectoryClose += CloseHandler;
            this.directoriesHandler.Add(handler);
            //start handle of directory of the path.
            handler.StartHandleDirectory(path);
            //send messeage that server start to listen to directory.
            this.m_logging.Log(" start listening to directory " + path, MessageTypeEnum.INFO);
        }

        /*
         * function in case a directoryHandler closes.
         * <param name = sender> - is a object that send messege.
         * <param name = e> - the event that invoke the directory handler is closed
         */
        public void CloseHandler(object sender, DirectoryCloseEventArgs e)
        {
            IDirectoryHandler sendDirectoryHandler = (IDirectoryHandler)sender;
            if (sender is IDirectoryHandler)
            {
                this.m_logging.Log(e.Message, MessageTypeEnum.INFO); //stop listening message.
                //unsubscribing of the DirectoryHandler from the server message feed
                this.CommandRecieved -= sendDirectoryHandler.OnCommandRecieved;
                /*if (this.CommandRecieved == null)
                {
                    //if all the Directory Handlers closed succefully the server itself can finally close
                    this.m_logging.Log("Now, the server is closed", MessageTypeEnum.INFO);
                }*/
            }
            else
            {
                //an object that isn't supposed to activate the event did it
                this.m_logging.Log("source tried to abort handling folder: " + e.DirectoryPath, MessageTypeEnum.WARNING);
                return;
            }
        }
        /*function that close the server
         */
        public void CloseServer()
        {
            this.m_logging.Log("Now server is closing", MessageTypeEnum.INFO);
            CommandReceivedEventArgs commandRecievedEventArgs = new CommandReceivedEventArgs((int)CommandEnum.CloseCommand, null, "");
            this.CommandRecieved?.Invoke(this, commandRecievedEventArgs);
        }
        /*
         * function removes directory from handlers list and removes the listening to it in the eventhandler.
         * param name = path. the path to directory requested to be closed.
         * return true if closing handler succeed or false if not found or failed.
         */
        public bool CloseSpecificDir(string path)
        {
            foreach(IDirectoryHandler handler in directoriesHandler)
            {
                if(handler.getPath().Equals(path))
                {
                    this.CommandRecieved -= handler.OnCommandRecieved;
                    /*if (this.CommandRecieved == null)
                    {//listening to 0 directories warning message.
                        this.m_logging.Log("Service does not listen to any directory.", MessageTypeEnum.WARNING);
                    }*/
                    handler.CloseHandle();
                    directoriesHandler.Remove(handler);
                    return true;
                }
            }
            return false;
        }
    }
}