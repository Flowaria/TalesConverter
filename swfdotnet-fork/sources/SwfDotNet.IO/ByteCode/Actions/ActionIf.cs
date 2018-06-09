﻿/*
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

using System.IO;
using System;

namespace SwfDotNet.IO.ByteCode.Actions
{		
	/// <summary>
	/// bytecode instruction object
	/// </summary>

	public class ActionIf : MultiByteAction,IJump {
		
		// branch offset
		private int _offset;
		private int _labelId;
		
		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="offs">branch offset</param>
		public ActionIf(int offs):base(ActionCode.If)
		{
			_offset = offs;
			_labelId = 666;
		}
		
		/// <summary>
		/// branch offset
		/// </summary>
		public int Offset {
			get {
				return _offset;
			}
			set {
				_offset = value;
			}
		}
		
		/// <summary>
		/// label id
		/// </summary>
		public int LabelId {
			get {
				return _labelId;
			}
			set {
				_labelId = value;
			}
		}
		
		/// <see cref="SwfDotNet.IO.ByteCode.Actions.BaseAction.ByteCount"/>
		public override int ByteCount {
			get {
				return 5;
			}
		}
		
		/// <see cref="SwfDotNet.IO.ByteCode.Actions.BaseAction.PopCount"/>
		public override int PopCount {	get { return 1; } }
		
		/// <see cref="SwfDotNet.IO.ByteCode.Actions.BaseAction.PushCount"/>
		public override int PushCount { get { return 0; } }
		
		/// <see cref="SwfDotNet.IO.ByteCode.Actions.BaseAction.Compile"/>
		public override void Compile(BinaryWriter w) 
{
			base.Compile(w);
			w.Write(Convert.ToInt16(Offset));
		}
		
		/// <summary>overriden ToString method</summary>
		public override string ToString() {
			return String.Format("branchIf {0} ({1})",LabelId,Offset);
		}
	}
}
