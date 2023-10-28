using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Tools.ThreadManagers
{
    public abstract class ThreadManager
    {
        public string Name { get; private set; }
        protected List<MethodItem> methods;
        protected event Action<(object sender, object result)> EndThread;
        protected event Action<(object sender, object result)> innerEndThread;
       
        public ThreadManager(string name)
        {
            Name = name;
            methods = new List<MethodItem>();          
        }
       
        protected void OnInnerEndThread(object sender, object result)
        {
            innerEndThread?.Invoke((sender, result));
        }
        protected void OnEndThread(object sender, object result)
        {
            EndThread?.Invoke((sender, result));
        }      
        protected abstract void Start(MethodItem methodItem);        
    }
}
