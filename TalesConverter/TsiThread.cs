using SwfDotNet.IO;
using SwfDotNet.IO.Tags;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TalesConverter
{
    public class ImageContainer
    {
        public string Name { get; set; }
        private string saveurl = null;
        public string SaveUrl {
            get
            {
                return saveurl;
            }
            set
            {
                if (saveurl == null) saveurl = value;
            }
        }
    }

    public class TsiThread : IDisposable
    {
        public List<ImageContainer> ImgPNG = new List<ImageContainer>();
        public List<ImageContainer> ImgJPG = new List<ImageContainer>();
        public List<DefineBitsJpeg2Tag> jpeg2tags = new List<DefineBitsJpeg2Tag>();
        public List<DefineBitsJpeg3Tag> jpeg3tags = new List<DefineBitsJpeg3Tag>();
        SwfReader swfReader = null;

        public SymbolClass symbols = null;

        public int MaxThread;

        public TsiThread(int maxthread)
        {
            MaxThread = maxthread;
            new WeakReference(ImgPNG);
            new WeakReference(ImgJPG);
            new WeakReference(jpeg2tags);
            new WeakReference(jpeg3tags);
        }

        public void Dispose()
        {
            ImgPNG = null;
            ImgJPG = null;
            jpeg3tags = null;
            jpeg2tags = null;
            symbols = null;
        }

        public void ExtractFilesFromSWF(object obj)
        {
            string file;
            if (!obj.GetType().Equals(typeof(string)))
                return;
            file = (string)obj;


            swfReader = new SwfReader(file);
            new WeakReference(swfReader);
            Swf swf = swfReader.ReadSwf();
            swfReader.Close();
            swfReader = null;


            foreach (BaseTag tag in swf.Tags)
            {
                if (tag is SymbolClass)
                {
                    symbols = tag as SymbolClass;
                    new WeakReference(symbols);
                }
            }

            foreach (BaseTag tag in swf.Tags)
            {
                if (tag is DefineBitsJpeg3Tag)
                {
                    DefineBitsJpeg3Tag imgTag = tag as DefineBitsJpeg3Tag;
                    jpeg3tags.Add(imgTag);
                }
                else if (tag is DefineBitsJpeg2Tag)
                {
                    DefineBitsJpeg2Tag imgTag = tag as DefineBitsJpeg2Tag;
                    jpeg2tags.Add(imgTag);
                }
            }

            Parallel.ForEach(jpeg3tags, new ParallelOptions { MaxDegreeOfParallelism = MaxThread }, tag =>
            {
                ParallelJpeg3(tag);
            });

            Parallel.ForEach(jpeg2tags, new ParallelOptions { MaxDegreeOfParallelism = MaxThread }, tag =>
            {
                ParallelJpeg2(tag);
            });
        }

        public void ParallelJpeg2(DefineBitsJpeg2Tag tag)
        {
            var ic = new ImageContainer();
            ic.SaveUrl = Path.GetTempFileName();
            tag.DecompileToFile(ic.SaveUrl);
            if (symbols != null)
            {
                ic.Name = symbols.GetCharcterIdName(tag.CharacterId);
            }
            ImgJPG.Add(ic);
        }

        public void ParallelJpeg3(DefineBitsJpeg3Tag tag)
        {
            var ic = new ImageContainer();
            ic.SaveUrl = Path.GetTempFileName();
            tag.DecompileToFile(ic.SaveUrl);
            if (symbols != null)
            {
                ic.Name = symbols.GetCharcterIdName(tag.CharacterId);
            }
            ImgPNG.Add(ic);
        }
    }
}
