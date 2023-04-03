using READER_0._1.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace READER_0._1.ViewModel.tools
{    
    public class LoadedTablesManager
    {
        private List<Thread> interactionsPull = new List<Thread>();
        public LoadedTablesManagerState State { get; private set; }
        public EventHandler LoadingTableChange;
        private double loadingTable;        
        public double LoadingTable 
        {
            get 
            { 
                return loadingTable;
            }
            private set
            {
                if (value != loadingTable)
                {
                    LoadingTableChange?.Invoke(this, new EventArgs());
                }
                loadingTable = value;
            } 
        }       

        public void Start()
        {
            State = LoadedTablesManagerState.Works;
            Thread LoadedTablesManagerThread = new Thread(() => ThreadMethod());
            LoadedTablesManagerThread.Start();
        }
        private void ThreadMethod()
        {
            List<Thread> activeInteractions = new List<Thread>();
            while (State == LoadedTablesManagerState.Works)
            {
                if (interactionsPull.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }
                activeInteractions = ThreadHelper.SerchThreadLive(interactionsPull);
                if (activeInteractions.Count > 0)
                {
                    LoadingTable = 0;
                    interactionsPull.ForEach(item => item.Join());
                }
                else if (interactionsPull?.Last()?.IsAlive == false && activeInteractions?.Count == 0)
                {
                    LoadingTable = 0;
                    Thread lastNow = interactionsPull?.Last();
                    int lastNowNomber = interactionsPull.IndexOf(lastNow);
                    lastNow?.Start();
                    lastNow?.Join();
                    if (interactionsPull.Count > 0)
                    {
                        interactionsPull?.RemoveRange(0, lastNowNomber + 1);
                    }
                    LoadingTable = 1;
                }
                if (LoadingTable != 1)
                {
                    LoadingTable = 1;
                }
            }       
        }        
        public void LoadTable(Thread thread)
        {
            interactionsPull.Add(thread);
        }
        public void Stop()
        {
            State = LoadedTablesManagerState.Stop;
        }
    }
    
    public enum LoadedTablesManagerState
    {
        Works,
        Stop,
        Clane
    }
}
