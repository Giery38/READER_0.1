using READER_0._1.Model;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command.CommandExel
{
    class FolderViewNameChangeCommand : CommandBase
    {
        private readonly WindowFileBase windowFileBase;
        private readonly ExelViewModel exelViewModel;

        public FolderViewNameChangeCommand(ExelViewModel exelViewModel, WindowFileBase windowFileBase)
        {
            this.windowFileBase = windowFileBase;
            this.exelViewModel = exelViewModel;
        }

        public override void Execute(object parameter)
        {
            (string oldName, string newName) info = ((string, string))parameter;
            exelViewModel.SetFolderViewName(info.oldName, info.newName);
        }
    }
}
