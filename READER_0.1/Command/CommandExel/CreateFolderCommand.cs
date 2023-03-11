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
        private readonly WindowFileBase windowFileBase;
        private readonly ViewModel.ExelViewModel exelViewModel;
        protected event Action ChangeFoldersList;
        public CreateFolderCommand(ExelViewModel exelViewModel, WindowFileBase windowFileBase)
        {
            this.windowFileBase = windowFileBase;
            this.exelViewModel = exelViewModel;
            ChangeFoldersList += exelViewModel.UpdateFiles;
        }
        public override void Execute(object parameter)
        {
            List<string> folderViewsConvert = windowFileBase.exelWindowFileBase.FoldersWithFiles.Where(x => x is Directory).Select(x => ((Directory)x).Name).ToList();   
            string name = "Новая папка";
            while (folderViewsConvert.Find(item => item == name) != null)
            {
                name = name + "(1)";
            }
            windowFileBase.exelWindowFileBase.AddFolder(name);
            ChangeFoldersList.Invoke();
            //exelViewModel.FoldersView.Add(new FolderView(name));
        }
    }
}
