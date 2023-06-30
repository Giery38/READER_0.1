using READER_0._1.Model;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command.CommandWord
{
    class ReadWordFileCommand : CommandBase
    {
        private readonly WindowFileBase windowFileBase;
        private readonly WordViewModel wordViewModel;
        public ReadWordFileCommand(WordViewModel wordViewModel, WindowFileBase windowFileBase) // добавляет все файлы, переопрелить для каждого окна
        {
            this.windowFileBase = windowFileBase;
            this.wordViewModel = wordViewModel;
        }
        public override void Execute(object parameter)
        {
            wordViewModel.ReadWordFile(wordViewModel.SelectedWordFile, wordViewModel.SelectedSettings);
        }
    }
}
