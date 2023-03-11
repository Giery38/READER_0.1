using READER_0._1.Model.Exel;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command.CommandExel
{
    class RemoveExelFileCommand : CommandBase
    {
        private readonly ExelViewModel exelViewModel;

        public RemoveExelFileCommand(ExelViewModel exelViewModel)
        {
            this.exelViewModel = exelViewModel;
        }

        public override void Execute(object parameter)
        {
            (ExelFile exelFile, string folderName) remove = ((ExelFile, string))parameter;
            exelViewModel.RemoveExelFile(remove.exelFile, remove.folderName);
        }
    }
}
