using READER_0._1.Model;
using READER_0._1.Model.Settings.Word;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command.CommandWord
{
    class AddSearchStringInSettingsCommand : CommandBase
    {
        private readonly WindowFileBase windowFileBase;
        private readonly WordViewModel wordViewModel;
        public AddSearchStringInSettingsCommand(WordViewModel wordViewModel, WindowFileBase windowFileBase)
        {
            this.windowFileBase = windowFileBase;
            this.wordViewModel = wordViewModel;
        }

        public override void Execute(object parameter)
        {
            if (parameter is (SearchParagraph searchParagraph, string addedValue))
            {                               
                searchParagraph.SearchStrings.Add(new SearchString());              
            }          
        }
    }
}
