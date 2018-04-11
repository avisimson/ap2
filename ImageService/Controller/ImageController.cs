using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;
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
        private Dictionary<int, ICommand> commands;
        /*
         * constructor
         * param name =  modal is an IimageServiceModal.
         */
        public ImageController(IImageServiceModal modal)
        {
            // Storing the Modal Of The System
            m_modal = modal;                    
            commands = new Dictionary<int, ICommand>()
            {
                // For Now will contain NEW_FILE_COMMAND
                {(int) CommandEnum.AddNewFileCommand, new NewFileCommand(this.m_modal) }
            };
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
