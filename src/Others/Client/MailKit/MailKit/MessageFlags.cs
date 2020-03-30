﻿//
// MessageFlags.cs
//
// Author: Jeffrey Stedfast <jestedfa@microsoft.com>
//
// Copyright (c) 2013-2020 Xamarin Inc. (www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;

namespace MailKit {
	/// <summary>
	/// An enumeration of message flags.
	/// </summary>
	/// <remarks>
	/// An enumeration of message flags.
	/// </remarks>
	[Flags]
	public enum MessageFlags {
		/// <summary>
		/// No message flags are set.
		/// </summary>
		None        = 0,

		/// <summary>
		/// The message has been read.
		/// </summary>
		Seen        = 1 << 0,

		/// <summary>
		/// The message has been answered (replied to).
		/// </summary>
		Answered    = 1 << 1,

		/// <summary>
		/// The message has been flagged for importance.
		/// </summary>
		Flagged     = 1 << 2,

		/// <summary>
		/// The message has been marked for deletion.
		/// </summary>
		Deleted     = 1 << 3,

		/// <summary>
		/// The message is marked as a draft.
		/// </summary>
		Draft       = 1 << 4,

		/// <summary>
		/// The message has just recently arrived in the folder.
		/// </summary>
		Recent      = 1 << 5,

		/// <summary>
		/// User-defined flags are allowed by the folder.
		/// </summary>
		UserDefined = 1 << 6,
	}
}
