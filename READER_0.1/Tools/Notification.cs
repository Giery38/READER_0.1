using READER_0._1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace READER_0._1.Tools
{
    public class Notification : ModelPropertyChanged
    {
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
        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            private set
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
        private Brush brush;
        public Brush Brush 
        {
            get 
            {
                return brush;
            }
            private set 
            {
                brush = value;
                OnPropertyChanged(nameof(Brush));
            }
        }

        public int TimeExistence { get; private set; }        
        public Notification(string name, string text, Brush brush, int timeExistence)
        {
            Name = name;
            Text = text;
            Brush = brush;
            TimeExistence = timeExistence;
        }
        public void Show()
        {
            NotificationManager.ShowNotification(this);
        }
        public void Hide()
        {
            TimeExistence = 0;
        }
    }
}
