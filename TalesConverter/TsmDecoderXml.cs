using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TalesConverter.Properties;
using TalesSharp;

namespace TalesConverter
{
    public static class TsmDecoderXml
    {
        #region Members
        private static XmlDocument cfg = new XmlDocument();
        private const string CFG_FILE = "header.xml";
        private static ConcurrentDictionary<string,TsmDecoder> decoders = new ConcurrentDictionary<string, TsmDecoder>();
        #endregion

        #region Static
        static TsmDecoderXml()
        {
            if (!File.Exists(CFG_FILE))
            {
                File.WriteAllText(CFG_FILE, Resources.header);
            }
            cfg.Load(CFG_FILE);
        }
        #endregion

        #region Decoder
        public static bool WriteDecoder(string name, int start, int pattern, int pass, int salt)
        {
            XmlNode node = cfg.SelectSingleNode(String.Format("/Headers/Decoder[@name='{0}']", name));
            if (node == null)
            {
                XmlNode decoder = cfg.CreateElement("Decoder");

                XmlAttribute attr = cfg.CreateAttribute("name");
                attr.Value = name;
                decoder.Attributes.Append(attr);

                XmlAttribute attr1 = cfg.CreateAttribute("start");
                attr1.Value = start.ToString();
                decoder.Attributes.Append(attr1);

                XmlAttribute attr2 = cfg.CreateAttribute("pattern");
                attr2.Value = pattern.ToString();
                decoder.Attributes.Append(attr2);

                XmlAttribute attr3 = cfg.CreateAttribute("password");
                attr3.Value = pass.ToString();
                decoder.Attributes.Append(attr3);

                XmlAttribute attr4 = cfg.CreateAttribute("salt");
                attr4.Value = salt.ToString();
                decoder.Attributes.Append(attr4);

                cfg.DocumentElement.AppendChild(decoder);
                cfg.Save(CFG_FILE);
                return true;
            }
            else
            {
                return false;
            }  
        }

        public static TsmDecoder ReadDecoder(string name)
        {
            if(decoders.ContainsKey(name))
            {
                return decoders[name];
            }
            else
            {
                XmlNode node = cfg.SelectSingleNode(String.Format("/Headers/Decoder[@name='{0}']", name));
                if (node != null && IsValidDecoderNode(node))
                {
                    var decoder = new TsmDecoder();
                    decoder.Start = int.Parse(node.Attributes["start"].Value);
                    decoder.Pattern = int.Parse(node.Attributes["pattern"].Value);
                    decoder.Password = int.Parse(node.Attributes["password"].Value);
                    decoder.Salt = int.Parse(node.Attributes["salt"].Value);
                    decoders.TryAdd(name, decoder);
                    return decoder;
                }
            }
            return null;
        }

        private static bool IsValidDecoderNode(XmlNode node)
        {
            if (node.Attributes["start"] != null)
                if (node.Attributes["pattern"] != null)
                    if (node.Attributes["password"] != null)
                        if (node.Attributes["salt"] != null)
                            return true;
            return false;
        }
        #endregion

        #region Header
        public static bool WriteHeader(byte[] buffer, string decodername, bool zip, bool encoded)
        {
            XmlNode node = cfg.SelectSingleNode(String.Format("/Headers/Header[@target='{0}']", String.Join(" ", buffer)));
            if (node == null && buffer.Length == 3)
            {
                XmlNode header = cfg.CreateElement("Header");

                XmlAttribute attr = cfg.CreateAttribute("target");
                attr.Value = String.Join(" ", buffer);
                header.Attributes.Append(attr);

                XmlAttribute attr1 = cfg.CreateAttribute("decoder");
                attr1.Value = decodername;
                header.Attributes.Append(attr1);

                XmlAttribute attr2 = cfg.CreateAttribute("zip");
                attr2.Value = zip.ToString();
                header.Attributes.Append(attr2);

                XmlAttribute attr3 = cfg.CreateAttribute("encoded");
                attr3.Value = encoded.ToString();
                header.Attributes.Append(attr3);

                cfg.DocumentElement.AppendChild(header);
                cfg.Save(CFG_FILE);
                return true;
            }
            return false;
        }

        public static TsmDecoder ReadHeader(byte[] header, out bool iszip, out bool isencoded)
        {
            Console.WriteLine(String.Join(" ", header));
            XmlNode node = cfg.SelectSingleNode(String.Format("/Headers/Header[@target='{0}']", String.Join(" ", header)));
            if (node != null && IsValidHeaderNode(node))
            {
                Console.WriteLine("dddd");
                var decoder = ReadDecoder(node.Attributes["decoder"].Value);
                if(decoder != null)
                {
                    iszip = bool.Parse(node.Attributes["zip"].Value);
                    isencoded = bool.Parse(node.Attributes["encoded"].Value);
                    return decoder;
                }
            }

            iszip = false;
            isencoded = false;
            return null;
        }

        private static bool IsValidHeaderNode(XmlNode node)
        {
            if (node.Attributes["target"] != null)
                if (node.Attributes["decoder"] != null)
                    if (node.Attributes["zip"] != null)
                        if (node.Attributes["encoded"] != null)
                            return true;
            return false;
        }
        #endregion

        #region Swap
        public static void Swap(ref byte[] buffer, int length)
        {
            var head = new byte[length];
            for (int i = 0; i < length; i++)
                head[i] = buffer[i];

            var node = cfg.SelectSingleNode(String.Format("/Headers/Swap[@target='{0}']", String.Join(" ", head)));
            if (node != null)
            {
                var toswap = node.Attributes["to"].Value.Split(' ');
                if (toswap.Length == length)
                {
                    for (int i = 0; i < length; i++)
                    {
                        buffer[i] = byte.Parse(toswap[i]);
                    }
                }
            }
        }
        #endregion
    }
}
