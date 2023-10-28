using READER_0._1.Model.Excel.Settings;
using READER_0._1.Model.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MS.WindowsAPICodePack.Internal;
using System.Collections;
using READER_0._1.Tools.ThreadManagers;

namespace READER_0._1.Tools.ThreadManagers
{
    public class QueueManager : ThreadManager
    {        
        public QueueManager(string name) : base(name)
        {            
            
        }       
        private bool _active;
        private bool active
        {
            get
            {
                return _active;
            }
            set
            {
                if (_active != value)
                {                   
                    if (methods.Count > 0)
                    {
                        MethodItem startedFunc = methods.First();                        
                        Start(startedFunc);                        
                        _active = true;                      
                    }
                    else
                    {
                        _active = false;
                    }
                }
            }
        }
        /// <summary>    
        /// Если в списке действий есть элемент с таким же sender то новый элемент не будет добавлен
        /// </summary> 
        public void AddFunc(Delegate method, object sender, string threadName, params object[] args)
        {
            if (methods.FirstOrDefault(item => item.Sender.Equals(sender) == true) == null)
            {
                methods.Add(new MethodItem(method, sender, threadName, args));
                active = true;
            }          
        }        
        public void RemoveItem(object sender)// если задержка при удалении будет большая то код будет работать не правильно попровить это!!!!!
        {
            for (int i = 0; i < methods.Count; i++)
            {
                if (methods[i].Sender.Equals(sender) == true)
                {
                    methods.RemoveAt(i);
                }
            }
        }
        public void Close()
        {
            methods.Clear();
        }
        protected override void Start(MethodItem item)
        {
            Thread task = new Thread(_ =>
            {
                object result = item.Start();
                OnEndThread(item.Sender,result);
                methods.RemoveAt(0);
                active = false;                
            });
            task.Name = Name + " " + item.ThreadName;
            task.Start();
        }

    }
}
