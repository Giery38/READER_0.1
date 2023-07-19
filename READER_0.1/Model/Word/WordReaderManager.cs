using READER_0._1.Model.Exel.Settings;
using READER_0._1.Model.Exel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using READER_0._1.Model.Settings.Word;

namespace READER_0._1.Model.Word
{
    public class WordReaderManager
    {
        public List<WordFileReader> WordFileReaders { get; private set; }
        public string TempFolderPath { get; private set; }
        private bool close = false;

        public WordReaderManager(string tempFolderPath)
        {
            TempFolderPath = tempFolderPath;
            WordFileReaders = new List<WordFileReader>();
        }
        public bool TryReadExelFile(WordFile wordFile, WordSettingsRead wordSettingsRead)
        {
            lock (this)
            {
                if (close == true)
                {
                    return false;
                }
                Thread threadMain = Thread.CurrentThread;
                string name = "Чтение Word файла" + "&&" + wordFile.Path;
                threadMain.Name = name;               
                try
                {
                    WordFileReader wordFileReader = new WordFileReader(wordFile, TempFolderPath, wordSettingsRead);
                    WordFileReaders.Add(wordFileReader);
                    wordFile.Tables = wordFileReader.Read();
                    wordFileReader.Close();
                    WordFileReaders.Remove(wordFileReader);
                    wordFile.SetReaded(true);
                }
                catch (Exception)
                {
                    wordFile.Corrupted = true;
                    return false;
                }                
                return true;
            }
        }
    }
}
