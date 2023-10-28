using READER_0._1.Model.Excel.Settings;
using READER_0._1.Model.Excel;
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
        public void Read(WordFile wordFile, WordSettingsRead wordSettingsRead)
        {
            if (close == true)
            {
                return;
            }
            WordFileReader wordFileReader = new WordFileReader(wordFile, TempFolderPath, wordSettingsRead);
            wordFileReader.ReadEnd += ReadEnd;
            WordFileReaders.Add(wordFileReader);
            wordFileReader.Read();           
        }

        private void ReadEnd(object sender, IReader<List<Excel.TableData.Table>>.ReadEventArgs e)
        {
            WordFileReader wordFileReader = sender as WordFileReader;
            if (wordFileReader == null)
            {
                return;
            }
            try
            {
                if (e.Exception != null)
                {
                    wordFileReader.WordFile.SetCorrupted(true);
                    return;
                }
                wordFileReader.Close();
                WordFileReaders.Remove(wordFileReader);
                wordFileReader.WordFile.SetReaded(true);
            }
            catch (Exception)
            {
               
            }           
        }
    }
}
