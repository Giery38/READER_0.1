using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace READER_0._1.Tools
{
    public static class ThreadHelper
    {
        public static List<Thread> SerchThreadLive(List<Thread> threads)
        {
            List<Thread> threadsLive = new List<Thread>();
            for (int i = 0; i < threads.Count; i++)
            {
                if (threads[i].IsAlive == true)
                {
                    threadsLive.Add(threads[i]);
                }
            }
            return threadsLive;
        }
    }
}
