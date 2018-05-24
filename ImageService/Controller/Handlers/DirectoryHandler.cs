using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication.Enums;
using ImageService.Logging;
using Communication.Modal;
using Communication.Event;
using ImageService.Controller.Handlers;
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
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;
        /*
         * constructor
         * param name = controller initializes image controller
         * param name = log initialize loggingModal
         * also initialize new list of FileSystemWatchers.
         */
        public DirectoryHandler(IImageController controller, ILoggingService log)
        {
            this.imageController = controller;
            this.loggingModal = log;
            this.sysWatchers = new List<FileSystemWatcher>();
        }
        /*
         * param name = dirPath - directory path to start listen to.
         * Function starts listen to new files in type of types[] in the directory,
         * and adds them to the watchers list.
         */
        public void StartHandleDirectory(string dirPath)
        {
            this.path = dirPath;
            //make file system watcher for every type of file in the directory path.
            int i;
            for (i = 0; i < this.types.Length; i++)
            {
                FileSystemWatcher fileWatcher = new FileSystemWatcher(dirPath, this.types[i]);
                //start listen to files that being added.
                fileWatcher.EnableRaisingEvents = true;
                fileWatcher.Created += new FileSystemEventHandler(this.AddNewFileCommand);
                this.sysWatchers.Add(fileWatcher);
            }
        }
        /*
         * param name = sender - the object that sent command.
         * param name = e - the command that is recieved.
         * OnCommandRecieved checks if the command is for the directory,
         * if the command is to close it stops listening to the directory,
         * if the command is everything but close so go to controller to execute command.
         */
        public void OnCommandRecieved(object sender, CommandReceivedEventArgs e)
        {
            //if command is close, just close and dont go to conroller.
            if (e.CommandID == (int)CommandEnum.CloseCommand)
            {
                this.CloseHandle();
                return;
            }
            if (this.path.Equals(e.RequestDirPath) || e.RequestDirPath.Equals("*"))
            {
                bool result;
                // execute recieved command
                string message = this.imageController.ExecuteCommand(e.CommandID, e.Args, out result);
                if (result)
                {//execute command succeed
                    loggingModal.Log(message, MessageTypeEnum.INFO);
                }
                else
                {//execute command failed
                    loggingModal.Log(message, MessageTypeEnum.FAIL);
                }
            }
        }
        /*
         * param name = sender - the object that send file command.
         * param name = e - the watcher of the path directory.
         * function handles adding a file to a the directory we listen to.
         */
        private void AddNewFileCommand(object sender, FileSystemEventArgs e)
        {
            String[] args = { e.FullPath, e.Name };
            CommandReceivedEventArgs crea = new CommandReceivedEventArgs((int)CommandEnum.AddNewFileCommand,
                args, this.path);
            //add command to new file in the directory.
            this.OnCommandRecieved(this, crea);
        }
        //stop handle the directory.
        public void CloseHandle()
        {
            //clear the list of watchers.
            foreach(FileSystemWatcher s in sysWatchers)
            {
                s.EnableRaisingEvents = false;
            }
            this.sysWatchers.Clear();
            //send close message.
            DirectoryCloseEventArgs closeListen = new DirectoryCloseEventArgs(this.path, "Close listening to path- " + this.path);
            //inform closing.
            this.DirectoryClose?.Invoke(this, closeListen);
        }
        public string getPath()
        {
            return this.path;
        }
    }
}