using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Enums
{
    //All possible commands to service.
    /*
     * AddNewFileCommand-add new file to outputdir directory and to thumbnails.
     * CloseCommand-Close The Service.
     * RemoveHandlerCommand- Remove one directory handler we listen to from directories list.
     * GetConfigCommand-get all appConfig from service.
     * LogCommand-Get all log history.
     */
    public enum CommandEnum : int
    {
        AddNewFileCommand,
        CloseCommand,
        RemoveHandlerCommand,
        GetConfigCommand,
        LogCommand,
        CloseGUI
    }
}
