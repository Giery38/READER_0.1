﻿using DevExpress.Utils.Native;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace READER_0._1.Command
{
    public abstract class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }
        public abstract void Execute(object parameter);

        public virtual void OnCanExecutedChanged() ///переодически remove excel file вызывает этот метод и вызыает ошибку из-за защиты основного потока
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
