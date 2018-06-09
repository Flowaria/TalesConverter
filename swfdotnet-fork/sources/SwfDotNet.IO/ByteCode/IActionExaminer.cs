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

using System;
using SwfDotNet.IO.ByteCode.Actions;

namespace SwfDotNet.IO.ByteCode
{	
	/// <summary>Interface for visitor-like objects passed to <see cref="SwfDotNet.IO.ByteCode.CodeTraverser"/>
	/// or <see cref="SwfDotNet.IO.ByteCode.CodeWalker"/>
	/// </summary>
	public interface IActionExaminer {		
		
		/// <summary>Invoked by traverser for each visited action.
		/// Examine action object found at given index.
		/// </summary>
		void Examine(int index,BaseAction action);
		
		/// <summary>Clone IActionExaminer object. Necessary for handling branches.</summary>
		IActionExaminer Clone();
	}
}
