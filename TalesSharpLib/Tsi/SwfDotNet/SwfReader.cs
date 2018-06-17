/*
	SwfDotNet is an open source library for writing and reading 
	Macromedia Flash (SWF) bytecode.
	Copyright (C) 2005 Olivier Carpentier - Adelina foundation
	see Licence.cs for GPL full text!
		
	SwfDotNet.IO uses a part of the open source library SwfOp actionscript 
	byte code management, writted by Florian Krüsch, Copyright (C) 2004 .
	
	This library is free software; you can redistribute it and/or
	modify it under the terms of the GNU General Public
	License as published by the Free Software Foundation; either
	version 2.1 of the License, or (at your option) any later version.
	
	This library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
	General Public License for more details.
	
	You should have received a copy of the GNU General Public
	License along with this library; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/
using System;
using System.IO;
using System.Collections;

using SwfDotNet.IO.Tags;
using SwfDotNet.IO.Utils;

namespace SwfDotNet.IO 
{
	/// <summary>
	/// The SwfReader class reads and parses a swf file from a stream. 
	/// </summary>
    /// <remarks>
    /// <p>
	/// The SwfReader creates a <see cref="Swf"/> object, that is a sequence
	/// of tags readed from a swf binary file. The tag sequences follows the
	/// Swf version 7 format specifications from Macromedia, and is compatible
	/// with Swf version 1 to 7.
	/// </p>
	/// <p>
	/// The working process of the tags sequence is explain in the Macromedia
	/// specifications. The tags sequence is composed of objects from the
	/// <see cref="SwfDotNet.IO.Tags"/> namespace.
	/// </p>
	/// <p>
	/// SwfReader gets all the content of a file and provides some functionnalities
	/// to decompile bitmap, sounds, video or scripting bytecode.
	/// </p>
	/// <p>
	/// The tag reading process doesnt decompile actionscript bytecode, though. This is 
	/// handled by <see cref="SwfDotNet.IO.ByteCode.Decompiler"/>. The action script 
	/// tags contain directly actionscript bytecodes as a byte array. The
	/// <see cref="SwfDotNet.IO.ByteCode.Decompiler"/> object provides the way to
	/// get actionscript command as objects of the <see cref="SwfDotNet.IO.ByteCode"/> namespace.
	/// </p>
	/// </remarks>
	/// <example>
	/// <p>
	/// <u>Read a swf file from a file, and display headers.</u>
	/// <code lang="C#">
	/// SwfReader swfReader = new SwfReader("myfile.swf");
	/// Swf swf = swfReader.ReadSwf();
	/// Console.WriteLine(swf.Version);
	/// swfReader.Close();
	/// </code>
	/// </p>
	/// </example>
	public class SwfReader 
	{
		#region Members

		private byte version = 0;
		private BufferedBinaryReader br;
		private BaseTagCollection tagList = null;
	
		#endregion

		#region Ctor

		/// <summary>
		/// Swf Reader class, takes an input stream as single argument.
		/// </summary>
		/// <param name="stream">Stream to read swf from, must allow random access</param>
		public SwfReader(Stream stream) 
		{				
			this.br = new BufferedBinaryReader(stream);
		}

		/// <summary>
		/// Creates a new <see cref="SwfReader"/> instance.
		/// </summary>
		/// <param name="path">Path.</param>
		public SwfReader(string path)
		{
			Init(path, false);
		}

        /// <summary>
        /// Creates a new <see cref="SwfReader"/> instance.
        /// If useBuffer is true, all the content of 
        /// the SWF file is readed first and is parsed from the memory
        /// after. If useBuffer is false, the SWF is parsed directly from
        /// the file stream. Use a buffer is faster to parse, but use
        /// more memory.
        /// </summary>
        /// <param name="path">String path of the local swf file</param>
        /// <param name="useBuffer">Use buffer.</param>
        public SwfReader(string path, bool useBuffer)
        {
            Init(path, useBuffer);
        }

        /// <summary>
        /// Inits the stream reading process.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="useBuffer">Use buffer.</param>
        private void Init(string path, bool useBuffer)
        {
            Stream stream = File.OpenRead(path);
            if (useBuffer)
            {
                FileInfo fi = new FileInfo(path);
                
                byte[] buff = new byte[fi.Length];
                stream.Read(buff, 0, System.Convert.ToInt32(fi.Length));
                stream.Close();

                MemoryStream ms = new MemoryStream(buff);
                this.br = new BufferedBinaryReader(ms);
            }
            else
            {
                this.br = new BufferedBinaryReader(stream);
            }
        }

		#endregion

		#region Properties

		/// <summary>
		/// Read only property; returns Flash version of the read swf.
		/// </summary>
		public byte Version 
		{
			get {
				return version;
			}
		}

		#endregion

		#region Methods
        
        #region Methods: Close

        /// <summary>
        /// Closes this stream reader.
        /// </summary>
        public void Close()
        {
            this.br.Close();
        }

        #endregion

		#region Methods: ReadSwf

		/// <summary>
		/// Read swf (header and tags), this is the only 
		/// public method of <see cref="SwfDotNet.IO.SwfReader">SwfReader</see>
		/// with <see cref="SwfDotNet.IO.SwfReader.Close">Close</see> and
		/// <see cref="SwfDotNet.IO.SwfReader.ReadSwfHeader">ReadSwfHeader</see> methods.
		/// The returned <see cref="SwfDotNet.IO.Swf">Swf</see> object contains swf headers informations and the
		/// tags list.
		/// </summary>
		public Swf ReadSwf() 
		{
			// compressed swf?
			if (br.PeekChar()=='C') 
				Inflate();				
			
			SwfHeader header = new SwfHeader();
            header.ReadData(br);
			this.version = header.Version;

			tagList = new BaseTagCollection();
			
            bool readEndTag = false; //necessary for the 1 more byte bug
			while (br.BaseStream.Position < br.BaseStream.Length && !readEndTag) 
			{
				BaseTag b = SwfReader.ReadTag(this.version, this.br, this.tagList);
                if (b != null)
                {
                    if (b is EndTag)
                        readEndTag = true;
                    tagList.Add(b);
                }
			};
			
			br.Close();
			
			return new Swf(header, tagList);
		}

        /// <summary>
        /// Reads the SWF header only.
        /// This method don't read the complete content of the 
        /// SWF file. Then, it provides the possibility to
        /// get faster header informations, and only it.
        /// </summary>
        /// <returns></returns>
        public SwfHeader ReadSwfHeader()
        {
            // compressed swf?
            if (br.PeekChar()=='C') 
                Inflate();				
			
            SwfHeader header = new SwfHeader();
            header.ReadData(br);
            this.version = header.Version;   

            tagList = new BaseTagCollection();
			br.Close();
            return header;
        }

		#endregion

		#region Methods: Inflate Compressed Swf
		
		/// <summary>
		/// Inflate compressed swf
		/// </summary>
		private void Inflate() 
		{
			// read size
			br.BaseStream.Position = 4; // skip signature
			int size = Convert.ToInt32(br.ReadUInt32());
			
			// read swf head
			byte[] uncompressed = new byte[size];			
			br.BaseStream.Position = 0;
			br.Read(uncompressed,0,8); // header data is not compress											
			
			// un-zip
			byte[] compressed = br.ReadBytes(size);
            uncompressed = DeflatorWraper.Decompress(compressed);				
			
			// new memory stream for uncompressed swf
			MemoryStream m = new MemoryStream(uncompressed);
			br = new BufferedBinaryReader(m);
			br.BaseStream.Position = 0;
		}

		#endregion
        
        /// <summary>
        /// Read next tag from swf input stream.
        /// </summary>
        /// <param name="version">Version.</param>
        /// <param name="binaryReader">Binary reader.</param>
        /// <param name="tagList">Tag list.</param>
        /// <returns></returns>
		internal static BaseTag ReadTag(byte version, BufferedBinaryReader binaryReader, BaseTagCollection tagList) 
		{
			long posBefore = binaryReader.BaseStream.Position;
			RecordHeader rh = new RecordHeader();
            rh.ReadData(binaryReader);

			int offset = (int)(binaryReader.BaseStream.Position - posBefore);
			binaryReader.BaseStream.Position = posBefore;
			
			BaseTag resTag = null;
			            
			switch (rh.TagCode)
			{
				case (int)TagCodeEnum.DefineBitsJpeg2: resTag = new DefineBitsJpeg2Tag(); break;
				case (int)TagCodeEnum.DefineBitsJpeg3: resTag = new DefineBitsJpeg3Tag(); break;
				case (int)TagCodeEnum.DefineBitsLossLess: resTag = new DefineBitsLossLessTag(); break;
				case (int)TagCodeEnum.DefineBitsLossLess2: resTag = new DefineBitsLossLess2Tag(); break;
				case (int)TagCodeEnum.End: resTag = new EndTag(); break;
                case (int)TagCodeEnum.SymbolClass: resTag = new SymbolClass(); break;
				default: resTag = new BaseTag(binaryReader.ReadBytes(System.Convert.ToInt32(rh.TagLength + offset))); break;
			}

            //Read the data of the current tag
            resTag.ReadData(version, binaryReader);

			//LOG
            long mustRead = rh.TagLength + offset;
			if (posBefore + mustRead != binaryReader.BaseStream.Position)
			{
				binaryReader.BaseStream.Position = posBefore + rh.TagLength + offset;
			}
			
			return resTag;
		}

		#region Methods: ReadVideoFrameTag

		/// <summary>
		/// Read and parse VideoFrameTag, into inner tags and raw byte-array header data
		/// </summary>
		private static VideoFrameTag ReadVideoFrameTag(BufferedBinaryReader binaryReader, BaseTagCollection tagList)
		{
			VideoFrameTag video = new VideoFrameTag();
			ushort streamId = binaryReader.PeekUInt16();

            IEnumerator tags = tagList.GetEnumerator();
            while (tags.MoveNext())
            {
                BaseTag tag = (BaseTag)tags.Current;
                if (tag is DefineVideoStreamTag)
                {
                    if (((DefineVideoStreamTag)tag).CharacterId == streamId)
                        video.CodecId = ((DefineVideoStreamTag)tag).CodecId;
                }
            }
			return video;
		}

		#endregion

		#endregion
	}
}
