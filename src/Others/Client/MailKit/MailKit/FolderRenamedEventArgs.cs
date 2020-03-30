﻿//
// FolderRenamedEventArgs.cs
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
	/// Event args used when a <see cref="IMailFolder"/> is renamed.
	/// </summary>
	/// <remarks>
	/// Event args used when a <see cref="IMailFolder"/> is renamed.
	/// </remarks>
	public class FolderRenamedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MailKit.FolderRenamedEventArgs"/> class.
		/// </summary>
		/// <remarks>
		/// Creates a new <see cref="FolderRenamedEventArgs"/>.
		/// </remarks>
		/// <param name="oldName">The old name of the folder.</param>
		/// <param name="newName">The new name of the folder.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="oldName"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="newName"/> is <c>null</c>.</para>
		/// </exception>
		public FolderRenamedEventArgs (string oldName, string newName)
		{
			if (oldName == null)
				throw new ArgumentNullException (nameof (oldName));

			if (newName == null)
				throw new ArgumentNullException (nameof (newName));

			OldName = oldName;
			NewName = newName;
		}

		/// <summary>
		/// The old name of the folder.
		/// </summary>
		/// <remarks>
		/// The old name of the folder.
		/// </remarks>
		/// <value>The old name.</value>
		public string OldName {
			get; private set;
		}

		/// <summary>
		/// The new name of the folder.
		/// </summary>
		/// <remarks>
		/// The new name of the folder.
		/// </remarks>
		/// <value>The new name.</value>
		public string NewName {
			get; private set;
		}
	}
}
