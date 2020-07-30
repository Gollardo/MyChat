﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyChatClient.ViewModel.Commands
{
    public class SendMessageCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public MainWindowModel ViewModel { get; set; }

        public SendMessageCommand(MainWindowModel vieModel)
        {
            this.ViewModel = vieModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //this.ViewModel.SendMessageToServer(parameter as String);
            this.ViewModel.ListenServer();
        }
    }
}
