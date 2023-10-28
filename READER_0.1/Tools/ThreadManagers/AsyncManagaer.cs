using DocumentFormat.OpenXml.Wordprocessing;
using MS.WindowsAPICodePack.Internal;
using READER_0._1.Tools.ThreadManagers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace READER_0._1.Tools.ThreadManagers
{
    public class AsyncManager : ThreadManager
    {             
        public int MaxAliveThreads { get; private set; }        
        private int aliveThreads;
        public int AliveThreads 
        {
            get
            {
                return aliveThreads;
            }
            private set
            {
                if (value < aliveThreads)
                {                    
                    if (aliveThreads < MaxAliveThreads && methods.Count > 0)
                    {
                        MethodItem method = methods.FirstOrDefault(item => item.State == MethodItemStatus.None);
                        if (method != null)
                        {
                            Start(method);
                        }
                    }
                }
                aliveThreads = value;
            }
        }

        public AsyncManager(string name, int maxAliveThreads) : base(name)
        {            
            MaxAliveThreads = maxAliveThreads;
            innerEndThread += AsyncManager_innerEndThread;           
        }

        private void AsyncManager_innerEndThread((object sender, object result) obj)
        {           
            methods.Remove(FindMethodItem(obj.sender));
            AliveThreads--;
            OnEndThread(obj.sender,obj.result);
        }

        /// <summary>
        /// параметр outOfTur отвечает за запуск метода вне очереди.
        /// Если в списке действий есть элемент с таким же sender то новый элемент не будет добавлен
        /// </summary>        
        public void AddFunc(Delegate method, object sender, string threadName,bool outOfTurn, params object[] args)
        {
            if (FindMethodItem(sender) != null)
            {
                return;
            }
            MethodItem methodItem = new MethodItem(method, sender, threadName, args);
            methods.Add(new MethodItem(method, sender, threadName, args));
            if (outOfTurn == true)
            {
                Start(methodItem);
            }
            else if (AliveThreads < MaxAliveThreads)
            {
                Start(methodItem);              
            }
        }
        public void HotStart(MethodItem methodItem)
        {
            Start(methodItem);
        }
        public MethodItem FindMethodItem(object Sender)
        {
            return methods.Find(item => item.Sender.Equals(Sender) == true);
        }       
        protected override void Start(MethodItem methodItem)
        {
            AliveThreads++;
            Thread thread = new Thread(_ =>
            {                
                object result = methodItem.Start();
                OnInnerEndThread(methodItem.Sender, result);                
            });
            thread.Name = Name + " " + methodItem.ThreadName;
            thread.Start();
          
        }
    }
}
