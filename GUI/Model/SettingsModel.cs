﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Infrastructure;
using Infrastructure.Enums;
using GUI.Model;
using Infrastructure.Event;
using Newtonsoft.Json;
using Prism.Commands;

namespace GUI.ViewModel
{
    class SettingsViewModel : ISettingsModel
    {
        private ISettingsModel model;

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsViewModel()
        {
            this.RemoveCommand = new DelegateCommand<object>(this.OnRemove, this.CanRemove);
            this.model = new SettingsModel();
            this.model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                this.NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }

        private void OnRemove(object obj)
        {
            string[] args = { this.model.SelectedHandler };
            CommandReceivedEventArgs eventArgs = new CommandReceivedEventArgs((int)CommandEnum.CloseCommand, args, null);

            this.model.Connection.Write(eventArgs);
            this.model.Connection.Read();
            //this.model.Handlers.Remove(this.model.SelectedHandler);
        }

        private bool CanRemove(object arg)
        {
            if (string.IsNullOrEmpty(this.model.SelectedHandler))
            {
                return false;
            }
            return true;
        }

        public ICommand RemoveCommand
        {
            get; private set;
        }

        public string OutputDirectory
        {
            get { return this.model.OutputDirectory; }
        }

        public string SourceName
        {
            get { return this.model.SourceName; }
        }

        public string LogName
        {
            get { return this.model.LogName; }
        }

        public int ThumbnailSize
        {
            get { return this.model.ThumbnailSize; }
        }

        public string SelectedHandler
        {
            get { return this.model.SelectedHandler; }
            set
            {
                this.model.SelectedHandler = value;
                var command = this.RemoveCommand as DelegateCommand<object>;
                command.RaiseCanExecuteChanged();
            }
        }

        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public ObservableCollection<string> VM_Handlers
        {
            get { return this.model.Handlers; }
            set { this.model.Handlers = value; }
        }
    }
}