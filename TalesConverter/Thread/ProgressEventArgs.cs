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
}
