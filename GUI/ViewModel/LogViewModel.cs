using ImageServiceGUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedFiles;
namespace GUI.ViewModel
{
    class LogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ILogModel logModel = new LogModel();
        public ObservableCollection<LogTuple> Logs
        {
            get { return this.logModel.Logs; }
            set => throw new NotImplementedException();
        }

    }
}