using SwfDotNet.IO.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwfDotNet.IO.Tags
{
    public class SymbolClass : SwfDotNet.IO.Tags.BaseTag
    {
        private Dictionary<int, string> tagsData;

        /// <summary>
        /// Creates a new <see cref="SymbolClass"/> instance.
        /// </summary>
        public SymbolClass()
        {
            this._tagCode = (int)TagCodeEnum.SymbolClass;
            tagsData = new Dictionary<int, string>();
        }

        /// <summary>
        /// see <see cref="SwfDotNet.IO.Tags.BaseTag">base class</see>
        /// </summary>
        public override void ReadData(byte version, BufferedBinaryReader binaryReader)
        {
            RecordHeader rh = new RecordHeader();
            rh.ReadData(binaryReader);

            int tl = System.Convert.ToInt32(rh.TagLength);
            int count = binaryReader.ReadUInt16();
            for(int i=0;i<count;i++)
            {
                int id = binaryReader.ReadUInt16();
                string name = binaryReader.ReadString(Encoding.UTF8);
                tagsData.Add(id, name);
            }
        }

        public string GetCharcterIdName(int id)
        {
            if(tagsData.ContainsKey(id))
            {
                return tagsData[id];
            }
            return null;
        }
    }
}
