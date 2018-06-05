using Communication.Connection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Model
{
    //main window model interface.
    interface IMainWindowModel : INotifyPropertyChanged
    {
        bool IsConnected { get; set; }
        IClientConnection Client { get; }
    }
}