using READER_0._1.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.ViewModel.ViewElement
{
    public class FolderView : ViewModelBase
    {
        public ObservableCollection<object> Files { get; private set; }
        private Type type;                                                                      
        public FolderView(string name) // общая папка
        {
            Name = name;
            Files = new ObservableCollection<object>();           
        }
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public FolderView(string name, Type type) //форматная папка
        {
            Name = name;
            Files = new ObservableCollection<object>();
            this.type = type;
        } 
        public void AddFile(object file)
        {
            if (type != null)
            {
                var dd = file.GetType();
                if (file.GetType() == type)
                {
                    Files.Add(file);
                }
                else
                {
                    throw new Exception("Формат файла и папки не совпадают");
                }
            }
            else
            {
                Files.Add(file);
            }
        } 
        public void AddFiles<T>(List<T> addedFiles)
        {
            foreach (object file in addedFiles)
            {
                AddFile(file);
            }
        }
    }
}
