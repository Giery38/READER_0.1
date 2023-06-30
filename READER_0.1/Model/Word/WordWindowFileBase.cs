using READER_0._1.Model.Exel.Settings;
using READER_0._1.Model.Exel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using READER_0._1.Model.Settings.Word;

namespace READER_0._1.Model.Word
{
    public class WordWindowFileBase
    {
        static public string TempFolderPath { get; private set; }
        public List<WordFile> WordFiles { get; private set; }

        private readonly Settings.Word.WordSettingsRead wordSettingsRead;
        private List<Thread> threadsReadFiles;

        public WordReaderManager WordReaderManager { get; private set; }

        public WordWindowFileBase(string tempFolderPath, Settings.Word.WordSettingsRead wordSettingsRead)
        {
            WordFiles = new List<WordFile>();
            threadsReadFiles = new List<Thread>();
            this.wordSettingsRead = wordSettingsRead;
            TempFolderPath = tempFolderPath;
            WordReaderManager = new WordReaderManager(tempFolderPath);
        }
        public void AddFiles(List<WordFile> AddedFiles)
        {
            AddedFiles = AddedFiles.Except(WordFiles).ToList();
            WordFiles.AddRange(AddedFiles);            
        }
        
        public bool TryReadWordFile(WordFile wordFile, WordSettingsRead wordSettingsRead)
        {
            bool result = false;
            Thread readWordFile = new Thread(() =>
            {
                result = WordReaderManager.TryReadExelFile(wordFile, wordSettingsRead);
            });
            readWordFile.Start();
            readWordFile.Join();
            return result;
        }
     
    }
}
