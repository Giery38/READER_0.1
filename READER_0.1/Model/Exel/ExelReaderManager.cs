using READER_0._1.Model.Excel.Settings;
using READER_0._1.ViewModel.ViewElement;
using READER_0._1.ViewModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using READER_0._1.Tools;
using READER_0._1.Tools.ThreadManagers;

namespace READER_0._1.Model.Excel
{
    public class ExcelReaderManager
    {
        public List<ExcelFileReader> ExcelFileReaders { get; private set; }
        public string TempFolderPath { get; private set; }

        private bool close = false;
        private QueueManager readManager;

        public ExcelReaderManager(string tempFolderPath)
        {
            TempFolderPath = tempFolderPath;            
            ExcelFileReaders = new List<ExcelFileReader>();
            readManager = new QueueManager("ReadExcelManager");      
        }       
        public void Read(ExcelFile excelFile, ExcelSettingsRead excelSettingsRead)
        {
            readManager.AddFunc(new Func<ExcelFile, ExcelSettingsRead, Exception>(ReadExcelFileBody), excelFile, "Чтение Excel файла" + "&&" + excelFile.Path, new object[] { excelFile, excelSettingsRead });
        }
        private Exception ReadExcelFileBody(ExcelFile excelFile, ExcelSettingsRead excelSettingsRead)
        {
            Exception exception = null;
            if (close == true)
            {
                return exception;
            }           
              try
              {
                  ExcelFileReader excelFileReader = new ExcelFileReader(excelFile, TempFolderPath, excelSettingsRead);
                  ExcelFileReaders.Add(excelFileReader);
                  excelFile.AddPage(excelFileReader.Read());
                  excelFileReader.Close();
                  ExcelFileReaders.Remove(excelFileReader);
                  excelFile.SetReaded(true);
              }
              catch (Exception e)
              {
                  exception = e;
                  excelFile.SetCorrupted(true);                
              }
            
            return exception;
        }    
        public void RemoveExcelFileReader(ExcelFile RemovedFile)
        {
            ExcelFileReader excelFileReader = ExcelFileReaders.Find(item => item.ExcelFile == RemovedFile);
            if (excelFileReader != null)
            {
                excelFileReader.Close();                               
                ExcelFileReaders.Remove(excelFileReader);
            }
            readManager.RemoveItem(RemovedFile);
        }
        public void Close()
        {
            ExcelFileReaders.ForEach(reader => reader.Close());
            ExcelFileReaders.Clear();
            readManager.Close();
            close = true;       
        }
        public void QueuesManagerStart()
        {

        }
    }
}
