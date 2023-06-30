using READER_0._1.Model.Exel.Settings;
using System;
using System.Collections.Concurrent;
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
        public string TempFolderPath { get; private set; }        
        private bool close = false;

        public ExelReaderManager(string tempFolderPath)
        {
            TempFolderPath = tempFolderPath;            
            ExelFileReaders = new List<ExelFileReader>();
        }       
        public bool TryReadExelFile(ExelFile exelFile, ExelSettingsRead exelSettingsRead)
        {
            lock (this)
            {
                if (close == true)
                {
                    return false;
                }
                Thread threadMain = Thread.CurrentThread;
                string name = "Чтение Excel файла" + "&&" + exelFile.Path;
                threadMain.Name = name;
                try
                {
                    ExelFileReader exelFileReader = new ExelFileReader(exelFile, TempFolderPath, exelSettingsRead);
                    ExelFileReaders.Add(exelFileReader);
                    exelFile.AddPage(exelFileReader.Read());
                    exelFileReader.Close();                    
                    ExelFileReaders.Remove(exelFileReader);
                    exelFile.SetReaded(true);
                }
                catch (Exception)
                {
                    exelFile.Corrupted = true; 
                    return false;
                }
                return true;
            }            
        }     
        public void RemoveExelFileReader(ExelFile RemovedFile)
        {
            ExelFileReader exelFileReader = ExelFileReaders.Find(item => item.ExelFile == RemovedFile);
            if (exelFileReader != null)
            {
                exelFileReader.Close();                               
                ExelFileReaders.Remove(exelFileReader);
            }
        }
        public void Close()
        {
            ExelFileReaders.ForEach(reader => reader.Close());
            ExelFileReaders.Clear();
            close = true;       
        }
    }
}
