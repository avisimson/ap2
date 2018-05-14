using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUI.Model;
using Communication.Model;

namespace GUI.ViewModel
{
    class LogViewModel : ILogViewModel
    {
        private ILogModel model;
        public event PropertyChangedEventHandler PropertyChanged;

        public LogViewModel()
        {
            model = new LogModel();
            this.model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                this.NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }

        private void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public ObservableCollection<MessageReceivedEventArgs> VM_LogEntries
        {
            get
            {
                return this.model.LogEntries;
            }
        }
    }
}