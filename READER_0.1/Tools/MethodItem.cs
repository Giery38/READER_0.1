using System;
using System.Windows;

namespace READER_0._1.Tools
{
    public class MethodItem()
    {

        private Delegate method;
        public object Sender { get; private set; }
        public string ThreadName { get; private set; }
        public object[] Args { get; private set; }
        public MethodItemStatus State { get; private set; }

        public event Action<(object sender, object result)> EndThread;
        public MethodItem(Delegate method, object sender, string threadName, object[] args) : this(new MethodItem())
        {
            this.method = method;
            Sender = sender;
            ThreadName = threadName;
            Args = args;
            State = MethodItemStatus.None;
        }

        private MethodItem(MethodItem queueItem)
        {

        }
        public object Start()
        {
            State = MethodItemStatus.Active;
            object result = method.DynamicInvoke(Args);
            State = MethodItemStatus.Finished;
            return result;
        }
    }
    public enum MethodItemStatus
    {
        None,        
        Active,
        Finished
    }
}
