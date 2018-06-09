using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3Validate
{
    public class MP3Header
    {
        public bool IsValid { get; private set; }
        public int Bitrate { get; private set; }
        public int Frequency { get; private set; }
        public bool Padding { get; private set; }
        public MpegVersion Version { get; private set; } = MpegVersion.MPEG2;
        public MpegLayer Layer { get; private set; } = MpegLayer.Layer3;

        public int? AAUSize
        {
            get
            {
                if(Frequency != 0 && Bitrate != 0)
                {
                    return 144 * Bitrate * 1000 / Frequency + (Padding ? 1 : 0);
                }
                return null;
            }
        }

        public MP3Header(byte[] buffer)
        {
            if (buffer.Length >= 4)
            {
                byte one = buffer[0];
                byte two = buffer[1];
                byte three = buffer[2];
                if (CompBit(one, 1, 8, true, true, true, true, true, true, true, true))
                {
                    if ((Version = GetMpegVersion(two)) == MpegVersion.Reserved)
                        IsValid = false;
                    if ((Layer = GetMpegLayer(two)) == MpegLayer.Reserved)
                        IsValid = false;
                    var b = GetBitrate(three, Version, Layer);
                    var f = GetFreqency(three, Version);
                    if (b.HasValue && f.HasValue)
                    {
                        Bitrate = b.Value;
                        Frequency = f.Value;
                        IsValid = true;
                    }
                    Padding = GetBit(three, 7);
                }
            }
        }

        private static MpegVersion GetMpegVersion(byte two)
        {
            if (CompBit(two, 4, 2, false, false)) return MpegVersion.MPEG25;
            if (CompBit(two, 4, 2, false, true)) return MpegVersion.Reserved;
            if (CompBit(two, 4, 2, true, false)) return MpegVersion.MPEG2;
            if (CompBit(two, 4, 2, true, true)) return MpegVersion.MPEG1;
            return MpegVersion.Reserved;
        }

        private static MpegLayer GetMpegLayer(byte two)
        {
            if (CompBit(two, 6, 2, false, false)) return MpegLayer.Reserved;
            if (CompBit(two, 6, 2, false, true)) return MpegLayer.Layer3;
            if (CompBit(two, 6, 2, true, false)) return MpegLayer.Layer2;
            if (CompBit(two, 6, 2, true, true)) return MpegLayer.Layer1;
            return MpegLayer.Reserved;
        }

        private static int? GetBitrate(byte three, MpegVersion version, MpegLayer layer)
        {
            if(version == MpegVersion.MPEG1)
            {
                if(layer == MpegLayer.Layer1)
                {
                    if (CompBit(three, 1, 4, false, false, false, false)) return null;
                    if (CompBit(three, 1, 4, false, false, false, true)) return 32;
                    if (CompBit(three, 1, 4, false, false, true, false)) return 64;
                    if (CompBit(three, 1, 4, false, false, true, true)) return 96;
                    if (CompBit(three, 1, 4, false, true, false, false)) return 128;
                    if (CompBit(three, 1, 4, false, true, false, true)) return 160;
                    if (CompBit(three, 1, 4, false, true, true, false)) return 192;
                    if (CompBit(three, 1, 4, false, true, true, true)) return 224;
                    if (CompBit(three, 1, 4, true, false, false, false)) return 256;
                    if (CompBit(three, 1, 4, true, false, false, true)) return 288;
                    if (CompBit(three, 1, 4, true, false, true, false)) return 320;
                    if (CompBit(three, 1, 4, true, false, true, true)) return 352;
                    if (CompBit(three, 1, 4, true, true, false, false)) return 384;
                    if (CompBit(three, 1, 4, true, true, false, true)) return 416;
                    if (CompBit(three, 1, 4, true, true, true, false)) return 448;
                    if (CompBit(three, 1, 4, true, true, true, true)) return null;
                }
                else if(layer == MpegLayer.Layer2)
                {
                    if (CompBit(three, 1, 4, false, false, false, false)) return null;
                    if (CompBit(three, 1, 4, false, false, false, true)) return 32;
                    if (CompBit(three, 1, 4, false, false, true, false)) return 45;
                    if (CompBit(three, 1, 4, false, false, true, true)) return 56;
                    if (CompBit(three, 1, 4, false, true, false, false)) return 64;
                    if (CompBit(three, 1, 4, false, true, false, true)) return 80;
                    if (CompBit(three, 1, 4, false, true, true, false)) return 96;
                    if (CompBit(three, 1, 4, false, true, true, true)) return 112;
                    if (CompBit(three, 1, 4, true, false, false, false)) return 128;
                    if (CompBit(three, 1, 4, true, false, false, true)) return 160;
                    if (CompBit(three, 1, 4, true, false, true, false)) return 192;
                    if (CompBit(three, 1, 4, true, false, true, true)) return 224;
                    if (CompBit(three, 1, 4, true, true, false, false)) return 256;
                    if (CompBit(three, 1, 4, true, true, false, true)) return 320;
                    if (CompBit(three, 1, 4, true, true, true, false)) return 384;
                    if (CompBit(three, 1, 4, true, true, true, true)) return null;
                }
                else if(layer == MpegLayer.Layer3)
                {
                    if (CompBit(three, 1, 4, false, false, false, false)) return null;
                    if (CompBit(three, 1, 4, false, false, false, true)) return 32;
                    if (CompBit(three, 1, 4, false, false, true, false)) return 40;
                    if (CompBit(three, 1, 4, false, false, true, true)) return 45;
                    if (CompBit(three, 1, 4, false, true, false, false)) return 56;
                    if (CompBit(three, 1, 4, false, true, false, true)) return 64;
                    if (CompBit(three, 1, 4, false, true, true, false)) return 80;
                    if (CompBit(three, 1, 4, false, true, true, true)) return 96;
                    if (CompBit(three, 1, 4, true, false, false, false)) return 112;
                    if (CompBit(three, 1, 4, true, false, false, true)) return 128;
                    if (CompBit(three, 1, 4, true, false, true, false)) return 160;
                    if (CompBit(three, 1, 4, true, false, true, true)) return 192;
                    if (CompBit(three, 1, 4, true, true, false, false)) return 224;
                    if (CompBit(three, 1, 4, true, true, false, true)) return 256;
                    if (CompBit(three, 1, 4, true, true, true, false)) return 320;
                    if (CompBit(three, 1, 4, true, true, true, true)) return null;
                }
            }
            return null;
        }

        private static int? GetFreqency(byte three, MpegVersion version)
        {
            int basevalue = 0;
            if (CompBit(three, 5, 2, false, false)) basevalue = 11025;
            if (CompBit(three, 5, 2, false, true)) basevalue = 12000;
            if (CompBit(three, 5, 2, true, false)) basevalue = 8000;
            if (CompBit(three, 5, 2, true, true)) basevalue = 0;

            if(basevalue != 0)
            {
                if (version == MpegVersion.MPEG25) return basevalue;
                if (version == MpegVersion.MPEG2) return basevalue * 2;
                if (version == MpegVersion.MPEG1) return basevalue * 4;
            }
            return null;
        }

        private static bool GetBit(byte b, int bitNumber)
        {
            return (b & (128 >> bitNumber-1)) != 0;
        }

        private static bool CompBit(byte b, int start, int length, params bool[] bit)
        {
            for(int i=0;i<length;i++)
            {
                if (GetBit(b, start+i) != bit[i])
                    return false;
            }
            return true;
        }
    }

    public class MP3HeaderCollection : ICollection<MP3Header>
    {
        ICollection<MP3Header> _items;

        public int Count {
            get
            {
                return _items.Count;
            }
        }
        public bool IsReadOnly {
            get
            {
                return _items.IsReadOnly;
            }
        }

        public MP3HeaderCollection()
        {
            // Default to using a List<T>.
            _items = new List<MP3Header>();
        }
        
        public void Add(MP3Header item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(MP3Header item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(MP3Header[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<MP3Header> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public bool Remove(MP3Header item)
        {
            return _items.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
