using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3Validate
{
    public class MP3Reader
    {
        public MP3HeaderCollection Headers { get; private set; }
        private MP3Reader(byte[] content)
        {
            Headers = new MP3HeaderCollection();

            var stream = new MemoryStream(content);
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            for (int i = 0; i < stream.Length; i++)
            {
                if(buffer[i] == 255 && buffer[i+1] == 251)
                {
                    Headers.Add(new MP3Header(new byte[]{ buffer[i], buffer[i+1], buffer[i+2], buffer[i+3] }));
                    //Console.WriteLine(i);
                }
            }
        }

        public static MP3Reader ReadFile(string file)
        {
            return new MP3Reader(File.ReadAllBytes(file));
        }

        public static MP3Reader ReadBuffer(byte[] buffer)
        {
            return new MP3Reader(buffer);
        }

        public bool IsValidFile()
        {
            List<int> size = new List<int>();
            foreach (var head in Headers)
            {
                if (head.IsValid && head.AAUSize.HasValue)
                    size.Add(head.AAUSize.Value);
            }
            return !size.Any(o => o != size[0]);
        }
    }
}
