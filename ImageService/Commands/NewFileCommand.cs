using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public class NewFileCommand : ICommand
    {
        private IImageServiceModal m_modal;
       /*
        * constructor
        * param name =  modal is an IimageServiceModal.
        */
        public NewFileCommand(IImageServiceModal modal)
        {
            // Storing the Modal
            m_modal = modal;
        }
        /*
         * function that execute the command accodring to "ImageServiceModel"
         * param name =  args is the args to the command.
         * param name = result is a out bool var gets true if command executed successfuly and false otherwise.
         * return the result of the command or an error message.
         */
        public string Execute(string[] args, out bool result)
        {
            // args[0] is the path.
            return this.m_modal.AddFile(args[0], out result);
        }
    }
}
