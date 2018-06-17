using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TalesSharp
{
    public static class TsmDecoderData
    {
        private static XmlDocument cfg;
        private const string CFG_FILE = "header.xml";

        static TsmDecoderData()
        {
            cfg = new XmlDocument();

            if (File.Exists(CFG_FILE))
            {
                cfg.Load(CFG_FILE);
            } 
            else
            {
                XmlNode root = cfg.CreateElement("Headers");
                cfg.AppendChild(root);
                cfg.Save(CFG_FILE);
            }
        }

        public static bool Write(byte[] head, int start, int pattern, int pass, int salt, bool zip, bool encrypted)
        {
            if (!Exists(head))
            {
                XmlNode header = cfg.CreateElement("Header");

                XmlAttribute attr = cfg.CreateAttribute("target");
                attr.Value = string.Join(" ", head);
                header.Attributes.Append(attr);

                XmlAttribute attr1 = cfg.CreateAttribute("start");
                attr1.Value = start.ToString();
                header.Attributes.Append(attr1);

                XmlAttribute attr2 = cfg.CreateAttribute("pattern");
                attr2.Value = pattern.ToString();
                header.Attributes.Append(attr2);

                XmlAttribute attr3 = cfg.CreateAttribute("password");
                attr3.Value = pass.ToString();
                header.Attributes.Append(attr3);

                XmlAttribute attr4 = cfg.CreateAttribute("salt");
                attr4.Value = salt.ToString();
                header.Attributes.Append(attr4);

                XmlAttribute attr5 = cfg.CreateAttribute("zip");
                attr5.Value = zip.ToString();
                header.Attributes.Append(attr5);

                XmlAttribute attr6 = cfg.CreateAttribute("encoded");
                attr6.Value = encrypted.ToString();
                header.Attributes.Append(attr6);

                cfg.DocumentElement.AppendChild(header);
                cfg.Save(CFG_FILE);
                return true;
            }
            else
            {
                return false;
            }  
        }

        public static TsmDecoder Read(byte[] head)
        {
            var node = Get(string.Join(" ", head));

            if(node != null)
            {
                var decoder = new TsmDecoder();

                decoder.Start = int.Parse(node.Attributes["start"].Value);
                decoder.Pattern = int.Parse(node.Attributes["pattern"].Value);
                decoder.Password = byte.Parse(node.Attributes["password"].Value);
                decoder.Salt = byte.Parse(node.Attributes["salt"].Value);
                decoder.IsZipFile = bool.Parse(node.Attributes["zip"].Value);
                decoder.IsEncoded = bool.Parse(node.Attributes["encoded"].Value);

                return decoder;
            }
            return null;
        }

        private static bool Exists(byte[] head)
        {
            var header = string.Join(" ", head);
            foreach(XmlNode node in cfg.DocumentElement.ChildNodes)
            {
                if (node != null && node.Attributes != null)
                {
                    if (node.Attributes["target"] != null)
                    {
                        if (node.Attributes["target"].Value.Equals(header))
                        {
                            return true;
                        }
                    }
                }     
            }
            return false;
        }

        private static XmlNode Get(string header)
        {
            foreach (XmlNode node in cfg.DocumentElement.ChildNodes)
            {
                if(node != null && node.Attributes != null)
                {
                    if (node.Attributes["target"] != null)
                    {
                        if (node.Attributes["target"].Value.Equals(header))
                        {
                            return node;
                        }
                    }
                }
            }
            return null;
        }
        
        public static void Swap(ref byte[] buffer, int length)
        {
            var head = new byte[length];
            for (int i = 0; i < length; i++)
                head[i] = buffer[i];

            var node = Get(string.Join(" ", head));
            if(node != null)
            {
                var toswap = node.Attributes["to"].Value.Split(' ');
                if(toswap.Length == length)
                {
                    for(int i=0;i<length;i++)
                    {
                        buffer[i] = byte.Parse(toswap[i]);
                    }
                }
            }
        }
    }
}
