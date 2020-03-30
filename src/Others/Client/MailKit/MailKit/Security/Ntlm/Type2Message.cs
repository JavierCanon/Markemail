﻿//
// Mono.Security.Protocol.Ntlm.Type2Message - Challenge
//
// Authors: Sebastien Pouliot <sebastien@ximian.com>
//          Jeffrey Stedfast <jeff@xamarin.com>
//
// Copyright (c) 2003 Motus Technologies Inc. (http://www.motus.com)
// Copyright (c) 2004 Novell, Inc (http://www.novell.com)
// Copyright (c) 2013-2020 Xamarin Inc. (www.xamarin.com)
//
// References
// a.	NTLM Authentication Scheme for HTTP, Ronald Tschalär
//	http://www.innovation.ch/java/ntlm.html
// b.	The NTLM Authentication Protocol, Copyright © 2003 Eric Glass
//	http://davenport.sourceforge.net/ntlm.html
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Text;
using System.Security.Cryptography;

namespace MailKit.Security.Ntlm {
	class Type2Message : MessageBase
	{
		byte[] targetInfo;
		byte[] nonce;

		public Type2Message () : base (2)
		{
			Flags = NtlmFlags.NegotiateNtlm | NtlmFlags.NegotiateUnicode /*| NtlmFlags.NegotiateAlwaysSign*/;
			nonce = new byte[8];

			using (var rng = RandomNumberGenerator.Create ())
				rng.GetBytes (nonce);
		}

		public Type2Message (byte[] message, int startIndex, int length) : base (2)
		{
			nonce = new byte[8];
			Decode (message, startIndex, length);
		}

		~Type2Message ()
		{
			if (nonce != null)
				Array.Clear (nonce, 0, nonce.Length);
		}

		public byte[] Nonce {
			get { return (byte[]) nonce.Clone (); }
			set { 
				if (value == null)
					throw new ArgumentNullException (nameof (value));

				if (value.Length != 8)
					throw new ArgumentException ("Invalid Nonce Length (should be 8 bytes).", nameof (value));

				nonce = (byte[]) value.Clone (); 
			}
		}

		public string TargetName {
			get; set;
		}

		public TargetInfo TargetInfo {
			get; set;
		}

		public byte[] EncodedTargetInfo {
			get {
				if (targetInfo != null)
					return (byte[]) targetInfo.Clone ();

				return new byte[0];
			}
		}

		void Decode (byte[] message, int startIndex, int length)
		{
			ValidateArguments (message, startIndex, length);

			Flags = (NtlmFlags) BitConverterLE.ToUInt32 (message, startIndex + 20);

			Buffer.BlockCopy (message, startIndex + 24, nonce, 0, 8);

			var targetNameLength = BitConverterLE.ToUInt16 (message, startIndex + 12);
			var targetNameOffset = BitConverterLE.ToUInt16 (message, startIndex + 16);

			if (targetNameLength > 0) {
				var encoding = (Flags & NtlmFlags.NegotiateUnicode) != 0 ? Encoding.Unicode : Encoding.UTF8;

				TargetName = encoding.GetString (message, startIndex + targetNameOffset, targetNameLength);
			}

			// The Target Info block is optional.
			if (length >= 48 && targetNameOffset >= 48) {
				var targetInfoLength = BitConverterLE.ToUInt16 (message, startIndex + 40);
				var targetInfoOffset = BitConverterLE.ToUInt16 (message, startIndex + 44);

				if (targetInfoLength > 0 && targetInfoOffset < length && targetInfoLength <= (length - targetInfoOffset)) {
					TargetInfo = new TargetInfo (message, startIndex + targetInfoOffset, targetInfoLength, (Flags & NtlmFlags.NegotiateOem) == 0);

					targetInfo = new byte[targetInfoLength];
					Buffer.BlockCopy (message, startIndex + targetInfoOffset, targetInfo, 0, targetInfoLength);
				}
			}
		}

		public override byte[] Encode ()
		{
			int targetNameOffset = 40;
			int targetInfoOffset = 48;
			byte[] targetName = null;
			int size = 40;

			if (TargetName != null) {
				var encoding = (Flags & NtlmFlags.NegotiateUnicode) != 0 ? Encoding.Unicode : Encoding.UTF8;

				targetName = encoding.GetBytes (TargetName);
				targetInfoOffset += targetName.Length;
				size += targetName.Length;
			}

			if (TargetInfo != null || targetInfo != null) {
				if (targetInfo == null)
					targetInfo = TargetInfo.Encode ((Flags & NtlmFlags.NegotiateUnicode) != 0);
				size += targetInfo.Length + 8;
				targetNameOffset += 8;
			}

			var data = PrepareMessage (size);

			// message length
			short length = (short) data.Length;
			data[16] = (byte) length;
			data[17] = (byte)(length >> 8);

			// flags
			data[20] = (byte) Flags;
			data[21] = (byte)((uint) Flags >> 8);
			data[22] = (byte)((uint) Flags >> 16);
			data[23] = (byte)((uint) Flags >> 24);

			Buffer.BlockCopy (nonce, 0, data, 24, nonce.Length);

			if (targetName != null) {
				data[12] = (byte) targetName.Length;
				data[13] = (byte)(targetName.Length >> 8);
				data[14] = (byte)targetName.Length;
				data[15] = (byte)(targetName.Length >> 8);
				data[16] = (byte) targetNameOffset;
				data[17] = (byte)(targetNameOffset >> 8);

				Buffer.BlockCopy (targetName, 0, data, targetNameOffset, targetName.Length);
			}

			if (targetInfo != null) {
				data[40] = (byte) targetInfo.Length;
				data[41] = (byte)(targetInfo.Length >> 8);
				data[42] = (byte) targetInfo.Length;
				data[43] = (byte)(targetInfo.Length >> 8);
				data[44] = (byte) targetInfoOffset;
				data[45] = (byte)(targetInfoOffset >> 8);

				Buffer.BlockCopy (targetInfo, 0, data, targetInfoOffset, targetInfo.Length);
			}

			return data;
		}
	}
}
