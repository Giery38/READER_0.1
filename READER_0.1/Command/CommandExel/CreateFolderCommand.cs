using READER_0._1.Model;
using READER_0._1.ViewModel;
using READER_0._1.ViewModel.ViewElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command.CommandExel
{
    class CreateFolderCommand : CommandBase
    {
        private readonly ViewModel.ExelViewModel exelViewModel;
        public CreateFolderCommand(ExelViewModel exelViewModel)
        {;
            this.exelViewModel = exelViewModel;
        }
        public override void Execute(object parameter)
        {
            List<string> folderViewsConvert = exelViewModel.FoldersView.Where(x => x is FolderView).Select(x => ((FolderView)x).Name).ToList();
            string name = "Новая папка";
            while (folderViewsConvert.Find(item => item == name) != null)
            {
                name = name + "(1)";
            }
            exelViewModel.AddFolderView(name);            
        }
    }
}
