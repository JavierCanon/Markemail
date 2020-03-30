﻿//
// Socks4ClientTests.cs
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

// Note: Find Socks4 proxy list here: https://hideip.me/en/proxy/socks4list

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using NUnit.Framework;

using MailKit.Net.Proxy;

namespace UnitTests.Net.Proxy {
	[TestFixture]
	public class Socks4ClientTests
	{
		const int ConnectTimeout = 5 * 1000; // 5 seconds

		[Test]
		public void TestArgumentExceptions ()
		{
			var credentials = new NetworkCredential ("user", "password");
			var socks = new Socks4Client ("socks4.proxy.com", 0, credentials);

			Assert.Throws<ArgumentNullException> (() => new Socks4Client (null, 1080));
			Assert.Throws<ArgumentException> (() => new Socks4Client (string.Empty, 1080));
			Assert.Throws<ArgumentOutOfRangeException> (() => new Socks4Client (socks.ProxyHost, -1));
			Assert.Throws<ArgumentNullException> (() => new Socks4Client (socks.ProxyHost, 1080, null));

			Assert.AreEqual (4, socks.SocksVersion);
			Assert.AreEqual (1080, socks.ProxyPort);
			Assert.AreEqual ("socks4.proxy.com", socks.ProxyHost);
			Assert.AreEqual (credentials, socks.ProxyCredentials);

			Assert.Throws<ArgumentNullException> (() => socks.Connect (null, 80));
			Assert.Throws<ArgumentNullException> (() => socks.Connect (null, 80, ConnectTimeout));
			Assert.Throws<ArgumentNullException> (async () => await socks.ConnectAsync (null, 80));
			Assert.Throws<ArgumentNullException> (async () => await socks.ConnectAsync (null, 80, ConnectTimeout));

			Assert.Throws<ArgumentException> (() => socks.Connect (string.Empty, 80));
			Assert.Throws<ArgumentException> (() => socks.Connect (string.Empty, 80, ConnectTimeout));
			Assert.Throws<ArgumentException> (async () => await socks.ConnectAsync (string.Empty, 80));
			Assert.Throws<ArgumentException> (async () => await socks.ConnectAsync (string.Empty, 80, ConnectTimeout));

			Assert.Throws<ArgumentOutOfRangeException> (() => socks.Connect ("www.google.com", 0));
			Assert.Throws<ArgumentOutOfRangeException> (() => socks.Connect ("www.google.com", 0, ConnectTimeout));
			Assert.Throws<ArgumentOutOfRangeException> (async () => await socks.ConnectAsync ("www.google.com", 0));
			Assert.Throws<ArgumentOutOfRangeException> (async () => await socks.ConnectAsync ("www.google.com", 0, ConnectTimeout));

			Assert.Throws<ArgumentOutOfRangeException> (() => socks.Connect ("www.google.com", 80, -ConnectTimeout));
			Assert.Throws<ArgumentOutOfRangeException> (async () => await socks.ConnectAsync ("www.google.com", 80, -ConnectTimeout));
		}

		static string ResolveIPv4 (string host)
		{
			var ipAddresses = Dns.GetHostAddresses (host);

			for (int i = 0; i < ipAddresses.Length; i++) {
				if (ipAddresses[i].AddressFamily == AddressFamily.InterNetwork)
					return ipAddresses[i].ToString ();
			}

			return null;
		}

		[Test]
		public void TestConnectByIPv4 ()
		{
			using (var proxy = new Socks4ProxyListener ()) {
				proxy.Start (IPAddress.Loopback, 0);

				var socks = new Socks4Client (proxy.IPAddress.ToString (), proxy.Port);
				var host = "74.125.197.99"; // ResolveIPv4 ("www.google.com");
				Socket socket = null;

				if (host == null)
					return;

				try {
					socket = socks.Connect (host, 80, ConnectTimeout);
					socket.Disconnect (false);
				} catch (TimeoutException) {
					Assert.Inconclusive ("Timed out.");
				} catch (Exception ex) {
					Assert.Fail (ex.Message);
				} finally {
					if (socket != null)
						socket.Dispose ();
				}
			}
		}

		[Test]
		public async Task TestConnectByIPv4Async ()
		{
			using (var proxy = new Socks4ProxyListener ()) {
				proxy.Start (IPAddress.Loopback, 0);

				var socks = new Socks4Client (proxy.IPAddress.ToString (), proxy.Port);
				var host = "74.125.197.99"; // ResolveIPv4 ("www.google.com");
				Socket socket = null;

				if (host == null)
					return;

				try {
					socket = await socks.ConnectAsync (host, 80, ConnectTimeout);
					socket.Disconnect (false);
				} catch (TimeoutException) {
					Assert.Inconclusive ("Timed out.");
				} catch (Exception ex) {
					Assert.Fail (ex.Message);
				} finally {
					if (socket != null)
						socket.Dispose ();
				}
			}
		}

		[Test]
		public void TestConnectByDomain ()
		{
			using (var proxy = new Socks4ProxyListener ()) {
				proxy.Start (IPAddress.Loopback, 0);

				var socks = new Socks4Client (proxy.IPAddress.ToString (), proxy.Port);
				Socket socket = null;

				try {
					socket = socks.Connect ("www.google.com", 80, ConnectTimeout);
					socket.Disconnect (false);
				} catch (TimeoutException) {
					Assert.Inconclusive ("Timed out.");
				} catch (Exception ex) {
					Assert.Fail (ex.Message);
				} finally {
					if (socket != null)
						socket.Dispose ();
				}
			}
		}

		[Test]
		public async Task TestConnectByDomainAsync ()
		{
			using (var proxy = new Socks4ProxyListener ()) {
				proxy.Start (IPAddress.Loopback, 0);

				var socks = new Socks4Client (proxy.IPAddress.ToString (), proxy.Port);
				Socket socket = null;

				try {
					socket = await socks.ConnectAsync ("www.google.com", 80, ConnectTimeout);
					socket.Disconnect (false);
				} catch (TimeoutException) {
					Assert.Inconclusive ("Timed out.");
				} catch (Exception ex) {
					Assert.Fail (ex.Message);
				} finally {
					if (socket != null)
						socket.Dispose ();
				}
			}
		}

		[Test]
		public async Task TestTimeoutException ()
		{
			using (var proxy = new Socks4ProxyListener ()) {
				proxy.Start (IPAddress.Loopback, 0);

				var socks = new Socks4Client (proxy.IPAddress.ToString (), proxy.Port);
				Socket socket = null;

				try {
					socket = await socks.ConnectAsync ("example.com", 25, 1000);
					socket.Disconnect (false);
				} catch (TimeoutException) {
					Assert.Pass ();
				} catch (Exception ex) {
					Assert.Fail (ex.Message);
				} finally {
					if (socket != null)
						socket.Dispose ();
				}
			}
		}
	}
}
