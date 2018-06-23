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

namespace TalesConverter.Thread
{
    public class ImageContainer
    {
        public string Name { get; set; }
        public string SWFFileName { get; set; }
        public DefineBitsJpeg3Tag PngTag = null;
        public DefineBitsJpeg2Tag JpegTag = null;
    }

    public class TsiThread : IDisposable
    {
        public List<ImageContainer> ImgPNG = new List<ImageContainer>();
        public List<ImageContainer> ImgJPG = new List<ImageContainer>();
        SwfReader swfReader = null;

        public SymbolClass symbols = null;

        public int MaxThread;

        public TsiThread(int maxthread)
        {
            MaxThread = maxthread;
        }

        public void Dispose()
        {
            ImgPNG = null;
            ImgJPG = null;
            symbols = null;
        }

        public void ExtractFilesFromSWF(object obj)
        {
            string file;
            if (!obj.GetType().Equals(typeof(string)))
                return;
            file = (string)obj;

            swfReader = new SwfReader(file);
            Swf swf = swfReader.ReadSwf();
            swfReader.Close();
            swfReader = null;

            foreach (BaseTag tag in swf.Tags)
            {
                if (tag is SymbolClass)
                {
                    symbols = tag as SymbolClass;
                    break;
                }
            }

            foreach (BaseTag tag in swf.Tags)
            {
                if (tag is DefineBitsJpeg3Tag)
                {
                    DefineBitsJpeg3Tag imgTag = tag as DefineBitsJpeg3Tag;
                    var ic = new ImageContainer();
                    ic.SWFFileName = Path.GetFileNameWithoutExtension(file);

                    if (symbols != null) ic.Name = symbols.GetCharcterIdName(imgTag.CharacterId);
                    else ic.Name = imgTag.CharacterId.ToString();

                    ic.PngTag = imgTag;
                    ImgPNG.Add(ic);
                }
                else if (tag is DefineBitsJpeg2Tag)
                {
                    DefineBitsJpeg2Tag imgTag = tag as DefineBitsJpeg2Tag;
                    var ic = new ImageContainer();
                    ic.SWFFileName = Path.GetFileNameWithoutExtension(file);

                    if (symbols != null) ic.Name = symbols.GetCharcterIdName(imgTag.CharacterId);
                    else ic.Name = imgTag.CharacterId.ToString();

                    ic.JpegTag = imgTag;
                    ImgJPG.Add(ic);
                }
            }
        }
    }
}
