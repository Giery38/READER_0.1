using READER_0._1.Model.Settings.Exel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace READER_0._1.Model.Exel
{
    public class ExelReaderManager
    {
        public List<ExelFileReader> ExelFileReaders { get; private set; }
        public List<Thread> ThreadsReadFiles { get; private set; }
        public string TempFolderPath { get; private set; }
        public ExelSettingsRead ExelSettingsRead { get; private set; }

        public ExelReaderManager(string tempFolderPath, ExelSettingsRead exelSettingsRead)
        {
            TempFolderPath = tempFolderPath;
            ExelSettingsRead = exelSettingsRead;
            ExelFileReaders = new List<ExelFileReader>();
            ThreadsReadFiles = new List<Thread>();
        }
        public Thread FindThreadsReadFile(string filePath)
        {
            string[] name;

            for (int i = 0; i < ThreadsReadFiles.Count; i++)
            {
                name = ThreadsReadFiles[i].Name.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
                if (filePath == name[1])
                {
                    return ThreadsReadFiles[i];
                }
            }
            return null;
        }
        public bool TryReadExelFile(ExelFile exelFile)
        {
            Thread threadMain = Thread.CurrentThread;
            threadMain.Name = "Чтение Excel файла" + "&&" + exelFile.Path;
            ThreadsReadFiles.Add(threadMain);
            Thread readExelFile = null;
            try
            {
                bool TryReadExelFileResult = false;
                readExelFile = new Thread(() =>
                {
                    TryReadExelFileResult = InternalReadExelFile(exelFile);
                });               
                readExelFile.Start();               
                readExelFile.Join();                
                if (ThreadsReadFiles.Contains(threadMain))
                {
                    ThreadsReadFiles.Remove(threadMain);
                }
                return TryReadExelFileResult;
            }
            catch (Exception)
            {                
                ExelFileReader exelFileReader = ExelFileReaders.Find(item => item.ExelFile == exelFile);
                if (exelFileReader != null)
                {
                    exelFileReader.Close();
                    ExelFileReaders.Remove(exelFileReader);
                }
                if (ThreadsReadFiles.Contains(threadMain))
                {
                    ThreadsReadFiles.Remove(threadMain);
                }
                GC.Collect();
                return false;
            }
        }
        public bool InternalReadExelFile(ExelFile exelFile)
        {            
            ExelFileReader exelFileReader = new ExelFileReader(exelFile, TempFolderPath, ExelSettingsRead);
            ExelFileReaders.Add(exelFileReader);
            try
            {              
                if (exelFile != null)
                {
                    exelFile.AddPage(exelFileReader.Read());
                    exelFile.SetReaded(true);
                }
                if (ExelFileReaders.Find(item => item.ExelFile == exelFileReader.ExelFile) != null)
                {
                    exelFileReader.Close();
                    ExelFileReaders.Remove(exelFileReader);
                }
                return true;
            }
            catch (Exception ex)
            {
                //Process.Start("cmd.exe", "/K echo ошибка" + ex.ToString());
                if (exelFile != null)
                {
                    exelFile.Corrupted = true;
                }
                if (ExelFileReaders.Find(item => item.ExelFile == exelFileReader.ExelFile) != null)
                {
                    exelFileReader.Close();
                    ExelFileReaders.Remove(exelFileReader);
                }
                return false;
            }
        }

    }
}
