using GUI.Communication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Model
{
    /// <summary>
    /// main window model interface
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    interface IMainWindowModel : INotifyPropertyChanged
    {
        bool IsConnected { get; set; }
        IClientConnection Client { get; }
    }
}