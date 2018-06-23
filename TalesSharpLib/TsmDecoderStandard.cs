using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalesSharp
{
    public partial class TsmDecoder
    {
        public static TsmDecoder V1 { get; } = new TsmDecoder();
        public static TsmDecoder V2 { get; } = new TsmDecoder();

        static TsmDecoder()
        {
            V1.Start = 0;
            V1.Pattern = 250;
            V1.Password = 115;
            V1.Salt = 3;

            V2.Start = 250;
            V2.Pattern = 250;
            V2.Password = 117;
            V2.Salt = 3;
            V2.Swap[0].Add(251, 255);
            V2.Swap[0].Add(75, 80);
        }
    }
}
