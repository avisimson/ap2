using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Modal;
namespace ImageService.Commands
{
    class CloseCommand : ICommand
    {
        public event EventHandler<DirectoryCloseEventArgs> Closed;

        public string Execute(string[] args, out bool result)
        {
            string[] arguments = { };
            DirectoryCloseEventArgs e = new DirectoryCloseEventArgs(args[0], "Closing handler");
            Closed?.Invoke(this, e);
            result = true;
            return "Executed Close Command with arguments: " + args[0];
        }
    }
}
