﻿using READER_0._1.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.ViewModel.ViewElement
{
    public class FolderView : INotifyPropertyChanged
    {
        public ObservableCollection<object> Files { get; private set; }
        private Type type;                                                                      
        public FolderView(string name) // общая папка
        {
            Name = name;
            Files = new ObservableCollection<object>();           
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        private bool correctName = true;
        public bool CorrectName
        {
            get
            {
                return correctName;
            }
            set
            {
                correctName = value;
                OnPropertyChanged(nameof(CorrectName));
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
        public void RemoveFile(object removedFile)
        {
            Files.Remove(removedFile);
        }
        public void RemoveFile(List<object> removedFiles)
        {
            foreach (var file in removedFiles)
            {
                Files.Remove(file);
            }            
        }
    }
}
