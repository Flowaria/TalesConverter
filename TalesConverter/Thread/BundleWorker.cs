using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesConverter.Thread
{
    public class ProgressEventArgs : EventArgs
    {
        public int Max { get; set; }
        public int Current { get; set; }

        public ProgressEventArgs(int currentcount, int maxcount)
        {
            Max = maxcount;
            Current = currentcount;
        }
    }

    public class BundleWorker
    {
        public string SaveDirectory { get; set; }
        public int MaxThread { get; set; }

        public int TotalResources { get; protected set; }
        public int DoneResources { get; protected set; } = 0;

        public delegate void WorkEndHandler(object sender, ProgressEventArgs e);
        public event WorkEndHandler OnSingleEnd;
        public event WorkEndHandler OnTotalCalculated;

        protected void SingleDone()
        {
            if (OnSingleEnd == null) return;
            OnSingleEnd(this, new ProgressEventArgs(++DoneResources, TotalResources));
        }

        protected void TotalCalculated()
        {
            if (OnTotalCalculated == null) return;
            OnTotalCalculated(this, new ProgressEventArgs(DoneResources, TotalResources));
        }

        public virtual async Task Handle(List<string> files)
        {

        }
    }
}
