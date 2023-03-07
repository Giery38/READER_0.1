using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace READER_0._1.Model.Word
{
    public class WordWindowFileBase
    {
        static public string TempFolderPath { get; private set; }
        public List<WordFile> WordFiles { get; private set; }

        private readonly Settings.Word.WordSettingsRead wordSettingsRead;
        private List<Thread> threadsReadFiles;

        public WordWindowFileBase(string tempFolderPath, Settings.Word.WordSettingsRead wordSettingsRead)
        {
            WordFiles = new List<WordFile>();
            threadsReadFiles = new List<Thread>();
            this.wordSettingsRead = wordSettingsRead;
            TempFolderPath = tempFolderPath;
        }
        public void AddFiles(List<WordFile> AddedFiles)
        {
            AddedFiles = AddedFiles.Except(WordFiles).ToList();
            WordFiles.AddRange(AddedFiles);
            Thread readExelFiles = new Thread(() => ReadWordFiles(AddedFiles));
            readExelFiles.Start();
        }
        private void ReadWordFiles(List<WordFile> wordFileReaed)
        {
            foreach (WordFile wordFile in wordFileReaed)
            {
                Thread readExelFile = new Thread(() => ReadWordFile(wordFile));
                readExelFile.IsBackground = true;
                readExelFile.Name = "Чтение Word файла " + wordFile.FileName;
                threadsReadFiles.Add(readExelFile);
                readExelFile.Start();
                readExelFile.Join();
            }
        }
        private void ReadWordFile(WordFile wordFile)
        {
            int id = WordFiles.FindIndex(file => file.FileName == wordFile.FileName);
            WordFileReader exelFileReader = new WordFileReader(WordFiles[id], TempFolderPath, wordSettingsRead);
            exelFileReader.Read();
            WordFiles[id].SetReaded(true);
        }
    }
}
