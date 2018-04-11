using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging.Modal;
using ImageService.Logging;
using ImageService.Modal;
using System.Configuration;

namespace ImageService.Server
{
    class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion
        /*
         * constructor
         * <param name = contoller> - the ImageController.
         * <param name = logging> - the LoggingService.
         */
        public ImageServer(IImageController controller, ILoggingService logging)
        {
            this.m_controller = controller;
            this.m_logging = logging;
            //initilaize the directoryHandlers.
            CreateDirectoryHandlers();
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
                ListenToDirectory(path);
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
            //start handle of directory of the path.
            handler.StartHandleDirectory(path);
            //send messeage that server start to listen to directory.
            m_logging.Log(DateTime.Now.ToString() + " start listening to directory " + path, MessageTypeEnum.INFO);
        }
        /*
         * function that close in case that the Service closes
         * <param name = path> - the source path.
         */
        public void CloseHandlers()
        {
            foreach (EventHandler<CommandRecievedEventArgs> handler in CommandRecieved.GetInvocationList())
            {
                //close every event handler that found in the list.
                handler(this, new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, null));
                //delete him from the list.
                CommandRecieved -= handler;
            }
        }

        /*
         * function in case a directoryHandler closes.
         * <param name = sender> - is a object that send messege.
         * <param name = e> - the event that invoke the directory handler is closed
         */
        public void CloseHandler(object sender, DirectoryCloseEventArgs e)
        {
            IDirectoryHandler sendDirectoryHandler = sender as IDirectoryHandler;
            if (sender is IDirectoryHandler)
            {
                this.m_logging.Log("Directory Handler of Directory in: " + e.DirectoryPath + @" with message: " + e.Message, MessageTypeEnum.INFO);
                //unsubscribing of the DirectoryHandler from the server message feed
                this.CommandRecieved -= sendDirectoryHandler.OnCommandRecieved;
                if (this.CommandRecieved == null)
                {
                    //if all the Directory Handlers closed succefully the server itself can finally close
                    this.m_logging.Log("Now, the server is closed", MessageTypeEnum.INFO);
                }
            } else {
                //an object that isn't supposed to activate the event did it
                this.m_logging.Log("source tried to abort handling folder: " + e.DirectoryPath, MessageTypeEnum.WARNING);
                return;
            }
        }
        public void CloseServer()
        {
            this.m_logging.Log("Now server is closing", MessageTypeEnum.INFO);
            CommandRecievedEventArgs commandRecievedEventArgs = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, "");
            this.CommandRecieved?.Invoke(this, commandRecievedEventArgs);
        }
    }
}