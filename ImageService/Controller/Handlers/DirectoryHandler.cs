using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System.Text.RegularExpressions;

namespace ImageService.Controller.Handlers
{
    public class DirectoryHandler : IDirectoryHandler
    {
        private IImageController imageController;
        private ILoggingService loggingModal;
        private List<FileSystemWatcher> sysWatchers;
        //directory path.
        private string path { get; set; }
        //file types to check.
        private String[] types = { "*.jpg", "*.png", "*.gif", "*.bmp" };
        // The Event That Notifies that the Directory is being closed
        event EventHandler<DirectoryCloseEventArgs> DirectoryClose;

        public DirectoryHandler(IImageController controller, ILoggingService log)
        {
            this.imageController = controller;
            this.loggingModal = log;
            this.sysWatchers = new List<FileSystemWatcher>();
        }
        // The Function Recieves the directory to Handle, and makes file system watchers
        //for every type of file to listen to.
        public void StartHandleDirectory(string dirPath)
        {
            this.path = dirPath;
            //make file system watcher for every type of file in the directory path.
            int i;
            for (i = 0; i < this.types.Length; i++)
            {
                FileSystemWatcher fileWatcher = new FileSystemWatcher(dirPath, this.types[i]);
                //start listen to events that occur.
                fileWatcher.EnableRaisingEvents = true;
                fileWatcher.Created += new FileSystemEventHandler(this.NewFile);
                this.sysWatchers.Add(fileWatcher);
            }
        }
        // The Event that will be activated upon new Command
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if (this.path.Equals(e.RequestDirPath) || e.RequestDirPath.Equals("*"))
            {
                // if command recieved is close command
                if (e.CommandID == (int)CommandEnum.CloseCommand)
                {
                    this.CloseHandle();
                    return;
                }
                bool result;
                // execute recieved command
                string message = this.imageController.ExecuteCommand(e.CommandID, e.Args, out result);
                // check if command has executed succesfully and write result to the log
                if (result)
                {
                    loggingModal.Log(message, MessageTypeEnum.INFO);
                }
                else
                {
                    loggingModal.Log(message, MessageTypeEnum.FAIL);
                }
            }
        }
        //stop handle the directory.
        public void CloseHandle()
        {
            this.sysWatchers.Clear();
            DirectoryCloseEventArgs closeListen = new DirectoryCloseEventArgs(this.path, "Closing path- " + this.path);
            this.DirectoryClose?.Invoke(this, closeListen);
        }
    }
}