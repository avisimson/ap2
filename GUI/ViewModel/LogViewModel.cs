using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUI.Model;
using Communication.Modal;

namespace GUI.ViewModel
{
    //log view model class
    class LogViewModel : ILogViewModel
    {
        private ILogModel model;
        public event PropertyChangedEventHandler PropertyChanged;
        /// Initializes a new instance of log view model.
        public LogViewModel()
        {
            model = new LogModel();
            this.model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                this.NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }
        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        private void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        /// <summary>
        /// Gets the vm log entries.
        /// </summary>
        /// <value>
        /// The vm log entries.
        /// </value>
        public ObservableCollection<MessageReceivedEventArgs> VM_LogEntries
        {
            get
            {
                return this.model.LogEntries;
            }
        }
    }
}