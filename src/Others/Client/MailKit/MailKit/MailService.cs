﻿//
// MailService.cs
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
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net.Security;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using SslProtocols = System.Security.Authentication.SslProtocols;

using MailKit.Net;
using MailKit.Net.Proxy;
using MailKit.Security;

namespace MailKit {
	/// <summary>
	/// An abstract mail service implementation.
	/// </summary>
	/// <remarks>
	/// An abstract mail service implementation.
	/// </remarks>
	public abstract class MailService : IMailService
	{
#if NET48
		const SslProtocols DefaultSslProtocols = SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13;
#else
		const SslProtocols DefaultSslProtocols = SslProtocols.Tls11 | SslProtocols.Tls12;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="MailKit.MailService"/> class.
		/// </summary>
		/// <remarks>
		/// Initializes a new instance of the <see cref="MailKit.MailService"/> class.
		/// </remarks>
		/// <param name="protocolLogger">The protocol logger.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="protocolLogger"/> is <c>null</c>.
		/// </exception>
		protected MailService (IProtocolLogger protocolLogger)
		{
			if (protocolLogger == null)
				throw new ArgumentNullException (nameof (protocolLogger));

			SslProtocols = DefaultSslProtocols;
			CheckCertificateRevocation = true;
			ProtocolLogger = protocolLogger;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MailKit.MailService"/> class.
		/// </summary>
		/// <remarks>
		/// Initializes a new instance of the <see cref="MailKit.MailService"/> class.
		/// </remarks>
		protected MailService () : this (new NullProtocolLogger ())
		{
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="MailService"/> is reclaimed by garbage collection.
		/// </summary>
		/// <remarks>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="MailService"/> is reclaimed by garbage collection.
		/// </remarks>
		~MailService ()
		{
			Dispose (false);
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the service.
		/// </summary>
		/// <remarks>
		/// <para>Gets an object that can be used to synchronize access to the service.</para>
		/// </remarks>
		/// <value>The sync root.</value>
		public abstract object SyncRoot {
			get;
		}

		/// <summary>
		/// Gets the protocol supported by the message service.
		/// </summary>
		/// <remarks>
		/// Gets the protocol supported by the message service.
		/// </remarks>
		/// <value>The protocol.</value>
		protected abstract string Protocol {
			get;
		}

		/// <summary>
		/// Get the protocol logger.
		/// </summary>
		/// <remarks>
		/// Gets the protocol logger.
		/// </remarks>
		/// <value>The protocol logger.</value>
		public IProtocolLogger ProtocolLogger {
			get; private set;
		}

		/// <summary>
		/// Gets or sets the SSL and TLS protocol versions that the client is allowed to use.
		/// </summary>
		/// <remarks>
		/// <para>Gets or sets the SSL and TLS protocol versions that the client is allowed to use.</para>
		/// <para>By default, MailKit initializes this value to support only TLS v1.1 and greater and
		/// does not support TLS v1.0 or any version of SSL due to those protocols no longer being considered
		/// secure.</para>
		/// <para>This property should be set before calling any of the
		/// <a href="Overload_MailKit_MailService_Connect.htm">Connect</a> methods.</para>
		/// </remarks>
		/// <value>The SSL and TLS protocol versions that are supported.</value>
		public SslProtocols SslProtocols {
			get; set;
		}

		/// <summary>
		/// Gets or sets the client SSL certificates.
		/// </summary>
		/// <remarks>
		/// <para>Some servers may require the client SSL certificates in order
		/// to allow the user to connect.</para>
		/// <para>This property should be set before calling any of the
		/// <a href="Overload_MailKit_MailService_Connect.htm">Connect</a> methods.</para>
		/// </remarks>
		/// <value>The client SSL certificates.</value>
		public X509CertificateCollection ClientCertificates {
			get; set;
		}

		/// <summary>
		/// Get or set whether connecting via SSL/TLS should check certificate revocation.
		/// </summary>
		/// <remarks>
		/// <para>Gets or sets whether connecting via SSL/TLS should check certificate revocation.</para>
		/// <para>Normally, the value of this property should be set to <c>true</c> (the default) for security
		/// reasons, but there are times when it may be necessary to set it to <c>false</c>.</para>
		/// <para>For example, most Certificate Authorities are probably pretty good at keeping their CRL and/or
		/// OCSP servers up 24/7, but occasionally they do go down or are otherwise unreachable due to other
		/// network problems between the client and the Certificate Authority. When this happens, it becomes
		/// impossible to check the revocation status of one or more of the certificates in the chain
		/// resulting in an <see cref="SslHandshakeException"/> being thrown in the
		/// <a href="Overload_MailKit_MailService_Connect.htm">Connect</a> method. If this becomes a problem,
		/// it may become desirable to set <see cref="CheckCertificateRevocation"/> to <c>false</c>.</para>
		/// </remarks>
		/// <value><c>true</c> if certificate revocation should be checked; otherwise, <c>false</c>.</value>
		public bool CheckCertificateRevocation {
			get; set;
		}

		/// <summary>
		/// Get or sets a callback function to validate the server certificate.
		/// </summary>
		/// <remarks>
		/// <para>Gets or sets a callback function to validate the server certificate.</para>
		/// <para>This property should be set before calling any of the
		/// <a href="Overload_MailKit_MailService_Connect.htm">Connect</a> methods.</para>
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\SslCertificateValidation.cs"/>
		/// </example>
		/// <value>The server certificate validation callback function.</value>
		public RemoteCertificateValidationCallback ServerCertificateValidationCallback {
			get; set;
		}

		/// <summary>
		/// Get or set the local IP end point to use when connecting to the remote host.
		/// </summary>
		/// <remarks>
		/// Gets or sets the local IP end point to use when connecting to the remote host.
		/// </remarks>
		/// <value>The local IP end point or <c>null</c> to use the default end point.</value>
		public IPEndPoint LocalEndPoint {
			get; set;
		}

		/// <summary>
		/// Get or set the proxy client to use when connecting to a remote host.
		/// </summary>
		/// <remarks>
		/// Gets or sets the proxy client to use when connecting to a remote host via any of the
		/// <a href="Overload_MailKit_MailService_Connect.htm">Connect</a> methods.
		/// </remarks>
		/// <value>The proxy client.</value>
		public IProxyClient ProxyClient {
			get; set;
		}

		/// <summary>
		/// Gets the authentication mechanisms supported by the mail server.
		/// </summary>
		/// <remarks>
		/// The authentication mechanisms are queried as part of the
		/// <a href="Overload_MailKit_MailService_Connect.htm">Connect</a> method.
		/// </remarks>
		/// <value>The authentication mechanisms.</value>
		public abstract HashSet<string> AuthenticationMechanisms {
			get;
		}

		/// <summary>
		/// Gets whether or not the client is currently connected to an mail server.
		/// </summary>
		/// <remarks>
		///<para>The <see cref="IsConnected"/> state is set to <c>true</c> immediately after
		/// one of the <a href="Overload_MailKit_MailService_Connect.htm">Connect</a>
		/// methods succeeds and is not set back to <c>false</c> until either the client
		/// is disconnected via <see cref="Disconnect(bool,CancellationToken)"/> or until a
		/// <see cref="ProtocolException"/> is thrown while attempting to read or write to
		/// the underlying network socket.</para>
		/// <para>When an <see cref="ProtocolException"/> is caught, the connection state of the
		/// <see cref="MailService"/> should be checked before continuing.</para>
		/// </remarks>
		/// <value><c>true</c> if the client is connected; otherwise, <c>false</c>.</value>
		public abstract bool IsConnected {
			get;
		}

		/// <summary>
		/// Get whether or not the connection is secure (typically via SSL or TLS).
		/// </summary>
		/// <remarks>
		/// Gets whether or not the connection is secure (typically via SSL or TLS).
		/// </remarks>
		/// <value><c>true</c> if the connection is secure; otherwise, <c>false</c>.</value>
		public abstract bool IsSecure {
			get;
		}

		/// <summary>
		/// Get whether or not the client is currently authenticated with the mail server.
		/// </summary>
		/// <remarks>
		/// <para>Gets whether or not the client is currently authenticated with the mail server.</para>
		/// <para>To authenticate with the mail server, use one of the
		/// <a href="Overload_MailKit_MailService_Authenticate.htm">Authenticate</a> methods
		/// or any of the Async alternatives.</para>
		/// </remarks>
		/// <value><c>true</c> if the client is authenticated; otherwise, <c>false</c>.</value>
		public abstract bool IsAuthenticated {
			get;
		}

		/// <summary>
		/// Gets or sets the timeout for network streaming operations, in milliseconds.
		/// </summary>
		/// <remarks>
		/// Gets or sets the underlying socket stream's <see cref="System.IO.Stream.ReadTimeout"/>
		/// and <see cref="System.IO.Stream.WriteTimeout"/> values.
		/// </remarks>
		/// <value>The timeout in milliseconds.</value>
		public abstract int Timeout {
			get; set;
		}

		const string AppleCertificateIssuer = "C=US, O=Apple Inc., OU=Certification Authority, CN=Apple IST CA 2 - G1";
		const string GMailCertificateIssuer = "CN=GTS CA 1O1, O=Google Trust Services, C=US";
		const string OutlookCertificateIssuer = "CN=DigiCert Cloud Services CA-1, O=DigiCert Inc, C=US";
		const string YahooCertificateIssuer = "CN=DigiCert SHA2 High Assurance Server CA, OU=www.digicert.com, O=DigiCert Inc, C=US";
		const string GmxCertificateIssuer = "CN=TeleSec ServerPass Extended Validation Class 3 CA, STREET=Untere Industriestr. 20, L=Netphen, OID.2.5.4.17=57250, S=Nordrhein Westfalen, OU=T-Systems Trust Center, O=T-Systems International GmbH, C=DE";

		static bool IsKnownMailServerCertificate (X509Certificate2 certificate)
		{
			var cn = certificate.GetNameInfo (X509NameType.SimpleName, false);
			var fingerprint = certificate.Thumbprint;
			var serial = certificate.SerialNumber;
			var issuer = certificate.Issuer;

			switch (cn) {
			case "imap.gmail.com":
				return issuer == GMailCertificateIssuer && serial == "0096768414983DDE9C0800000000320A68" && fingerprint == "A53BA86C137D828618540738014F7C3D52F699C7";
			case "pop.gmail.com":
				return issuer == GMailCertificateIssuer && serial == "00D80446EA4406BA970800000000320A6A" && fingerprint == "379A18659C855AE5CD00E24CEBE2C6552235B701";
			case "smtp.gmail.com":
				return issuer == GMailCertificateIssuer && serial == "00A2683EEFC8500CA20800000000320A71" && fingerprint == "8F0A0B43DE223D360C4BBC41725C202B806CED32";
			case "outlook.com":
				return issuer == OutlookCertificateIssuer && serial == "0654F84B6325595A20BC68A6A5851CBB" && fingerprint == "7F0804B4D0A6C83E46A3A00EC98F8343D7308566";
			case "imap.mail.me.com":
				return issuer == AppleCertificateIssuer && serial == "62CBBFC566127C4758E96BDBC38EC9E6" && fingerprint == "E1A5F9D22A810979CACDFC0B4151F561E8D02976";
			case "smtp.mail.me.com":
				return issuer == AppleCertificateIssuer && serial == "3460D64A763D9ACA4B460C25021653C7" && fingerprint == "C262F01E83D6CE0C361E8B049E5BE8FE6E55806B";
			case "*.imap.mail.yahoo.com":
				return issuer == YahooCertificateIssuer && serial == "0B2804C9ED82D14FEFEF111E54A0551C" && fingerprint == "F8047F0F60C4641F718353BE7DDC31665B96B5C0";
			case "legacy.pop.mail.yahoo.com":
				return issuer == YahooCertificateIssuer && serial == "05179AA3E07FA5B4D0FC55A7A950B8D8" && fingerprint == "08E010CBAEFAADD20DB0B222C8B6812E762F28EC";
			case "smtp.mail.yahoo.com":
				return issuer == YahooCertificateIssuer && serial == "0F962C48837807B6556C5B6961FC4671" && fingerprint == "E53995EBA816FB73FD4F4BD55ABED04981DA0F18";
			case "mail.gmx.net":
				return issuer == GmxCertificateIssuer && serial == "218296213149726650EB233346353EEA" && fingerprint == "67DED57393303E005937D5EDECB6A29C136024CA";
			default:
				return false;
			}
		}

		static bool IsUntrustedRoot (X509Chain chain)
		{
			foreach (var status in chain.ChainStatus) {
				if (status.Status == X509ChainStatusFlags.NoError || status.Status == X509ChainStatusFlags.UntrustedRoot)
					continue;

				return false;
			}

			return true;
		}

		internal SslCertificateValidationInfo SslCertificateValidationInfo;

		/// <summary>
		/// The default server certificate validation callback used when connecting via SSL or TLS.
		/// </summary>
		/// <remarks>
		/// <para>The default server certificate validation callback recognizes and accepts the certificates
		/// for a list of commonly used mail servers such as gmail.com, outlook.com, mail.me.com, yahoo.com,
		/// and gmx.net.
		/// </remarks>
		/// <returns><c>true</c> if the certificate is deemed valid; otherwise, <c>false</c>.</returns>
		/// <param name="sender">The object that is connecting via SSL or TLS.</param>
		/// <param name="certificate">The server's SSL certificate.</param>
		/// <param name="chain">The server's SSL certificate chain.</param>
		/// <param name="sslPolicyErrors">The SSL policy errors.</param>
		protected bool DefaultServerCertificateValidationCallback (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			const SslPolicyErrors mask = SslPolicyErrors.RemoteCertificateNotAvailable | SslPolicyErrors.RemoteCertificateNameMismatch;

			SslCertificateValidationInfo = null;

			if (sslPolicyErrors == SslPolicyErrors.None)
				return true;

			if ((sslPolicyErrors & mask) == 0) {
				// At this point, all that is left is SslPolicyErrors.RemoteCertificateChainErrors

				// If the problem is an untrusted root, then compare the certificate to a list of known mail server certificates.
				if (IsUntrustedRoot (chain) && certificate is X509Certificate2 certificate2) {
					if (IsKnownMailServerCertificate (certificate2))
						return true;
				}
			}

			SslCertificateValidationInfo = new SslCertificateValidationInfo (sender, certificate, chain, sslPolicyErrors);

			return false;
		}

		internal async Task<Socket> ConnectSocket (string host, int port, bool doAsync, CancellationToken cancellationToken)
		{
			if (ProxyClient != null) {
				ProxyClient.LocalEndPoint = LocalEndPoint;

				if (doAsync)
					return await ProxyClient.ConnectAsync (host, port, Timeout, cancellationToken).ConfigureAwait (false);

				return ProxyClient.Connect (host, port, Timeout, cancellationToken);
			}

			return await SocketUtils.ConnectAsync (host, port, LocalEndPoint, Timeout, doAsync, cancellationToken).ConfigureAwait (false);
		}

		/// <summary>
		/// Establish a connection to the specified mail server.
		/// </summary>
		/// <remarks>
		/// Establishes a connection to the specified mail server.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\SmtpExamples.cs" region="SendMessage"/>
		/// </example>
		/// <param name="host">The host name to connect to.</param>
		/// <param name="port">The port to connect to. If the specified port is <c>0</c>, then the default port will be used.</param>
		/// <param name="options">The secure socket options to when connecting.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="host"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="port"/> is not between <c>0</c> and <c>65535</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// The <paramref name="host"/> is a zero-length string.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="MailService"/> is already connected.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.Net.Sockets.SocketException">
		/// A socket error occurred trying to connect to the remote host.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// A protocol error occurred.
		/// </exception>
		public abstract void Connect (string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously establish a connection to the specified mail server.
		/// </summary>
		/// <remarks>
		/// Asynchronously establishes a connection to the specified mail server.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="host">The host name to connect to.</param>
		/// <param name="port">The port to connect to. If the specified port is <c>0</c>, then the default port will be used.</param>
		/// <param name="options">The secure socket options to when connecting.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="host"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="port"/> is not between <c>0</c> and <c>65535</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// The <paramref name="host"/> is a zero-length string.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="MailService"/> is already connected.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.Net.Sockets.SocketException">
		/// A socket error occurred trying to connect to the remote host.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// A protocol error occurred.
		/// </exception>
		public abstract Task ConnectAsync (string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Establish a connection to the specified mail server using the provided socket.
		/// </summary>
		/// <remarks>
		/// <para>Establish a connection to the specified mail server using the provided socket.</para>
		/// <para>If a successful connection is made, the <see cref="AuthenticationMechanisms"/>
		/// property will be populated.</para>
		/// </remarks>
		/// <param name="socket">The socket to use for the connection.</param>
		/// <param name="host">The host name to connect to.</param>
		/// <param name="port">The port to connect to. If the specified port is <c>0</c>, then the default port will be used.</param>
		/// <param name="options">The secure socket options to when connecting.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="socket"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="host"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="port"/> is not between <c>0</c> and <c>65535</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="socket"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <paramref name="host"/> is a zero-length string.</para>
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailService"/> is already connected.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command was rejected by the mail server.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server responded with an unexpected token.
		/// </exception>
		public abstract void Connect (Socket socket, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously establish a connection to the specified mail server using the provided socket.
		/// </summary>
		/// <remarks>
		/// <para>Asynchronously establishes a connection to the specified mail server using the provided socket.</para>
		/// <para>If a successful connection is made, the <see cref="AuthenticationMechanisms"/>
		/// property will be populated.</para>
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="socket">The socket to use for the connection.</param>
		/// <param name="host">The host name to connect to.</param>
		/// <param name="port">The port to connect to. If the specified port is <c>0</c>, then the default port will be used.</param>
		/// <param name="options">The secure socket options to when connecting.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="socket"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="host"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="port"/> is not between <c>0</c> and <c>65535</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="socket"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <paramref name="host"/> is a zero-length string.</para>
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailService"/> is already connected.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command was rejected by the mail server.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server responded with an unexpected token.
		/// </exception>
		public abstract Task ConnectAsync (Socket socket, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Establish a connection to the specified mail server using the provided stream.
		/// </summary>
		/// <remarks>
		/// <para>Establish a connection to the specified mail server using the provided stream.</para>
		/// <para>If a successful connection is made, the <see cref="AuthenticationMechanisms"/>
		/// property will be populated.</para>
		/// </remarks>
		/// <param name="stream">The stream to use for the connection.</param>
		/// <param name="host">The host name to connect to.</param>
		/// <param name="port">The port to connect to. If the specified port is <c>0</c>, then the default port will be used.</param>
		/// <param name="options">The secure socket options to when connecting.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="stream"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="host"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="port"/> is not between <c>0</c> and <c>65535</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// The <paramref name="host"/> is a zero-length string.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailService"/> is already connected.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command was rejected by the mail server.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server responded with an unexpected token.
		/// </exception>
		public abstract void Connect (Stream stream, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously establish a connection to the specified mail server using the provided stream.
		/// </summary>
		/// <remarks>
		/// <para>Asynchronously establishes a connection to the specified mail server using the provided stream.</para>
		/// <para>If a successful connection is made, the <see cref="AuthenticationMechanisms"/>
		/// property will be populated.</para>
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="stream">The stream to use for the connection.</param>
		/// <param name="host">The host name to connect to.</param>
		/// <param name="port">The port to connect to. If the specified port is <c>0</c>, then the default port will be used.</param>
		/// <param name="options">The secure socket options to when connecting.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="stream"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="host"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="port"/> is not between <c>0</c> and <c>65535</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// The <paramref name="host"/> is a zero-length string.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailService"/> is already connected.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command was rejected by the mail server.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server responded with an unexpected token.
		/// </exception>
		public abstract Task ConnectAsync (Stream stream, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default (CancellationToken));

		internal SecureSocketOptions GetSecureSocketOptions (Uri uri)
		{
			var query = uri.ParsedQuery ();
			var protocol = uri.Scheme;
			string value;

			// Note: early versions of MailKit used "pop3" and "pop3s"
			if (protocol.Equals ("pop3s", StringComparison.OrdinalIgnoreCase))
				protocol = "pops";
			else if (protocol.Equals ("pop3", StringComparison.OrdinalIgnoreCase))
				protocol = "pop";

			if (protocol.Equals (Protocol + "s", StringComparison.OrdinalIgnoreCase))
				return SecureSocketOptions.SslOnConnect;

			if (!protocol.Equals (Protocol, StringComparison.OrdinalIgnoreCase))
				throw new ArgumentException ("Unknown URI scheme.", nameof (uri));

			if (query.TryGetValue ("starttls", out value)) {
				switch (value.ToLowerInvariant ()) {
				default:
					return SecureSocketOptions.StartTlsWhenAvailable;
				case "always": case "true": case "yes":
					return SecureSocketOptions.StartTls;
				case "never": case "false": case "no":
					return SecureSocketOptions.None;
				}
			}

			return SecureSocketOptions.StartTlsWhenAvailable;
		}

		/// <summary>
		/// Establish a connection to the specified mail server.
		/// </summary>
		/// <remarks>
		/// Establishes a connection to the specified mail server.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\SmtpExamples.cs" region="SendMessageUri"/>
		/// </example>
		/// <param name="uri">The server URI.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// The <paramref name="uri"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// The <paramref name="uri"/> is not an absolute URI.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="MailService"/> is already connected.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.Net.Sockets.SocketException">
		/// A socket error occurred trying to connect to the remote host.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// A protocol error occurred.
		/// </exception>
		public void Connect (Uri uri, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (uri == null)
				throw new ArgumentNullException (nameof (uri));

			if (!uri.IsAbsoluteUri)
				throw new ArgumentException ("The uri must be absolute.", nameof (uri));

			var options = GetSecureSocketOptions (uri);

			Connect (uri.Host, uri.Port < 0 ? 0 : uri.Port, options, cancellationToken);
		}

		/// <summary>
		/// Asynchronously establish a connection to the specified mail server.
		/// </summary>
		/// <remarks>
		/// Asynchronously establishes a connection to the specified mail server.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="uri">The server URI.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// The <paramref name="uri"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// The <paramref name="uri"/> is not an absolute URI.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="MailService"/> is already connected.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.Net.Sockets.SocketException">
		/// A socket error occurred trying to connect to the remote host.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// A protocol error occurred.
		/// </exception>
		public Task ConnectAsync (Uri uri, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (uri == null)
				throw new ArgumentNullException (nameof (uri));

			if (!uri.IsAbsoluteUri)
				throw new ArgumentException ("The uri must be absolute.", nameof (uri));

			var options = GetSecureSocketOptions (uri);

			return ConnectAsync (uri.Host, uri.Port < 0 ? 0 : uri.Port, options, cancellationToken);
		}

		/// <summary>
		/// Establish a connection to the specified mail server.
		/// </summary>
		/// <remarks>
		/// <para>Establishes a connection to the specified mail server.</para>
		/// <note type="note">
		/// <para>The <paramref name="useSsl"/> argument only controls whether or
		/// not the client makes an SSL-wrapped connection. In other words, even if the
		/// <paramref name="useSsl"/> parameter is <c>false</c>, SSL/TLS may still be used if
		/// the mail server supports the STARTTLS extension.</para>
		/// <para>To disable all use of SSL/TLS, use the
		/// <see cref="Connect(string,int,MailKit.Security.SecureSocketOptions,System.Threading.CancellationToken)"/>
		/// overload with a value of
		/// <see cref="MailKit.Security.SecureSocketOptions.None">SecureSocketOptions.None</see>
		/// instead.</para>
		/// </note>
		/// </remarks>
		/// <param name="host">The host to connect to.</param>
		/// <param name="port">The port to connect to. If the specified port is <c>0</c>, then the default port will be used.</param>
		/// <param name="useSsl"><value>true</value> if the client should make an SSL-wrapped connection to the server; otherwise, <value>false</value>.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// The <paramref name="host"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="port"/> is out of range (<value>0</value> to <value>65535</value>, inclusive).
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// The <paramref name="host"/> is a zero-length string.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="MailService"/> is already connected.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.Net.Sockets.SocketException">
		/// A socket error occurred trying to connect to the remote host.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// A protocol error occurred.
		/// </exception>
		public void Connect (string host, int port, bool useSsl, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (host == null)
				throw new ArgumentNullException (nameof (host));

			if (host.Length == 0)
				throw new ArgumentException ("The host name cannot be empty.", nameof (host));

			if (port < 0 || port > 65535)
				throw new ArgumentOutOfRangeException (nameof (port));

			Connect (host, port, useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable, cancellationToken);
		}

		/// <summary>
		/// Asynchronously establish a connection to the specified mail server.
		/// </summary>
		/// <remarks>
		/// <para>Asynchronously establishes a connection to the specified mail server.</para>
		/// <note type="note">
		/// <para>The <paramref name="useSsl"/> argument only controls whether or
		/// not the client makes an SSL-wrapped connection. In other words, even if the
		/// <paramref name="useSsl"/> parameter is <c>false</c>, SSL/TLS may still be used if
		/// the mail server supports the STARTTLS extension.</para>
		/// <para>To disable all use of SSL/TLS, use the
		/// <see cref="ConnectAsync(string,int,MailKit.Security.SecureSocketOptions,System.Threading.CancellationToken)"/>
		/// overload with a value of
		/// <see cref="MailKit.Security.SecureSocketOptions.None">SecureSocketOptions.None</see>
		/// instead.</para>
		/// </note>
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="host">The host to connect to.</param>
		/// <param name="port">The port to connect to. If the specified port is <c>0</c>, then the default port will be used.</param>
		/// <param name="useSsl"><value>true</value> if the client should make an SSL-wrapped connection to the server; otherwise, <value>false</value>.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// The <paramref name="host"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="port"/> is out of range (<value>0</value> to <value>65535</value>, inclusive).
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// The <paramref name="host"/> is a zero-length string.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="MailService"/> is already connected.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.Net.Sockets.SocketException">
		/// A socket error occurred trying to connect to the remote host.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// A protocol error occurred.
		/// </exception>
		public Task ConnectAsync (string host, int port, bool useSsl, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (host == null)
				throw new ArgumentNullException (nameof (host));

			if (host.Length == 0)
				throw new ArgumentException ("The host name cannot be empty.", nameof (host));

			if (port < 0 || port > 65535)
				throw new ArgumentOutOfRangeException (nameof (port));

			return ConnectAsync (host, port, useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable, cancellationToken);
		}

		/// <summary>
		/// Authenticate using the supplied credentials.
		/// </summary>
		/// <remarks>
		/// <para>If the server supports one or more SASL authentication mechanisms,
		/// then the SASL mechanisms that both the client and server support are tried
		/// in order of greatest security to weakest security. Once a SASL
		/// authentication mechanism is found that both client and server support,
		/// the credentials are used to authenticate.</para>
		/// <para>If the server does not support SASL or if no common SASL mechanisms
		/// can be found, then the default login command is used as a fallback.</para>
		/// <note type="tip">To prevent the usage of certain authentication mechanisms,
		/// simply remove them from the <see cref="AuthenticationMechanisms"/> hash set
		/// before calling this method.</note>
		/// </remarks>
		/// <param name="encoding">The encoding to use for the user's credentials.</param>
		/// <param name="credentials">The user's credentials.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="encoding"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="credentials"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="MailService"/> is not connected or is already authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="MailKit.Security.AuthenticationException">
		/// Authentication using the supplied credentials has failed.
		/// </exception>
		/// <exception cref="MailKit.Security.SaslException">
		/// A SASL authentication error occurred.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// A protocol error occurred.
		/// </exception>
		public abstract void Authenticate (Encoding encoding, ICredentials credentials, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously authenticate using the supplied credentials.
		/// </summary>
		/// <remarks>
		/// <para>If the server supports one or more SASL authentication mechanisms,
		/// then the SASL mechanisms that both the client and server support are tried
		/// in order of greatest security to weakest security. Once a SASL
		/// authentication mechanism is found that both client and server support,
		/// the credentials are used to authenticate.</para>
		/// <para>If the server does not support SASL or if no common SASL mechanisms
		/// can be found, then the default login command is used as a fallback.</para>
		/// <note type="tip">To prevent the usage of certain authentication mechanisms,
		/// simply remove them from the <see cref="AuthenticationMechanisms"/> hash set
		/// before calling this method.</note>
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="encoding">The encoding to use for the user's credentials.</param>
		/// <param name="credentials">The user's credentials.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="encoding"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="credentials"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="MailService"/> is not connected or is already authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="MailKit.Security.AuthenticationException">
		/// Authentication using the supplied credentials has failed.
		/// </exception>
		/// <exception cref="MailKit.Security.SaslException">
		/// A SASL authentication error occurred.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// A protocol error occurred.
		/// </exception>
		public abstract Task AuthenticateAsync (Encoding encoding, ICredentials credentials, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Authenticate using the supplied credentials.
		/// </summary>
		/// <remarks>
		/// <para>If the server supports one or more SASL authentication mechanisms,
		/// then the SASL mechanisms that both the client and server support are tried
		/// in order of greatest security to weakest security. Once a SASL
		/// authentication mechanism is found that both client and server support,
		/// the credentials are used to authenticate.</para>
		/// <para>If the server does not support SASL or if no common SASL mechanisms
		/// can be found, then the default login command is used as a fallback.</para>
		/// <note type="tip">To prevent the usage of certain authentication mechanisms,
		/// simply remove them from the <see cref="AuthenticationMechanisms"/> hash set
		/// before calling this method.</note>
		/// </remarks>
		/// <param name="credentials">The user's credentials.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="credentials"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="MailService"/> is not connected or is already authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="MailKit.Security.AuthenticationException">
		/// Authentication using the supplied credentials has failed.
		/// </exception>
		/// <exception cref="MailKit.Security.SaslException">
		/// A SASL authentication error occurred.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// A protocol error occurred.
		/// </exception>
		public void Authenticate (ICredentials credentials, CancellationToken cancellationToken = default (CancellationToken))
		{
			Authenticate (Encoding.UTF8, credentials, cancellationToken);
		}

		/// <summary>
		/// Asynchronously authenticate using the supplied credentials.
		/// </summary>
		/// <remarks>
		/// <para>If the server supports one or more SASL authentication mechanisms,
		/// then the SASL mechanisms that both the client and server support are tried
		/// in order of greatest security to weakest security. Once a SASL
		/// authentication mechanism is found that both client and server support,
		/// the credentials are used to authenticate.</para>
		/// <para>If the server does not support SASL or if no common SASL mechanisms
		/// can be found, then the default login command is used as a fallback.</para>
		/// <note type="tip">To prevent the usage of certain authentication mechanisms,
		/// simply remove them from the <see cref="AuthenticationMechanisms"/> hash set
		/// before calling this method.</note>
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="credentials">The user's credentials.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="credentials"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="MailService"/> is not connected or is already authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="MailKit.Security.AuthenticationException">
		/// Authentication using the supplied credentials has failed.
		/// </exception>
		/// <exception cref="MailKit.Security.SaslException">
		/// A SASL authentication error occurred.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// A protocol error occurred.
		/// </exception>
		public Task AuthenticateAsync (ICredentials credentials, CancellationToken cancellationToken = default (CancellationToken))
		{
			return AuthenticateAsync (Encoding.UTF8, credentials, cancellationToken);
		}

		/// <summary>
		/// Authenticate using the specified user name and password.
		/// </summary>
		/// <remarks>
		/// <para>If the server supports one or more SASL authentication mechanisms,
		/// then the SASL mechanisms that both the client and server support are tried
		/// in order of greatest security to weakest security. Once a SASL
		/// authentication mechanism is found that both client and server support,
		/// the credentials are used to authenticate.</para>
		/// <para>If the server does not support SASL or if no common SASL mechanisms
		/// can be found, then the default login command is used as a fallback.</para>
		/// <note type="tip">To prevent the usage of certain authentication mechanisms,
		/// simply remove them from the <see cref="AuthenticationMechanisms"/> hash set
		/// before calling this method.</note>
		/// </remarks>
		/// <param name="encoding">The encoding to use for the user's credentials.</param>
		/// <param name="userName">The user name.</param>
		/// <param name="password">The password.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="encoding"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="userName"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="password"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="MailService"/> is not connected or is already authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="MailKit.Security.AuthenticationException">
		/// Authentication using the supplied credentials has failed.
		/// </exception>
		/// <exception cref="MailKit.Security.SaslException">
		/// A SASL authentication error occurred.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// A protocol error occurred.
		/// </exception>
		public void Authenticate (Encoding encoding, string userName, string password, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (encoding == null)
				throw new ArgumentNullException (nameof (encoding));

			if (userName == null)
				throw new ArgumentNullException (nameof (userName));

			if (password == null)
				throw new ArgumentNullException (nameof (password));

			var credentials = new NetworkCredential (userName, password);

			Authenticate (encoding, credentials, cancellationToken);
		}

		/// <summary>
		/// Asynchronously authenticate using the specified user name and password.
		/// </summary>
		/// <remarks>
		/// <para>If the server supports one or more SASL authentication mechanisms,
		/// then the SASL mechanisms that both the client and server support are tried
		/// in order of greatest security to weakest security. Once a SASL
		/// authentication mechanism is found that both client and server support,
		/// the credentials are used to authenticate.</para>
		/// <para>If the server does not support SASL or if no common SASL mechanisms
		/// can be found, then the default login command is used as a fallback.</para>
		/// <note type="tip">To prevent the usage of certain authentication mechanisms,
		/// simply remove them from the <see cref="AuthenticationMechanisms"/> hash set
		/// before calling this method.</note>
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="encoding">The encoding to use for the user's credentials.</param>
		/// <param name="userName">The user name.</param>
		/// <param name="password">The password.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="encoding"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="userName"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="password"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="MailService"/> is not connected or is already authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="MailKit.Security.AuthenticationException">
		/// Authentication using the supplied credentials has failed.
		/// </exception>
		/// <exception cref="MailKit.Security.SaslException">
		/// A SASL authentication error occurred.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// A protocol error occurred.
		/// </exception>
		public Task AuthenticateAsync (Encoding encoding, string userName, string password, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (encoding == null)
				throw new ArgumentNullException (nameof (encoding));

			if (userName == null)
				throw new ArgumentNullException (nameof (userName));

			if (password == null)
				throw new ArgumentNullException (nameof (password));

			var credentials = new NetworkCredential (userName, password);

			return AuthenticateAsync (encoding, credentials, cancellationToken);
		}

		/// <summary>
		/// Authenticate using the specified user name and password.
		/// </summary>
		/// <remarks>
		/// <para>If the server supports one or more SASL authentication mechanisms,
		/// then the SASL mechanisms that both the client and server support are tried
		/// in order of greatest security to weakest security. Once a SASL
		/// authentication mechanism is found that both client and server support,
		/// the credentials are used to authenticate.</para>
		/// <para>If the server does not support SASL or if no common SASL mechanisms
		/// can be found, then the default login command is used as a fallback.</para>
		/// <note type="tip">To prevent the usage of certain authentication mechanisms,
		/// simply remove them from the <see cref="AuthenticationMechanisms"/> hash set
		/// before calling this method.</note>
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\SmtpExamples.cs" region="SendMessage"/>
		/// </example>
		/// <param name="userName">The user name.</param>
		/// <param name="password">The password.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="userName"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="password"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="MailService"/> is not connected or is already authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="MailKit.Security.AuthenticationException">
		/// Authentication using the supplied credentials has failed.
		/// </exception>
		/// <exception cref="MailKit.Security.SaslException">
		/// A SASL authentication error occurred.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// A protocol error occurred.
		/// </exception>
		public void Authenticate (string userName, string password, CancellationToken cancellationToken = default (CancellationToken))
		{
			Authenticate (Encoding.UTF8, userName, password, cancellationToken);
		}

		/// <summary>
		/// Asynchronously authenticate using the specified user name and password.
		/// </summary>
		/// <remarks>
		/// <para>If the server supports one or more SASL authentication mechanisms,
		/// then the SASL mechanisms that both the client and server support are tried
		/// in order of greatest security to weakest security. Once a SASL
		/// authentication mechanism is found that both client and server support,
		/// the credentials are used to authenticate.</para>
		/// <para>If the server does not support SASL or if no common SASL mechanisms
		/// can be found, then the default login command is used as a fallback.</para>
		/// <note type="tip">To prevent the usage of certain authentication mechanisms,
		/// simply remove them from the <see cref="AuthenticationMechanisms"/> hash set
		/// before calling this method.</note>
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="userName">The user name.</param>
		/// <param name="password">The password.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="userName"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="password"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="MailService"/> is not connected or is already authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="MailKit.Security.AuthenticationException">
		/// Authentication using the supplied credentials has failed.
		/// </exception>
		/// <exception cref="MailKit.Security.SaslException">
		/// A SASL authentication error occurred.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// A protocol error occurred.
		/// </exception>
		public Task AuthenticateAsync (string userName, string password, CancellationToken cancellationToken = default (CancellationToken))
		{
			return AuthenticateAsync (Encoding.UTF8, userName, password, cancellationToken);
		}

		/// <summary>
		/// Authenticate using the specified SASL mechanism.
		/// </summary>
		/// <remarks>
		/// <para>Authenticates using the specified SASL mechanism.</para>
		/// <para>For a list of available SASL authentication mechanisms supported by the server,
		/// check the <see cref="AuthenticationMechanisms"/> property after the service has been
		/// connected.</para>
		/// </remarks>
		/// <param name="mechanism">The SASL mechanism.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="mechanism"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="MailService"/> is not connected or is already authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="MailKit.Security.AuthenticationException">
		/// Authentication using the supplied credentials has failed.
		/// </exception>
		/// <exception cref="MailKit.Security.SaslException">
		/// A SASL authentication error occurred.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// A protocol error occurred.
		/// </exception>
		public abstract void Authenticate (SaslMechanism mechanism, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously authenticate using the specified SASL mechanism.
		/// </summary>
		/// <remarks>
		/// <para>Authenticates using the specified SASL mechanism.</para>
		/// <para>For a list of available SASL authentication mechanisms supported by the server,
		/// check the <see cref="AuthenticationMechanisms"/> property after the service has been
		/// connected.</para>
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="mechanism">The SASL mechanism.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="mechanism"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="MailService"/> is not connected or is already authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="MailKit.Security.AuthenticationException">
		/// Authentication using the supplied credentials has failed.
		/// </exception>
		/// <exception cref="MailKit.Security.SaslException">
		/// A SASL authentication error occurred.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// A protocol error occurred.
		/// </exception>
		public abstract Task AuthenticateAsync (SaslMechanism mechanism, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Disconnect the service.
		/// </summary>
		/// <remarks>
		/// If <paramref name="quit"/> is <c>true</c>, a logout/quit command will be issued in order to disconnect cleanly.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\SmtpExamples.cs" region="SendMessage"/>
		/// </example>
		/// <param name="quit">If set to <c>true</c>, a logout/quit command will be issued in order to disconnect cleanly.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		public abstract void Disconnect (bool quit, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously disconnect the service.
		/// </summary>
		/// <remarks>
		/// If <paramref name="quit"/> is <c>true</c>, a logout/quit command will be issued in order to disconnect cleanly.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="quit">If set to <c>true</c>, a logout/quit command will be issued in order to disconnect cleanly.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		public abstract Task DisconnectAsync (bool quit, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Ping the mail server to keep the connection alive.
		/// </summary>
		/// <remarks>
		/// Mail servers, if left idle for too long, will automatically drop the connection.
		/// </remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="MailService"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="MailService"/> is not authenticated.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command was rejected by the mail server.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server responded with an unexpected token.
		/// </exception>
		public abstract void NoOp (CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously ping the mail server to keep the connection alive.
		/// </summary>
		/// <remarks>
		/// Mail servers, if left idle for too long, will automatically drop the connection.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="MailService"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="MailService"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="MailService"/> is not authenticated.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command was rejected by the mail server.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server responded with an unexpected token.
		/// </exception>
		public abstract Task NoOpAsync (CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Occurs when the client has been successfully connected.
		/// </summary>
		/// <remarks>
		/// The <see cref="Connected"/> event is raised when the client
		/// successfully connects to the mail server.
		/// </remarks>
		public event EventHandler<ConnectedEventArgs> Connected;

		/// <summary>
		/// Raise the connected event.
		/// </summary>
		/// <remarks>
		/// Raises the connected event.
		/// </remarks>
		/// <param name="host">The name of the host that the client connected to.</param>
		/// <param name="port">The port that the client connected to on the remote host.</param>
		/// <param name="options">The SSL/TLS options that were used when connecting.</param>
		protected virtual void OnConnected (string host, int port, SecureSocketOptions options)
		{
			var handler = Connected;

			if (handler != null)
				handler (this, new ConnectedEventArgs (host, port, options));
		}

		/// <summary>
		/// Occurs when the client gets disconnected.
		/// </summary>
		/// <remarks>
		/// The <see cref="Disconnected"/> event is raised whenever the client
		/// gets disconnected.
		/// </remarks>
		public event EventHandler<DisconnectedEventArgs> Disconnected;

		/// <summary>
		/// Raise the disconnected event.
		/// </summary>
		/// <remarks>
		/// Raises the disconnected event.
		/// </remarks>
		/// <param name="host">The name of the host that the client was connected to.</param>
		/// <param name="port">The port that the client was connected to on the remote host.</param>
		/// <param name="options">The SSL/TLS options that were used by the client.</param>
		/// <param name="requested"><c>true</c> if the disconnect was explicitly requested; otherwise, <c>false</c>.</param>
		protected virtual void OnDisconnected (string host, int port, SecureSocketOptions options, bool requested)
		{
			var handler = Disconnected;

			if (handler != null)
				handler (this, new DisconnectedEventArgs (host, port, options, requested));
		}

		/// <summary>
		/// Occurs when the client has been successfully authenticated.
		/// </summary>
		/// <remarks>
		/// The <see cref="Authenticated"/> event is raised whenever the client
		/// has been authenticated.
		/// </remarks>
		public event EventHandler<AuthenticatedEventArgs> Authenticated;

		/// <summary>
		/// Raise the authenticated event.
		/// </summary>
		/// <remarks>
		/// Raises the authenticated event.
		/// </remarks>
		/// <param name="message">The notification sent by the server when the client successfully authenticates.</param>
		protected virtual void OnAuthenticated (string message)
		{
			var handler = Authenticated;

			if (handler != null)
				handler (this, new AuthenticatedEventArgs (message));
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="MailService"/> and
		/// optionally releases the managed resources.
		/// </summary>
		/// <remarks>
		/// Releases the unmanaged resources used by the <see cref="MailService"/> and
		/// optionally releases the managed resources.
		/// </remarks>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources;
		/// <c>false</c> to release only the unmanaged resources.</param>
		protected virtual void Dispose (bool disposing)
		{
			if (disposing)
				ProtocolLogger.Dispose ();
		}

		/// <summary>
		/// Releases all resource used by the <see cref="MailService"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose()"/> when you are finished using the <see cref="MailService"/>. The
		/// <see cref="Dispose()"/> method leaves the <see cref="MailService"/> in an unusable state. After
		/// calling <see cref="Dispose()"/>, you must release all references to the <see cref="MailService"/> so
		/// the garbage collector can reclaim the memory that the <see cref="MailService"/> was occupying.</remarks>
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}
	}
}
