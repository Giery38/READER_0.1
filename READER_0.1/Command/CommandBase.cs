using DevExpress.Utils.Native;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace READER_0._1.Command
{
    abstract class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }
        public abstract void Execute(object parameter);

        protected void OnCanExecutedChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
