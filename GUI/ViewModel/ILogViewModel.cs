﻿using Communication.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModel
{
    interface ILogViewModel : INotifyPropertyChanged
    {
        ObservableCollection<MessageReceivedEventArgs> VM_LogEntries { get; }
    }
}