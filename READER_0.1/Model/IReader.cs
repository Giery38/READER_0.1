using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">T это type возвращаемый из Reader</typeparam>
    public interface IReader<T>
    {
        public event EventHandler<ReadEventArgs> ReadEnd;       
        public abstract void Read();       
        public abstract void Close();
        public class ReadEventArgs : EventArgs
        {
            public T ResultRead { get; private set; }
            public Exception Exception { get; private set; }
            public ReadEventArgs(T resultRead, Exception exception)
            {
                ResultRead = resultRead;
                Exception = exception;
            }          
        }
    }
}
