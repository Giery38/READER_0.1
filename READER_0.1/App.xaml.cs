using READER_0._1.Model;
using READER_0._1.Model.Settings;
using READER_0._1.View;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace READER_0._1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly WindowFileBase windowFileBase;
        private Settings settings;
        private static string tempFolderPath;
        private string configFilePath;
        public App()
        {                    
            tempFolderPath = CreateTempFolder();
            CleansingTempFolder();
            SetSettings();
            windowFileBase = new WindowFileBase(tempFolderPath, settings);                     
        }        
        protected override void OnStartup(StartupEventArgs e)
        {
           
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(windowFileBase)
                //DataContext = mainViewModel
            };
            MainWindow.Show();
            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            CleansingTempFolder();
            base.OnExit(e);
        }
        private void SetSettings()
        {
            configFilePath = CreateConfigFile();
            FileStream stream = new FileStream(configFilePath, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            Settings settings = new Settings();
            try
            {
                settings = (Settings)serializer.Deserialize(stream);
            }
            catch (Exception)
            {

            }
            stream.Close();

            this.settings = settings;
            this.settings.SetConfigFilePath(configFilePath);

        }

        private string CreateConfigFile()
        {
            FileStream stream = new FileStream("settings.xml", FileMode.OpenOrCreate);
            stream.Close();
            return stream.Name;
        }
        private string CreateTempFolder()
        {

            string tempPath = Path.GetTempPath();
            string uniqueFolderName = "112-d8bac726-file-6b75-4d79-9bd3-transfer-48668f2336f1-transfer";
            string tempFolderPath = Path.Combine(tempPath, uniqueFolderName);
            if (!System.IO.Directory.Exists(tempFolderPath))
            {
                DirectoryInfo directory = System.IO.Directory.CreateDirectory(tempFolderPath);
            }
            return tempFolderPath;
        }
        static private void CleansingTempFolder()
        {
            List<string> files = new List<string>();
            try
            {
                files = System.IO.Directory.GetFiles(tempFolderPath).ToList();
            }
            catch
            {
            }
            foreach (string filePath in files)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    string[] idString = fileName.Split("id022-");
                    int.TryParse(idString[1], out int id);
                    Process processe = Process.GetProcessById(id);
                    processe.Kill();
                }
                catch
                {
                }
            }
            foreach (string file in files)
            {
                try
                {
                    System.IO.File.Delete(file);
                }
                catch
                {
                }
            }
        }
    }
}
