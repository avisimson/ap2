using ImageService.Commands;
using Communication.Enums;
using ImageService.Logging;
using ImageService.Modal;
using ImageService.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModal m_modal;                      // The Modal Object
        private ILoggingService m_logging;                  //all logs for log commands.
        private Dictionary<int, ICommand> commands;
        private ImageServer server;
        /*
         * constructor
         * param name =  modal is an IimageServiceModal.
         * param name = logging, is a variable of logging service interface that holds all logs of the service.
         */
        public ImageController(IImageServiceModal modal, ILoggingService logging)
        {
            // Storing the Modal Of The System
            m_modal = modal;
            m_logging = logging;
            commands = new Dictionary<int, ICommand>()
            {
                // For Now will contain NEW_FILE_COMMAND, CloseHandler COMMAND, GET CONFIG AND GET LOG.
                { (int)CommandEnum.AddNewFileCommand, new NewFileCommand(this.m_modal) },
                { (int)CommandEnum.LogCommand, new LogCommand(this.m_logging) },
                { (int)CommandEnum.GetConfigCommand, new GetConfigCommand() }
            };
        }
        public ImageServer Server
        {//implementation for server.
            get
            {
                return this.server;
            }
            set
            {
                this.server = value;
                commands.Add((int)CommandEnum.RemoveHandlerCommand, new RemoveHandlerCommand(value));
            }
        }
        /*
         * function that execute the command that constoller sender to ICommand
         * param name =  commandID is an id of command.
         * param name = args is a array of the arguments from service.
         * param name = resultSuccessful is a out bool varת
         * gets true if command executed successfuly and false otherwise 
         * <return> return the result of the command or an error message
         */
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            //execute the command that we are need to implement
            return this.commands[commandID].Execute(args, out resultSuccesful);
        }
    }
}
