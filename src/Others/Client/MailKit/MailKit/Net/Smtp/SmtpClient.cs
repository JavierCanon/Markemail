﻿//
// SmtpClient.cs
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
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net.Security;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using MimeKit;
using MimeKit.IO;
using MimeKit.Cryptography;

using MailKit.Security;

using SslStream = MailKit.Net.SslStream;
using NetworkStream = MailKit.Net.NetworkStream;

namespace MailKit.Net.Smtp {
	/// <summary>
	/// An SMTP client that can be used to send email messages.
	/// </summary>
	/// <remarks>
	/// <para>The <see cref="SmtpClient"/> class supports both the "smtp" and "smtps" protocols. The "smtp"
	/// protocol makes a clear-text connection to the SMTP server and does not use SSL or TLS unless the SMTP
	/// server supports the <a href="https://tools.ietf.org/html/rfc3207">STARTTLS</a> extension. The "smtps"
	/// protocol, however, connects to the SMTP server using an SSL-wrapped connection.</para>
	/// <para>The connection established by any of the
	/// <a href="Overload_MailKit_Net_Smtp_SmtpClient_Connect.htm">Connect</a> methods may be re-used if an
	/// application wishes to send multiple messages to the same SMTP server. Since connecting and authenticating
	/// can be expensive operations, re-using a connection can significantly improve performance when sending a
	/// large number of messages to the same SMTP server over a short period of time.</para>
	/// </remarks>
	/// <example>
	/// <code language="c#" source="Examples\SmtpExamples.cs" region="SendMessages" />
	/// </example>
	public partial class SmtpClient : MailTransport, ISmtpClient
	{
		static readonly byte[] EndData = Encoding.ASCII.GetBytes (".\r\n");
		const int MaxLineLength = 998;

		enum SmtpCommand {
			MailFrom,
			RcptTo
		}

		readonly HashSet<string> authenticationMechanisms = new HashSet<string> (StringComparer.Ordinal);
		readonly List<SmtpCommand> queued = new List<SmtpCommand> ();
		SmtpCapabilities capabilities;
		int timeout = 2 * 60 * 1000;
		bool authenticated;
		bool connected;
		bool disposed;
		bool secure;
		Uri uri;

		/// <summary>
		/// Initializes a new instance of the <see cref="MailKit.Net.Smtp.SmtpClient"/> class.
		/// </summary>
		/// <remarks>
		/// Before you can send messages with the <see cref="SmtpClient"/>, you must first call one of
		/// the <a href="Overload_MailKit_Net_Smtp_SmtpClient_Connect.htm">Connect</a> methods.
		/// Depending on whether the SMTP server requires authenticating or not, you may also need to
		/// authenticate using one of the
		/// <a href="Overload_MailKit_Net_Smtp_SmtpClient_Authenticate.htm">Authenticate</a> methods.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\SmtpExamples.cs" region="SendMessages" />
		/// </example>
		public SmtpClient () : this (new NullProtocolLogger ())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MailKit.Net.Smtp.SmtpClient"/> class.
		/// </summary>
		/// <remarks>
		/// Before you can send messages with the <see cref="SmtpClient"/>, you must first call one of
		/// the <a href="Overload_MailKit_Net_Smtp_SmtpClient_Connect.htm">Connect</a> methods.
		/// Depending on whether the SMTP server requires authenticating or not, you may also need to
		/// authenticate using one of the
		/// <a href="Overload_MailKit_Net_Smtp_SmtpClient_Authenticate.htm">Authenticate</a> methods.
		/// </remarks>
		/// <param name="protocolLogger">The protocol logger.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="protocolLogger"/> is <c>null</c>.
		/// </exception>
		/// <example>
		/// <code language="c#" source="Examples\SmtpExamples.cs" region="ProtocolLogger"/>
		/// </example>
		public SmtpClient (IProtocolLogger protocolLogger) : base (protocolLogger)
		{
		}

		/// <summary>
		/// Get the underlying SMTP stream.
		/// </summary>
		/// <remarks>
		/// Gets the underlying SMTP stream.
		/// </remarks>
		/// <value>The SMTP stream.</value>
		SmtpStream Stream {
			get; set;
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the SMTP server.
		/// </summary>
		/// <remarks>
		/// <para>Gets an object that can be used to synchronize access to the SMTP server between multiple threads.</para>
		/// <para>When using <see cref="SmtpClient"/> methods from multiple threads, it is important to lock the
		/// <see cref="SyncRoot"/> object for thread safety.</para>
		/// </remarks>
		/// <value>The lock object.</value>
		public override object SyncRoot {
			get { return this; }
		}

		/// <summary>
		/// Get the protocol supported by the message service.
		/// </summary>
		/// <remarks>
		/// Gets the protocol supported by the message service.
		/// </remarks>
		/// <value>The protocol.</value>
		protected override string Protocol {
			get { return "smtp"; }
		}

		/// <summary>
		/// Get the capabilities supported by the SMTP server.
		/// </summary>
		/// <remarks>
		/// The capabilities will not be known until a successful connection has been made 
		/// and may change once the client is authenticated.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\SmtpExamples.cs" region="Capabilities"/>
		/// </example>
		/// <value>The capabilities.</value>
		/// <exception cref="System.ArgumentException">
		/// Capabilities cannot be enabled, they may only be disabled.
		/// </exception>
		public SmtpCapabilities Capabilities {
			get { return capabilities; }
			set {
				if ((capabilities | value) > capabilities)
					throw new ArgumentException ("Capabilities cannot be enabled, they may only be disabled.", nameof (value));

				capabilities = value;
			}
		}

		/// <summary>
		/// Get or set the local domain.
		/// </summary>
		/// <remarks>
		/// The local domain is used in the HELO or EHLO commands sent to
		/// the SMTP server. If left unset, the local IP address will be
		/// used instead.
		/// </remarks>
		/// <value>The local domain.</value>
		public string LocalDomain {
			get; set;
		}

		/// <summary>
		/// Get whether or not the BDAT command is preferred over the DATA command.
		/// </summary>
		/// <remarks>
		/// <para>Gets whether or not the <c>BDAT</c> command is preferred over the standard <c>DATA</c>
		/// command.</para>
		/// <para>The <c>BDAT</c> command is normally only used when the message being sent contains binary data
		/// (e.g. one mor more MIME parts contains a <c>Content-Transfer-Encoding: binary</c> header). This
		/// option provides a way to override this behavior, forcing the <see cref="SmtpClient"/> to send
		/// messages using the <c>BDAT</c> command instead of the <c>DATA</c> command even when it is not
		/// necessary to do so.</para>
		/// </remarks>
		/// <value><c>true</c> if the <c>BDAT</c> command is preferred over the <c>DATA</c> command; otherwise, <c>false</c>.</value>
		protected virtual bool PreferSendAsBinaryData {
			get { return false; }
		}

		/// <summary>
		/// Get the maximum message size supported by the server.
		/// </summary>
		/// <remarks>
		/// <para>The maximum message size will not be known until a successful connection has
		/// been made and may change once the client is authenticated.</para>
		/// <note type="note">This value is only relevant if the <see cref="Capabilities"/> includes
		/// the <see cref="SmtpCapabilities.Size"/> flag.</note>
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\SmtpExamples.cs" region="Capabilities"/>
		/// </example>
		/// <value>The maximum message size supported by the server.</value>
		public uint MaxSize {
			get; private set;
		}

		void CheckDisposed ()
		{
			if (disposed)
				throw new ObjectDisposedException (nameof (SmtpClient));
		}

		#region IMailService implementation

		/// <summary>
		/// Get the authentication mechanisms supported by the SMTP server.
		/// </summary>
		/// <remarks>
		/// <para>The authentication mechanisms are queried as part of the connection
		/// process.</para>
		/// <note type="tip">To prevent the usage of certain authentication mechanisms,
		/// simply remove them from the <see cref="AuthenticationMechanisms"/> hash set
		/// before authenticating.</note>
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\SmtpExamples.cs" region="Capabilities"/>
		/// </example>
		/// <value>The authentication mechanisms.</value>
		public override HashSet<string> AuthenticationMechanisms {
			get { return authenticationMechanisms; }
		}

		/// <summary>
		/// Get or set the timeout for network streaming operations, in milliseconds.
		/// </summary>
		/// <remarks>
		/// Gets or sets the underlying socket stream's <see cref="System.IO.Stream.ReadTimeout"/>
		/// and <see cref="System.IO.Stream.WriteTimeout"/> values.
		/// </remarks>
		/// <value>The timeout in milliseconds.</value>
		public override int Timeout {
			get { return timeout; }
			set {
				if (IsConnected && Stream.CanTimeout) {
					Stream.WriteTimeout = value;
					Stream.ReadTimeout = value;
				}

				timeout = value;
			}
		}

		/// <summary>
		/// Get whether or not the client is currently connected to an SMTP server.
		/// </summary>
		/// <remarks>
		/// <para>The <see cref="IsConnected"/> state is set to <c>true</c> immediately after
		/// one of the <a href="Overload_MailKit_Net_Smtp_SmtpClient_Connect.htm">Connect</a>
		/// methods succeeds and is not set back to <c>false</c> until either the client
		/// is disconnected via <see cref="Disconnect(bool,CancellationToken)"/> or until an
		/// <see cref="SmtpProtocolException"/> is thrown while attempting to read or write to
		/// the underlying network socket.</para>
		/// <para>When an <see cref="SmtpProtocolException"/> is caught, the connection state of the
		/// <see cref="SmtpClient"/> should be checked before continuing.</para>
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\SmtpExamples.cs" region="ExceptionHandling"/>
		/// </example>
		/// <value><c>true</c> if the client is connected; otherwise, <c>false</c>.</value>
		public override bool IsConnected {
			get { return connected; }
		}

		/// <summary>
		/// Get whether or not the connection is secure (typically via SSL or TLS).
		/// </summary>
		/// <remarks>
		/// Gets whether or not the connection is secure (typically via SSL or TLS).
		/// </remarks>
		/// <value><c>true</c> if the connection is secure; otherwise, <c>false</c>.</value>
		public override bool IsSecure {
			get { return IsConnected && secure; }
		}

		/// <summary>
		/// Get whether or not the client is currently authenticated with the SMTP server.
		/// </summary>
		/// <remarks>
		/// <para>Gets whether or not the client is currently authenticated with the SMTP server.</para>
		/// <para>To authenticate with the SMTP server, use one of the
		/// <a href="Overload_MailKit_Net_Smtp_SmtpClient_Authenticate.htm">Authenticate</a>
		/// methods.</para>
		/// </remarks>
		/// <value><c>true</c> if the client is connected; otherwise, <c>false</c>.</value>
		public override bool IsAuthenticated {
			get { return authenticated; }
		}

		bool ValidateRemoteCertificate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (ServerCertificateValidationCallback != null)
				return ServerCertificateValidationCallback (uri.Host, certificate, chain, sslPolicyErrors);

#if !NETSTANDARD1_3 && !NETSTANDARD1_6
			if (ServicePointManager.ServerCertificateValidationCallback != null)
				return ServicePointManager.ServerCertificateValidationCallback (uri.Host, certificate, chain, sslPolicyErrors);
#endif

			return DefaultServerCertificateValidationCallback (uri.Host, certificate, chain, sslPolicyErrors);
		}

		async Task QueueCommandAsync (SmtpCommand type, string command, bool doAsync, CancellationToken cancellationToken)
		{
			var bytes = Encoding.UTF8.GetBytes (command + "\r\n");

			// Note: queued commands will be buffered by the stream
			if (doAsync)
				await Stream.WriteAsync (bytes, 0, bytes.Length, cancellationToken).ConfigureAwait (false);
			else
				Stream.Write (bytes, 0, bytes.Length, cancellationToken);
			queued.Add (type);
		}

		/// <summary>
		/// Invoked only when no recipients were accepted by the SMTP server.
		/// </summary>
		/// <remarks>
		/// If <see cref="OnRecipientNotAccepted"/> is overridden to not throw
		/// an exception, this method should be overridden to throw an appropriate
		/// exception instead.
		/// </remarks>
		/// <param name="message">The message being sent.</param>
		protected virtual void OnNoRecipientsAccepted (MimeMessage message)
		{
		}

		async Task FlushCommandQueueAsync (MimeMessage message, MailboxAddress sender, IList<MailboxAddress> recipients, bool doAsync, CancellationToken cancellationToken)
		{
			try {
				// Note: Queued commands are buffered by the stream
				if (doAsync)
					await Stream.FlushAsync (cancellationToken).ConfigureAwait (false);
				else
					Stream.Flush (cancellationToken);
			} catch {
				queued.Clear ();
				throw;
			}

			var responses = new List<SmtpResponse> ();
			Exception rex = null;
			int accepted = 0;
			int rcpt = 0;

			// Note: We need to read all responses from the server before we can process
			// them in case any of them have any errors so that we can RSET the state.
			try {
				for (int i = 0; i < queued.Count; i++) {
					SmtpResponse response;

					if (doAsync)
						response = await Stream.ReadResponseAsync (cancellationToken).ConfigureAwait (false);
					else
						response = Stream.ReadResponse (cancellationToken);

					responses.Add (response);
				}
			} catch (Exception ex) {
				// Note: Most likely this exception is due to an unexpected disconnect.
				// Usually, before an SMTP server disconnects the client, it will send an
				// error code response that will be more useful to the user than an error
				// stating that the server has unexpected disconnected. Save this exception
				// in case the server didn't give us a response with an error code.
				rex = ex;
			}

			try {
				// process the responses
				for (int i = 0; i < responses.Count; i++) {
					switch (queued[i]) {
					case SmtpCommand.MailFrom:
						ProcessMailFromResponse (message, sender, responses[i]);
						break;
					case SmtpCommand.RcptTo:
						if (ProcessRcptToResponse (message, recipients[rcpt++], responses[i]))
							accepted++;
						break;
					}
				}
			} finally {
				queued.Clear ();
			}

			// throw the saved exception
			if (rex != null)
				throw rex;

			if (accepted == 0)
				OnNoRecipientsAccepted (message);
		}

		async Task<SmtpResponse> SendCommandAsync (string command, bool doAsync, CancellationToken cancellationToken)
		{
			var bytes = Encoding.UTF8.GetBytes (command + "\r\n");

			if (doAsync) {
				await Stream.WriteAsync (bytes, 0, bytes.Length, cancellationToken).ConfigureAwait (false);
				await Stream.FlushAsync (cancellationToken).ConfigureAwait (false);

				return await Stream.ReadResponseAsync (cancellationToken).ConfigureAwait (false);
			}

			Stream.Write (bytes, 0, bytes.Length, cancellationToken);
			Stream.Flush (cancellationToken);

			return Stream.ReadResponse (cancellationToken);
		}

		/// <summary>
		/// Send a custom command to the SMTP server.
		/// </summary>
		/// <remarks>
		/// <para>Sends a custom command to the SMTP server.</para>
		/// <note type="note">The command string should not include the terminating <c>\r\n</c> sequence.</note>
		/// </remarks>
		/// <returns>The command response.</returns>
		/// <param name="command">The command.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="command"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="SmtpClient"/> has been disposed.
		/// </exception>
		/// <exception cref="ServiceNotConnectedException">
		/// The <see cref="SmtpClient"/> is not connected.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation has been canceled.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="SmtpCommandException">
		/// The SMTP command failed.
		/// </exception>
		/// <exception cref="SmtpProtocolException">
		/// An SMTP protocol exception occurred.
		/// </exception>
		protected SmtpResponse SendCommand (string command, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (command == null)
				throw new ArgumentNullException (nameof (command));

			CheckDisposed ();

			if (!IsConnected)
				throw new ServiceNotConnectedException ("The SmtpClient must be connected before you can send commands.");

			return SendCommandAsync (command, false, cancellationToken).GetAwaiter ().GetResult ();
		}

		async Task<SmtpResponse> SendEhloAsync (bool ehlo, bool doAsync, CancellationToken cancellationToken)
		{
			var network = NetworkStream.Get (Stream.Stream);
			string command = ehlo ? "EHLO " : "HELO ";
			string domain = null;
			IPAddress ip = null;

			if (!string.IsNullOrEmpty (LocalDomain)) {
				if (!IPAddress.TryParse (LocalDomain, out ip))
					domain = LocalDomain;
			} else if (network != null) {
				var ipEndPoint = network.Socket.LocalEndPoint as IPEndPoint;

				if (ipEndPoint == null)
					domain = ((DnsEndPoint) network.Socket.LocalEndPoint).Host;
				else
					ip = ipEndPoint.Address;
			} else {
				domain = "[127.0.0.1]";
			}

			if (ip != null) {
				if (ip.IsIPv4MappedToIPv6) {
					try {
						ip = ip.MapToIPv4 ();
					} catch (ArgumentOutOfRangeException) {
						// .NET 4.5.2 bug on Windows 7 SP1 (issue #814)
					}
				}

				if (ip.AddressFamily == AddressFamily.InterNetworkV6)
					domain = "[IPv6:" + ip + "]";
				else
					domain = "[" + ip + "]";
			}

			command += domain;

			return await SendCommandAsync (command, doAsync, cancellationToken).ConfigureAwait (false);
		}

		async Task EhloAsync (bool doAsync, CancellationToken cancellationToken)
		{
			SmtpResponse response;

			response = await SendEhloAsync (true, doAsync, cancellationToken).ConfigureAwait (false);

			// Some SMTP servers do not accept an EHLO after authentication (despite the rfc saying it is required).
			if (authenticated && response.StatusCode == SmtpStatusCode.BadCommandSequence)
				return;

			if (response.StatusCode != SmtpStatusCode.Ok) {
				// Try sending HELO instead...
				response = await SendEhloAsync (false, doAsync, cancellationToken).ConfigureAwait (false);
				if (response.StatusCode != SmtpStatusCode.Ok)
					throw new SmtpCommandException (SmtpErrorCode.UnexpectedStatusCode, response.StatusCode, response.Response);
			} else {
				// Clear the extensions except STARTTLS so that this capability stays set after a STARTTLS command.
				capabilities &= SmtpCapabilities.StartTLS;
				AuthenticationMechanisms.Clear ();
				MaxSize = 0;

				var lines = response.Response.Split ('\n');
				for (int i = 0; i < lines.Length; i++) {
					// Outlook.com replies with "250-8bitmime" instead of "250-8BITMIME"
					// (strangely, it correctly capitalizes all other extensions...)
					var capability = lines[i].Trim ().ToUpperInvariant ();

					if (capability.StartsWith ("AUTH", StringComparison.Ordinal) || capability.StartsWith ("X-EXPS", StringComparison.Ordinal)) {
						int index = capability[0] == 'A' ? "AUTH".Length : "X-EXPS".Length;

						if (index < capability.Length && (capability[index] == ' ' || capability[index] == '=')) {
							capabilities |= SmtpCapabilities.Authentication;
							index++;

							var mechanisms = capability.Substring (index);
							foreach (var mechanism in mechanisms.Split (new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
								AuthenticationMechanisms.Add (mechanism);
						}
					} else if (capability.StartsWith ("SIZE", StringComparison.Ordinal)) {
						int index = 4;
						uint size;

						capabilities |= SmtpCapabilities.Size;

						while (index < capability.Length && char.IsWhiteSpace (capability[index]))
							index++;

						if (uint.TryParse (capability.Substring (index), NumberStyles.None, CultureInfo.InvariantCulture, out size))
							MaxSize = size;
					} else if (capability == "DSN") {
						capabilities |= SmtpCapabilities.Dsn;
					} else if (capability == "BINARYMIME") {
						capabilities |= SmtpCapabilities.BinaryMime;
					} else if (capability == "CHUNKING") {
						capabilities |= SmtpCapabilities.Chunking;
					} else if (capability == "ENHANCEDSTATUSCODES") {
						capabilities |= SmtpCapabilities.EnhancedStatusCodes;
					} else if (capability == "8BITMIME") {
						capabilities |= SmtpCapabilities.EightBitMime;
					} else if (capability == "PIPELINING") {
						capabilities |= SmtpCapabilities.Pipelining;
					} else if (capability == "STARTTLS") {
						capabilities |= SmtpCapabilities.StartTLS;
					} else if (capability == "SMTPUTF8") {
						capabilities |= SmtpCapabilities.UTF8;
					}
				}
			}
		}

		async Task AuthenticateAsync (SaslMechanism mechanism, bool doAsync, CancellationToken cancellationToken)
		{
			if (mechanism == null)
				throw new ArgumentNullException (nameof (mechanism));

			CheckDisposed ();

			if (!IsConnected)
				throw new ServiceNotConnectedException ("The SmtpClient must be connected before you can authenticate.");

			if (IsAuthenticated)
				throw new InvalidOperationException ("The SmtpClient is already authenticated.");

			if ((capabilities & SmtpCapabilities.Authentication) == 0)
				throw new NotSupportedException ("The SMTP server does not support authentication.");

			cancellationToken.ThrowIfCancellationRequested ();

			SmtpResponse response;
			string challenge;
			string command;

			mechanism.Uri = new Uri ($"smtp://{uri.Host}");

			// send an initial challenge if the mechanism supports it
			if (mechanism.SupportsInitialResponse) {
				challenge = mechanism.Challenge (null);
				command = string.Format ("AUTH {0} {1}", mechanism.MechanismName, challenge);
			} else {
				command = string.Format ("AUTH {0}", mechanism.MechanismName);
			}

			response = await SendCommandAsync (command, doAsync, cancellationToken).ConfigureAwait (false);

			if (response.StatusCode == SmtpStatusCode.AuthenticationMechanismTooWeak)
				throw new AuthenticationException (response.Response);

			SaslException saslException = null;

			try {
				while (!mechanism.IsAuthenticated) {
					if (response.StatusCode != SmtpStatusCode.AuthenticationChallenge)
						break;

					challenge = mechanism.Challenge (response.Response);
					response = await SendCommandAsync (challenge, doAsync, cancellationToken).ConfigureAwait (false);
				}

				saslException = null;
			} catch (SaslException ex) {
				// reset the authentication state
				response = await SendCommandAsync (string.Empty, doAsync, cancellationToken).ConfigureAwait (false);
				saslException = ex;
			}

			if (response.StatusCode == SmtpStatusCode.AuthenticationSuccessful) {
				if (mechanism.NegotiatedSecurityLayer)
					await EhloAsync (doAsync, cancellationToken).ConfigureAwait (false);
				authenticated = true;
				OnAuthenticated (response.Response);
				return;
			}

			var message = string.Format ("{0}: {1}", (int) response.StatusCode, response.Response);

			if (saslException != null)
				throw new AuthenticationException (message, saslException);

			throw new AuthenticationException (message);
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
		/// <exception cref="ServiceNotConnectedException">
		/// The <see cref="SmtpClient"/> is not connected.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="SmtpClient"/> is already authenticated.
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The SMTP server does not support authentication.
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
		/// <exception cref="SmtpCommandException">
		/// The SMTP command failed.
		/// </exception>
		/// <exception cref="SmtpProtocolException">
		/// An SMTP protocol error occurred.
		/// </exception>
		public override void Authenticate (SaslMechanism mechanism, CancellationToken cancellationToken = default (CancellationToken))
		{
			AuthenticateAsync (mechanism, false, cancellationToken).GetAwaiter ().GetResult ();
		}

		async Task AuthenticateAsync (Encoding encoding, ICredentials credentials, bool doAsync, CancellationToken cancellationToken)
		{
			if (encoding == null)
				throw new ArgumentNullException (nameof (encoding));

			if (credentials == null)
				throw new ArgumentNullException (nameof (credentials));

			CheckDisposed ();

			if (!IsConnected)
				throw new ServiceNotConnectedException ("The SmtpClient must be connected before you can authenticate.");

			if (IsAuthenticated)
				throw new InvalidOperationException ("The SmtpClient is already authenticated.");

			if ((capabilities & SmtpCapabilities.Authentication) == 0)
				throw new NotSupportedException ("The SMTP server does not support authentication.");

			var saslUri = new Uri ($"smtp://{uri.Host}");
			AuthenticationException authException = null;
			SmtpResponse response;
			SaslMechanism sasl;
			bool tried = false;
			string challenge;
			string command;

			foreach (var authmech in SaslMechanism.AuthMechanismRank) {
				if (!AuthenticationMechanisms.Contains (authmech))
					continue;

				if ((sasl = SaslMechanism.Create (authmech, saslUri, encoding, credentials)) == null)
					continue;

				tried = true;

				cancellationToken.ThrowIfCancellationRequested ();

				// send an initial challenge if the mechanism supports it
				if (sasl.SupportsInitialResponse) {
					challenge = sasl.Challenge (null);
					command = string.Format ("AUTH {0} {1}", authmech, challenge);
				} else {
					command = string.Format ("AUTH {0}", authmech);
				}

				response = await SendCommandAsync (command, doAsync, cancellationToken).ConfigureAwait (false);

				if (response.StatusCode == SmtpStatusCode.AuthenticationMechanismTooWeak)
					continue;

				SaslException saslException = null;

				try {
					while (!sasl.IsAuthenticated) {
						if (response.StatusCode != SmtpStatusCode.AuthenticationChallenge)
							break;

						challenge = sasl.Challenge (response.Response);
						response = await SendCommandAsync (challenge, doAsync, cancellationToken).ConfigureAwait (false);
					}

					saslException = null;
				} catch (SaslException ex) {
					// reset the authentication state
					response = await SendCommandAsync (string.Empty, doAsync, cancellationToken).ConfigureAwait (false);
					saslException = ex;
				}

				if (response.StatusCode == SmtpStatusCode.AuthenticationSuccessful) {
					if (sasl.NegotiatedSecurityLayer)
						await EhloAsync (doAsync, cancellationToken).ConfigureAwait (false);
					authenticated = true;
					OnAuthenticated (response.Response);
					return;
				}

				var message = string.Format ("{0}: {1}", (int) response.StatusCode, response.Response);
				Exception inner;

				if (saslException != null)
					inner = new SmtpCommandException (SmtpErrorCode.UnexpectedStatusCode, response.StatusCode, response.Response, saslException);
				else
					inner = new SmtpCommandException (SmtpErrorCode.UnexpectedStatusCode, response.StatusCode, response.Response);

				authException = new AuthenticationException (message, inner);
			}

			if (tried)
				throw authException ?? new AuthenticationException ();

			throw new NotSupportedException ("No compatible authentication mechanisms found.");
		}

		/// <summary>
		/// Authenticate using the supplied credentials.
		/// </summary>
		/// <remarks>
		/// <para>If the SMTP server supports authentication, then the SASL mechanisms
		/// that both the client and server support are tried in order of greatest
		/// security to weakest security. Once a SASL authentication mechanism is
		/// found that both client and server support, the credentials are used to
		/// authenticate.</para>
		/// <para>If, on the other hand, authentication is not supported by the SMTP
		/// server, then this method will throw <see cref="System.NotSupportedException"/>.
		/// The <see cref="Capabilities"/> property can be checked for the
		/// <see cref="SmtpCapabilities.Authentication"/> flag to make sure the
		/// SMTP server supports authentication before calling this method.</para>
		/// <note type="tip"> To prevent the usage of certain authentication mechanisms,
		/// simply remove them from the <see cref="AuthenticationMechanisms"/> hash set
		/// before calling this method.</note>
		/// </remarks>
		/// <param name="encoding">The text encoding to use for the user's credentials.</param>
		/// <param name="credentials">The user's credentials.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="encoding"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="credentials"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="ServiceNotConnectedException">
		/// The <see cref="SmtpClient"/> is not connected.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="SmtpClient"/> is already authenticated.
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The SMTP server does not support authentication.
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
		/// <exception cref="SmtpCommandException">
		/// The SMTP command failed.
		/// </exception>
		/// <exception cref="SmtpProtocolException">
		/// An SMTP protocol error occurred.
		/// </exception>
		public override void Authenticate (Encoding encoding, ICredentials credentials, CancellationToken cancellationToken = default (CancellationToken))
		{
			AuthenticateAsync (encoding, credentials, false, cancellationToken).GetAwaiter ().GetResult ();
		}

		internal void ReplayConnect (string host, Stream replayStream, CancellationToken cancellationToken = default (CancellationToken))
		{
			CheckDisposed ();

			if (host == null)
				throw new ArgumentNullException (nameof (host));

			if (replayStream == null)
				throw new ArgumentNullException (nameof (replayStream));

			Stream = new SmtpStream (replayStream, ProtocolLogger);
			capabilities = SmtpCapabilities.None;
			AuthenticationMechanisms.Clear ();
			uri = new Uri ($"smtp://{host}:25");
			secure = false;
			MaxSize = 0;

			try {
				// read the greeting
				var response = Stream.ReadResponse (cancellationToken);

				if (response.StatusCode != SmtpStatusCode.ServiceReady)
					throw new SmtpCommandException (SmtpErrorCode.UnexpectedStatusCode, response.StatusCode, response.Response);

				// Send EHLO and get a list of supported extensions
				EhloAsync (false, cancellationToken).GetAwaiter ().GetResult ();

				connected = true;
			} catch {
				Stream.Dispose ();
				Stream = null;
				throw;
			}

			OnConnected (host, 25, SecureSocketOptions.None);
		}

		internal async Task ReplayConnectAsync (string host, Stream replayStream, CancellationToken cancellationToken = default (CancellationToken))
		{
			CheckDisposed ();

			if (host == null)
				throw new ArgumentNullException (nameof (host));

			if (replayStream == null)
				throw new ArgumentNullException (nameof (replayStream));

			Stream = new SmtpStream (replayStream, ProtocolLogger);
			capabilities = SmtpCapabilities.None;
			AuthenticationMechanisms.Clear ();
			uri = new Uri ($"smtp://{host}:25");
			secure = false;
			MaxSize = 0;

			try {
				// read the greeting
				var response = await Stream.ReadResponseAsync (cancellationToken).ConfigureAwait (false);

				if (response.StatusCode != SmtpStatusCode.ServiceReady)
					throw new SmtpCommandException (SmtpErrorCode.UnexpectedStatusCode, response.StatusCode, response.Response);

				// Send EHLO and get a list of supported extensions
				await EhloAsync (true, cancellationToken).ConfigureAwait (false);

				connected = true;
			} catch {
				Stream.Dispose ();
				Stream = null;
				throw;
			}

			OnConnected (host, 25, SecureSocketOptions.None);
		}

		internal static void ComputeDefaultValues (string host, ref int port, ref SecureSocketOptions options, out Uri uri, out bool starttls)
		{
			switch (options) {
			default:
				if (port == 0)
					port = 25;
				break;
			case SecureSocketOptions.Auto:
				switch (port) {
				case 0: port = 25; goto default;
				case 465: options = SecureSocketOptions.SslOnConnect; break;
				default: options = SecureSocketOptions.StartTlsWhenAvailable; break;
				}
				break;
			case SecureSocketOptions.SslOnConnect:
				if (port == 0)
					port = 465;
				break;
			}

			switch (options) {
			case SecureSocketOptions.StartTlsWhenAvailable:
				uri = new Uri ("smtp://" + host + ":" + port + "/?starttls=when-available");
				starttls = true;
				break;
			case SecureSocketOptions.StartTls:
				uri = new Uri ("smtp://" + host + ":" + port + "/?starttls=always");
				starttls = true;
				break;
			case SecureSocketOptions.SslOnConnect:
				uri = new Uri ("smtps://" + host + ":" + port);
				starttls = false;
				break;
			default:
				uri = new Uri ("smtp://" + host + ":" + port);
				starttls = false;
				break;
			}
		}

		async Task ConnectAsync (string host, int port, SecureSocketOptions options, bool doAsync, CancellationToken cancellationToken)
		{
			if (host == null)
				throw new ArgumentNullException (nameof (host));

			if (host.Length == 0)
				throw new ArgumentException ("The host name cannot be empty.", nameof (host));

			if (port < 0 || port > 65535)
				throw new ArgumentOutOfRangeException (nameof (port));

			CheckDisposed ();

			if (IsConnected)
				throw new InvalidOperationException ("The SmtpClient is already connected.");

			capabilities = SmtpCapabilities.None;
			AuthenticationMechanisms.Clear ();
			MaxSize = 0;

			SmtpResponse response;
			Stream stream;
			bool starttls;

			ComputeDefaultValues (host, ref port, ref options, out uri, out starttls);

			var socket = await ConnectSocket (host, port, doAsync, cancellationToken).ConfigureAwait (false);

			if (options == SecureSocketOptions.SslOnConnect) {
				var ssl = new SslStream (new NetworkStream (socket, true), false, ValidateRemoteCertificate);

				try {
					if (doAsync) {
						await ssl.AuthenticateAsClientAsync (host, ClientCertificates, SslProtocols, CheckCertificateRevocation).ConfigureAwait (false);
					} else {
#if NETSTANDARD1_3 || NETSTANDARD1_6
						ssl.AuthenticateAsClientAsync (host, ClientCertificates, SslProtocols, CheckCertificateRevocation).GetAwaiter ().GetResult ();
#else
						ssl.AuthenticateAsClient (host, ClientCertificates, SslProtocols, CheckCertificateRevocation);
#endif
					}
				} catch (Exception ex) {
					ssl.Dispose ();

					throw SslHandshakeException.Create (this, ex, false);
				}

				secure = true;
				stream = ssl;
			} else {
				stream = new NetworkStream (socket, true);
				secure = false;
			}

			if (stream.CanTimeout) {
				stream.WriteTimeout = timeout;
				stream.ReadTimeout = timeout;
			}

			Stream = new SmtpStream (stream, ProtocolLogger);

			try {
				ProtocolLogger.LogConnect (uri);

				// read the greeting
				if (doAsync)
					response = await Stream.ReadResponseAsync (cancellationToken).ConfigureAwait (false);
				else
					response = Stream.ReadResponse (cancellationToken);

				if (response.StatusCode != SmtpStatusCode.ServiceReady)
					throw new SmtpCommandException (SmtpErrorCode.UnexpectedStatusCode, response.StatusCode, response.Response);

				// Send EHLO and get a list of supported extensions
				await EhloAsync (doAsync, cancellationToken).ConfigureAwait (false);

				if (options == SecureSocketOptions.StartTls && (capabilities & SmtpCapabilities.StartTLS) == 0)
					throw new NotSupportedException ("The SMTP server does not support the STARTTLS extension.");

				if (starttls && (capabilities & SmtpCapabilities.StartTLS) != 0) {
					response = await SendCommandAsync ("STARTTLS", doAsync, cancellationToken).ConfigureAwait (false);
					if (response.StatusCode != SmtpStatusCode.ServiceReady)
						throw new SmtpCommandException (SmtpErrorCode.UnexpectedStatusCode, response.StatusCode, response.Response);

					try {
						var tls = new SslStream (stream, false, ValidateRemoteCertificate);
						Stream.Stream = tls;

						if (doAsync) {
							await tls.AuthenticateAsClientAsync (host, ClientCertificates, SslProtocols, CheckCertificateRevocation).ConfigureAwait (false);
						} else {
#if NETSTANDARD1_3 || NETSTANDARD1_6
							tls.AuthenticateAsClientAsync (host, ClientCertificates, SslProtocols, CheckCertificateRevocation).GetAwaiter ().GetResult ();
#else
							tls.AuthenticateAsClient (host, ClientCertificates, SslProtocols, CheckCertificateRevocation);
#endif
						}
					} catch (Exception ex) {
						throw SslHandshakeException.Create (this, ex, true);
					}

					secure = true;

					// Send EHLO again and get the new list of supported extensions
					await EhloAsync (doAsync, cancellationToken).ConfigureAwait (false);
				}

				connected = true;
			} catch {
				Stream.Dispose ();
				secure = false;
				Stream = null;
				throw;
			}

			OnConnected (host, port, options);
		}

		/// <summary>
		/// Establish a connection to the specified SMTP or SMTP/S server.
		/// </summary>
		/// <remarks>
		/// <para>Establishes a connection to the specified SMTP or SMTP/S server.</para>
		/// <para>If the <paramref name="port"/> has a value of <c>0</c>, then the
		/// <paramref name="options"/> parameter is used to determine the default port to
		/// connect to. The default port used with <see cref="SecureSocketOptions.SslOnConnect"/>
		/// is <c>465</c>. All other values will use a default port of <c>25</c>.</para>
		/// <para>If the <paramref name="options"/> has a value of
		/// <see cref="SecureSocketOptions.Auto"/>, then the <paramref name="port"/> is used
		/// to determine the default security options. If the <paramref name="port"/> has a value
		/// of <c>465</c>, then the default options used will be
		/// <see cref="SecureSocketOptions.SslOnConnect"/>. All other values will use
		/// <see cref="SecureSocketOptions.StartTlsWhenAvailable"/>.</para>
		/// <para>Once a connection is established, properties such as
		/// <see cref="AuthenticationMechanisms"/> and <see cref="Capabilities"/> will be
		/// populated.</para>
		/// <note type="note">The connection established by any of the
		/// <a href="Overload_MailKit_Net_Smtp_SmtpClient_Connect.htm">Connect</a>
		/// methods may be re-used if an application wishes to send multiple messages
		/// to the same SMTP server. Since connecting and authenticating can be expensive
		/// operations, re-using a connection can significantly improve performance when
		/// sending a large number of messages to the same SMTP server over a short
		/// period of time.</note>
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
		/// The <see cref="SmtpClient"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="SmtpClient"/> is already connected.
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// <paramref name="options"/> was set to
		/// <see cref="MailKit.Security.SecureSocketOptions.StartTls"/>
		/// and the SMTP server does not support the STARTTLS extension.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled.
		/// </exception>
		/// <exception cref="System.Net.Sockets.SocketException">
		/// A socket error occurred trying to connect to the remote host.
		/// </exception>
		/// <exception cref="SslHandshakeException">
		/// An error occurred during the SSL/TLS negotiations.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="SmtpCommandException">
		/// An SMTP command failed.
		/// </exception>
		/// <exception cref="SmtpProtocolException">
		/// An SMTP protocol error occurred.
		/// </exception>
		public override void Connect (string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default (CancellationToken))
		{
			ConnectAsync (host, port, options, false, cancellationToken).GetAwaiter ().GetResult ();
		}

		async Task ConnectAsync (Stream stream, string host, int port, SecureSocketOptions options, bool doAsync, CancellationToken cancellationToken)
		{
			if (stream == null)
				throw new ArgumentNullException (nameof (stream));

			if (host == null)
				throw new ArgumentNullException (nameof (host));

			if (host.Length == 0)
				throw new ArgumentException ("The host name cannot be empty.", nameof (host));

			if (port < 0 || port > 65535)
				throw new ArgumentOutOfRangeException (nameof (port));

			CheckDisposed ();

			if (IsConnected)
				throw new InvalidOperationException ("The SmtpClient is already connected.");

			capabilities = SmtpCapabilities.None;
			AuthenticationMechanisms.Clear ();
			MaxSize = 0;

			SmtpResponse response;
			Stream network;
			bool starttls;

			ComputeDefaultValues (host, ref port, ref options, out uri, out starttls);

			if (options == SecureSocketOptions.SslOnConnect) {
				var ssl = new SslStream (stream, false, ValidateRemoteCertificate);

				try {
					if (doAsync) {
						await ssl.AuthenticateAsClientAsync (host, ClientCertificates, SslProtocols, CheckCertificateRevocation).ConfigureAwait (false);
					} else {
#if NETSTANDARD1_3 || NETSTANDARD1_6
						ssl.AuthenticateAsClientAsync (host, ClientCertificates, SslProtocols, CheckCertificateRevocation).GetAwaiter ().GetResult ();
#else
						ssl.AuthenticateAsClient (host, ClientCertificates, SslProtocols, CheckCertificateRevocation);
#endif
					}
				} catch (Exception ex) {
					ssl.Dispose ();

					throw SslHandshakeException.Create (this, ex, false);
				}

				network = ssl;
				secure = true;
			} else {
				network = stream;
				secure = false;
			}

			if (network.CanTimeout) {
				network.WriteTimeout = timeout;
				network.ReadTimeout = timeout;
			}

			Stream = new SmtpStream (network, ProtocolLogger);

			try {
				ProtocolLogger.LogConnect (uri);

				// read the greeting
				if (doAsync)
					response = await Stream.ReadResponseAsync (cancellationToken).ConfigureAwait (false);
				else
					response = Stream.ReadResponse (cancellationToken);

				if (response.StatusCode != SmtpStatusCode.ServiceReady)
					throw new SmtpCommandException (SmtpErrorCode.UnexpectedStatusCode, response.StatusCode, response.Response);

				// Send EHLO and get a list of supported extensions
				await EhloAsync (doAsync, cancellationToken).ConfigureAwait (false);

				if (options == SecureSocketOptions.StartTls && (capabilities & SmtpCapabilities.StartTLS) == 0)
					throw new NotSupportedException ("The SMTP server does not support the STARTTLS extension.");

				if (starttls && (capabilities & SmtpCapabilities.StartTLS) != 0) {
					response = await SendCommandAsync ("STARTTLS", doAsync, cancellationToken).ConfigureAwait (false);
					if (response.StatusCode != SmtpStatusCode.ServiceReady)
						throw new SmtpCommandException (SmtpErrorCode.UnexpectedStatusCode, response.StatusCode, response.Response);

					var tls = new SslStream (network, false, ValidateRemoteCertificate);
					Stream.Stream = tls;

					try {
						if (doAsync) {
							await tls.AuthenticateAsClientAsync (host, ClientCertificates, SslProtocols, CheckCertificateRevocation).ConfigureAwait (false);
						} else {
#if NETSTANDARD1_3 || NETSTANDARD1_6
							tls.AuthenticateAsClientAsync (host, ClientCertificates, SslProtocols, CheckCertificateRevocation).GetAwaiter ().GetResult ();
#else
							tls.AuthenticateAsClient (host, ClientCertificates, SslProtocols, CheckCertificateRevocation);
#endif
						}
					} catch (Exception ex) {
						throw SslHandshakeException.Create (this, ex, true);
					}

					secure = true;

					// Send EHLO again and get the new list of supported extensions
					await EhloAsync (doAsync, cancellationToken).ConfigureAwait (false);
				}

				connected = true;
			} catch {
				Stream.Dispose ();
				secure = false;
				Stream = null;
				throw;
			}

			OnConnected (host, port, options);
		}

		Task ConnectAsync (Socket socket, string host, int port, SecureSocketOptions options, bool doAsync, CancellationToken cancellationToken)
		{
			if (socket == null)
				throw new ArgumentNullException (nameof (socket));

			if (!socket.Connected)
				throw new ArgumentException ("The socket is not connected.", nameof (socket));

			return ConnectAsync (new NetworkStream (socket, true), host, port, options, doAsync, cancellationToken);
		}

		/// <summary>
		/// Establish a connection to the specified SMTP or SMTP/S server using the provided socket.
		/// </summary>
		/// <remarks>
		/// <para>Establishes a connection to the specified SMTP or SMTP/S server using the provided socket.</para>
		/// <para>If the <paramref name="options"/> has a value of
		/// <see cref="SecureSocketOptions.Auto"/>, then the <paramref name="port"/> is used
		/// to determine the default security options. If the <paramref name="port"/> has a value
		/// of <c>465</c>, then the default options used will be
		/// <see cref="SecureSocketOptions.SslOnConnect"/>. All other values will use
		/// <see cref="SecureSocketOptions.StartTlsWhenAvailable"/>.</para>
		/// <para>Once a connection is established, properties such as
		/// <see cref="AuthenticationMechanisms"/> and <see cref="Capabilities"/> will be
		/// populated.</para>
		/// <note type="info">With the exception of using the <paramref name="port"/> to determine the
		/// default <see cref="SecureSocketOptions"/> to use when the <paramref name="options"/> value
		/// is <see cref="SecureSocketOptions.Auto"/>, the <paramref name="host"/> and
		/// <paramref name="port"/> parameters are only used for logging purposes.</note>
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
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="SmtpClient"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="SmtpClient"/> is already connected.
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// <paramref name="options"/> was set to
		/// <see cref="MailKit.Security.SecureSocketOptions.StartTls"/>
		/// and the SMTP server does not support the STARTTLS extension.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled.
		/// </exception>
		/// <exception cref="SslHandshakeException">
		/// An error occurred during the SSL/TLS negotiations.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="SmtpCommandException">
		/// An SMTP command failed.
		/// </exception>
		/// <exception cref="SmtpProtocolException">
		/// An SMTP protocol error occurred.
		/// </exception>
		public override void Connect (Socket socket, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default (CancellationToken))
		{
			ConnectAsync (socket, host, port, options, false, cancellationToken).GetAwaiter ().GetResult ();
		}

		/// <summary>
		/// Establish a connection to the specified SMTP or SMTP/S server using the provided stream.
		/// </summary>
		/// <remarks>
		/// <para>Establishes a connection to the specified SMTP or SMTP/S server using the provided stream.</para>
		/// <para>If the <paramref name="options"/> has a value of
		/// <see cref="SecureSocketOptions.Auto"/>, then the <paramref name="port"/> is used
		/// to determine the default security options. If the <paramref name="port"/> has a value
		/// of <c>465</c>, then the default options used will be
		/// <see cref="SecureSocketOptions.SslOnConnect"/>. All other values will use
		/// <see cref="SecureSocketOptions.StartTlsWhenAvailable"/>.</para>
		/// <para>Once a connection is established, properties such as
		/// <see cref="AuthenticationMechanisms"/> and <see cref="Capabilities"/> will be
		/// populated.</para>
		/// <note type="info">With the exception of using the <paramref name="port"/> to determine the
		/// default <see cref="SecureSocketOptions"/> to use when the <paramref name="options"/> value
		/// is <see cref="SecureSocketOptions.Auto"/>, the <paramref name="host"/> and
		/// <paramref name="port"/> parameters are only used for logging purposes.</note>
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
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="SmtpClient"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="SmtpClient"/> is already connected.
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// <paramref name="options"/> was set to
		/// <see cref="MailKit.Security.SecureSocketOptions.StartTls"/>
		/// and the SMTP server does not support the STARTTLS extension.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled.
		/// </exception>
		/// <exception cref="SslHandshakeException">
		/// An error occurred during the SSL/TLS negotiations.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="SmtpCommandException">
		/// An SMTP command failed.
		/// </exception>
		/// <exception cref="SmtpProtocolException">
		/// An SMTP protocol error occurred.
		/// </exception>
		public override void Connect (Stream stream, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default (CancellationToken))
		{
			ConnectAsync (stream, host, port, options, false, cancellationToken).GetAwaiter ().GetResult ();
		}

		async Task DisconnectAsync (bool quit, bool doAsync, CancellationToken cancellationToken)
		{
			CheckDisposed ();

			if (!IsConnected)
				return;

			if (quit) {
				try {
					await SendCommandAsync ("QUIT", doAsync, cancellationToken).ConfigureAwait (false);
				} catch (OperationCanceledException) {
				} catch (SmtpProtocolException) {
				} catch (SmtpCommandException) {
				} catch (IOException) {
				}
			}

			Disconnect (uri.Host, uri.Port, GetSecureSocketOptions (uri), true);
		}

		/// <summary>
		/// Disconnect the service.
		/// </summary>
		/// <remarks>
		/// If <paramref name="quit"/> is <c>true</c>, a <c>QUIT</c> command will be issued in order to disconnect cleanly.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\SmtpExamples.cs" region="SendMessage"/>
		/// </example>
		/// <param name="quit">If set to <c>true</c>, a <c>QUIT</c> command will be issued in order to disconnect cleanly.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="SmtpClient"/> has been disposed.
		/// </exception>
		public override void Disconnect (bool quit, CancellationToken cancellationToken = default (CancellationToken))
		{
			DisconnectAsync (quit, false, cancellationToken).GetAwaiter ().GetResult ();
		}

		async Task NoOpAsync (bool doAsync, CancellationToken cancellationToken)
		{
			CheckDisposed ();

			if (!IsConnected)
				throw new ServiceNotConnectedException ("The SmtpClient is not connected.");

			SmtpResponse response;

			try {
				response = await SendCommandAsync ("NOOP", doAsync, cancellationToken).ConfigureAwait (false);
			} catch {
				Disconnect (uri.Host, uri.Port, GetSecureSocketOptions (uri), false);
				throw;
			}

			if (response.StatusCode != SmtpStatusCode.Ok)
				throw new SmtpCommandException (SmtpErrorCode.UnexpectedStatusCode, response.StatusCode, response.Response);
		}

		/// <summary>
		/// Ping the SMTP server to keep the connection alive.
		/// </summary>
		/// <remarks>Mail servers, if left idle for too long, will automatically drop the connection.</remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="SmtpClient"/> has been disposed.
		/// </exception>
		/// <exception cref="ServiceNotConnectedException">
		/// The <see cref="SmtpClient"/> is not connected.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="SmtpCommandException">
		/// The SMTP command failed.
		/// </exception>
		/// <exception cref="SmtpProtocolException">
		/// An SMTP protocol error occurred.
		/// </exception>
		public override void NoOp (CancellationToken cancellationToken = default (CancellationToken))
		{
			NoOpAsync (false, cancellationToken).GetAwaiter ().GetResult ();
		}

		void Disconnect (string host, int port, SecureSocketOptions options, bool requested)
		{
			capabilities = SmtpCapabilities.None;
			authenticated = false;
			connected = false;
			secure = false;
			uri = null;

			if (Stream != null) {
				Stream.Dispose ();
				Stream = null;
			}

			if (host != null)
				OnDisconnected (host, port, options, requested);
		}

		#endregion

		#region IMailTransport implementation

		static MailboxAddress GetMessageSender (MimeMessage message)
		{
			if (message.ResentSender != null)
				return message.ResentSender;

			if (message.ResentFrom.Count > 0)
				return message.ResentFrom.Mailboxes.FirstOrDefault ();

			if (message.Sender != null)
				return message.Sender;

			return message.From.Mailboxes.FirstOrDefault ();
		}

		static void AddUnique (IList<MailboxAddress> recipients, HashSet<string> unique, IEnumerable<MailboxAddress> mailboxes)
		{
			foreach (var mailbox in mailboxes) {
				if (unique.Add (mailbox.Address))
					recipients.Add (mailbox);
			}
		}

		static IList<MailboxAddress> GetMessageRecipients (MimeMessage message)
		{
			var unique = new HashSet<string> (StringComparer.OrdinalIgnoreCase);
			var recipients = new List<MailboxAddress> ();

			if (message.ResentSender != null || message.ResentFrom.Count > 0) {
				AddUnique (recipients, unique, message.ResentTo.Mailboxes);
				AddUnique (recipients, unique, message.ResentCc.Mailboxes);
				AddUnique (recipients, unique, message.ResentBcc.Mailboxes);
			} else {
				AddUnique (recipients, unique, message.To.Mailboxes);
				AddUnique (recipients, unique, message.Cc.Mailboxes);
				AddUnique (recipients, unique, message.Bcc.Mailboxes);
			}

			return recipients;
		}

		[Flags]
		enum SmtpExtension {
			None         = 0,
			EightBitMime = 1 << 0,
			BinaryMime   = 1 << 1,
			UTF8         = 1 << 2,
		}

		class ContentTransferEncodingVisitor : MimeVisitor
		{
			readonly SmtpCapabilities capabilities;

			public ContentTransferEncodingVisitor (SmtpCapabilities capabilities)
			{
				this.capabilities = capabilities;
			}

			public SmtpExtension SmtpExtensions {
				get; private set;
			}

			protected override void VisitMimePart (MimePart entity)
			{
				switch (entity.ContentTransferEncoding) {
				case ContentEncoding.EightBit:
					if ((capabilities & SmtpCapabilities.EightBitMime) != 0)
						SmtpExtensions |= SmtpExtension.EightBitMime;
					break;
				case ContentEncoding.Binary:
					if ((capabilities & SmtpCapabilities.BinaryMime) != 0)
						SmtpExtensions |= SmtpExtension.BinaryMime;
					break;
				}
			}
		}

		/// <summary>
		/// Invoked when the sender is accepted by the SMTP server.
		/// </summary>
		/// <remarks>
		/// The default implementation does nothing.
		/// </remarks>
		/// <param name="message">The message being sent.</param>
		/// <param name="mailbox">The mailbox used in the <c>MAIL FROM</c> command.</param>
		/// <param name="response">The response to the <c>MAIL FROM</c> command.</param>
		protected virtual void OnSenderAccepted (MimeMessage message, MailboxAddress mailbox, SmtpResponse response)
		{
		}

		/// <summary>
		/// Invoked when a recipient is not accepted by the SMTP server.
		/// </summary>
		/// <remarks>
		/// The default implementation throws an appropriate <see cref="SmtpCommandException"/>.
		/// </remarks>
		/// <param name="message">The message being sent.</param>
		/// <param name="mailbox">The mailbox used in the <c>MAIL FROM</c> command.</param>
		/// <param name="response">The response to the <c>MAIL FROM</c> command.</param>
		protected virtual void OnSenderNotAccepted (MimeMessage message, MailboxAddress mailbox, SmtpResponse response)
		{
			throw new SmtpCommandException (SmtpErrorCode.SenderNotAccepted, response.StatusCode, mailbox, response.Response);
		}

		void ProcessMailFromResponse (MimeMessage message, MailboxAddress mailbox, SmtpResponse response)
		{
			if (response.StatusCode >= SmtpStatusCode.Ok && response.StatusCode < (SmtpStatusCode) 260) {
				OnSenderAccepted (message, mailbox, response);
				return;
			}

			if (response.StatusCode == SmtpStatusCode.AuthenticationRequired)
				throw new ServiceNotAuthenticatedException (response.Response);

			OnSenderNotAccepted (message, mailbox, response);
		}

		/// <summary>
		/// Get the envelope identifier to be used with delivery status notifications.
		/// </summary>
		/// <remarks>
		/// <para>The envelope identifier, if non-empty, is useful in determining which message a delivery
		/// status notification was issued for.</para>
		/// <para>The envelope identifier should be unique and may be up to 100 characters in length, but
		/// must consist only of printable ASCII characters and no white space.</para>
		/// <para>For more information, see
		/// <a href="https://tools.ietf.org/html/rfc3461#section-4.4">rfc3461, section 4.4</a>.</para>
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\SmtpExamples.cs" region="DeliveryStatusNotification"/>
		/// </example>
		/// <returns>The envelope identifier.</returns>
		/// <param name="message">The message.</param>
		protected virtual string GetEnvelopeId (MimeMessage message)
		{
			return null;
		}

		/// <summary>
		/// Get or set how much of the message to include in any failed delivery status notifications.
		/// </summary>
		/// <remarks>
		/// Gets or sets how much of the message to include in any failed delivery status notifications.
		/// </remarks>
		/// <value>A value indicating how much of the message to include in a failure delivery status notification.</value>
		public DeliveryStatusNotificationType DeliveryStatusNotificationType {
			get; set;
		}

		static string GetAddrspec (FormatOptions options, MailboxAddress mailbox)
		{
			if (options.International)
				return MailboxAddress.DecodeAddrspec (mailbox.Address);

			return MailboxAddress.EncodeAddrspec (mailbox.Address);
		}

		async Task MailFromAsync (FormatOptions options, MimeMessage message, MailboxAddress mailbox, SmtpExtension extensions, long size, bool doAsync, CancellationToken cancellationToken)
		{
			var builder = new StringBuilder ("MAIL FROM:<");

			var addrspec = GetAddrspec (options, mailbox);
			builder.Append (addrspec);
			builder.Append ('>');

			if ((extensions & SmtpExtension.UTF8) != 0)
				builder.Append (" SMTPUTF8");

			if ((Capabilities & SmtpCapabilities.Size) != 0 && size != -1)
				builder.AppendFormat (CultureInfo.InvariantCulture, " SIZE={0}", size);

			if ((extensions & SmtpExtension.BinaryMime) != 0)
				builder.Append (" BODY=BINARYMIME");
			else if ((extensions & SmtpExtension.EightBitMime) != 0)
				builder.Append (" BODY=8BITMIME");

			if ((capabilities & SmtpCapabilities.Dsn) != 0) {
				var envid = GetEnvelopeId (message);

				if (!string.IsNullOrEmpty (envid)) {
					builder.Append (" ENVID=");
					builder.Append (envid);
				}

				switch (DeliveryStatusNotificationType) {
				case DeliveryStatusNotificationType.HeadersOnly:
					builder.Append (" RET=HDRS");
					break;
				case DeliveryStatusNotificationType.Full:
					builder.Append (" RET=FULL");
					break;
				}
			}

			var command = builder.ToString ();

			if ((capabilities & SmtpCapabilities.Pipelining) != 0) {
				await QueueCommandAsync (SmtpCommand.MailFrom, command, doAsync, cancellationToken).ConfigureAwait (false);
				return;
			}

			var response = await SendCommandAsync (command, doAsync, cancellationToken).ConfigureAwait (false);

			ProcessMailFromResponse (message, mailbox, response);
		}

		/// <summary>
		/// Invoked when a recipient is accepted by the SMTP server.
		/// </summary>
		/// <remarks>
		/// The default implementation does nothing.
		/// </remarks>
		/// <param name="message">The message being sent.</param>
		/// <param name="mailbox">The mailbox used in the <c>RCPT TO</c> command.</param>
		/// <param name="response">The response to the <c>RCPT TO</c> command.</param>
		protected virtual void OnRecipientAccepted (MimeMessage message, MailboxAddress mailbox, SmtpResponse response)
		{
		}

		/// <summary>
		/// Invoked when a recipient is not accepted by the SMTP server.
		/// </summary>
		/// <remarks>
		/// The default implementation throws an appropriate <see cref="SmtpCommandException"/>.
		/// </remarks>
		/// <param name="message">The message being sent.</param>
		/// <param name="mailbox">The mailbox used in the <c>RCPT TO</c> command.</param>
		/// <param name="response">The response to the <c>RCPT TO</c> command.</param>
		protected virtual void OnRecipientNotAccepted (MimeMessage message, MailboxAddress mailbox, SmtpResponse response)
		{
			throw new SmtpCommandException (SmtpErrorCode.RecipientNotAccepted, response.StatusCode, mailbox, response.Response);
		}

		bool ProcessRcptToResponse (MimeMessage message, MailboxAddress mailbox, SmtpResponse response)
		{
			if (response.StatusCode < (SmtpStatusCode) 300) {
				OnRecipientAccepted (message, mailbox, response);
				return true;
			}

			if (response.StatusCode == SmtpStatusCode.AuthenticationRequired)
				throw new ServiceNotAuthenticatedException (response.Response);

			OnRecipientNotAccepted (message, mailbox, response);

			return false;
		}

		/// <summary>
		/// Get the types of delivery status notification desired for the specified recipient mailbox.
		/// </summary>
		/// <remarks>
		/// Gets the types of delivery status notification desired for the specified recipient mailbox.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\SmtpExamples.cs" region="DeliveryStatusNotification"/>
		/// </example>
		/// <returns>The desired delivery status notification type.</returns>
		/// <param name="message">The message being sent.</param>
		/// <param name="mailbox">The mailbox.</param>
		protected virtual DeliveryStatusNotification? GetDeliveryStatusNotifications (MimeMessage message, MailboxAddress mailbox)
		{
			return null;
		}

		static string GetNotifyString (DeliveryStatusNotification notify)
		{
			string value = string.Empty;

			if (notify == DeliveryStatusNotification.Never)
				return "NEVER";

			if ((notify & DeliveryStatusNotification.Success) != 0)
				value += "SUCCESS,";

			if ((notify & DeliveryStatusNotification.Failure) != 0)
				value += "FAILURE,";

			if ((notify & DeliveryStatusNotification.Delay) != 0)
				value += "DELAY";

			return value.TrimEnd (',');
		}

		async Task<bool> RcptToAsync (FormatOptions options, MimeMessage message, MailboxAddress mailbox, bool doAsync, CancellationToken cancellationToken)
		{
			var command = string.Format ("RCPT TO:<{0}>", GetAddrspec (options, mailbox));

			if ((capabilities & SmtpCapabilities.Dsn) != 0) {
				var notify = GetDeliveryStatusNotifications (message, mailbox);

				if (notify.HasValue)
					command += " NOTIFY=" + GetNotifyString (notify.Value);
			}

			if ((capabilities & SmtpCapabilities.Pipelining) != 0) {
				await QueueCommandAsync (SmtpCommand.RcptTo, command, doAsync, cancellationToken).ConfigureAwait (false);
				return false;
			}

			var response = await SendCommandAsync (command, doAsync, cancellationToken).ConfigureAwait (false);

			return ProcessRcptToResponse (message, mailbox, response);
		}

		class SendContext
		{
			readonly ITransferProgress progress;
			readonly long size;
			long nwritten;

			public SendContext (ITransferProgress progress, long size)
			{
				this.progress = progress;
				this.size = size;
			}

			public void Update (int n)
			{
				nwritten += n;

				if (size != -1)
					progress.Report (nwritten, size);
				else
					progress.Report (nwritten);
			}
		}

		async Task BdatAsync (FormatOptions options, MimeMessage message, long size, bool doAsync, CancellationToken cancellationToken, ITransferProgress progress)
		{
			SmtpResponse response;
			byte[] bytes;

			bytes = Encoding.UTF8.GetBytes (string.Format (CultureInfo.InvariantCulture, "BDAT {0} LAST\r\n", size));

			if (doAsync)
				await Stream.WriteAsync (bytes, 0, bytes.Length, cancellationToken).ConfigureAwait (false);
			else
				Stream.Write (bytes, 0, bytes.Length, cancellationToken);

			if (progress != null) {
				var ctx = new SendContext (progress, size);

				using (var stream = new ProgressStream (Stream, ctx.Update)) {
					if (doAsync) {
						await message.WriteToAsync (options, stream, cancellationToken).ConfigureAwait (false);
						await stream.FlushAsync (cancellationToken).ConfigureAwait (false);
					} else {
						message.WriteTo (options, stream, cancellationToken);
						stream.Flush (cancellationToken);
					}
				}
			} else if (doAsync) {
				await message.WriteToAsync (options, Stream, cancellationToken).ConfigureAwait (false);
				await Stream.FlushAsync (cancellationToken).ConfigureAwait (false);
			} else {
				message.WriteTo (options, Stream, cancellationToken);
				Stream.Flush (cancellationToken);
			}

			if (doAsync)
				response = await Stream.ReadResponseAsync (cancellationToken).ConfigureAwait (false);
			else
				response =  Stream.ReadResponse (cancellationToken);

			switch (response.StatusCode) {
			default:
				throw new SmtpCommandException (SmtpErrorCode.MessageNotAccepted, response.StatusCode, response.Response);
			case SmtpStatusCode.AuthenticationRequired:
				throw new ServiceNotAuthenticatedException (response.Response);
			case SmtpStatusCode.Ok:
				OnMessageSent (new MessageSentEventArgs (message, response.Response));
				break;
			}
		}

		async Task DataAsync (FormatOptions options, MimeMessage message, long size, bool doAsync, CancellationToken cancellationToken, ITransferProgress progress)
		{
			var response = await SendCommandAsync ("DATA", doAsync, cancellationToken).ConfigureAwait (false);

			if (response.StatusCode != SmtpStatusCode.StartMailInput)
				throw new SmtpCommandException (SmtpErrorCode.UnexpectedStatusCode, response.StatusCode, response.Response);

			if (progress != null) {
				var ctx = new SendContext (progress, size);

				using (var stream = new ProgressStream (Stream, ctx.Update)) {
					using (var filtered = new FilteredStream (stream)) {
						filtered.Add (new SmtpDataFilter ());

						if (doAsync) {
							await message.WriteToAsync (options, filtered, cancellationToken).ConfigureAwait (false);
							await filtered.FlushAsync (cancellationToken).ConfigureAwait (false);
						} else {
							message.WriteTo (options, filtered, cancellationToken);
							filtered.Flush (cancellationToken);
						}
					}
				}
			} else {
				using (var filtered = new FilteredStream (Stream)) {
					filtered.Add (new SmtpDataFilter ());

					if (doAsync) {
						await message.WriteToAsync (options, filtered, cancellationToken).ConfigureAwait (false);
						await filtered.FlushAsync (cancellationToken).ConfigureAwait (false);
					} else {
						message.WriteTo (options, filtered, cancellationToken);
						filtered.Flush (cancellationToken);
					}
				}
			}

			if (doAsync) {
				await Stream.WriteAsync (EndData, 0, EndData.Length, cancellationToken).ConfigureAwait (false);
				await Stream.FlushAsync (cancellationToken).ConfigureAwait (false);

				response = await Stream.ReadResponseAsync (cancellationToken).ConfigureAwait (false);
			} else {
				Stream.Write (EndData, 0, EndData.Length, cancellationToken);
				Stream.Flush (cancellationToken);

				response = Stream.ReadResponse (cancellationToken);
			}

			switch (response.StatusCode) {
			default:
				throw new SmtpCommandException (SmtpErrorCode.MessageNotAccepted, response.StatusCode, response.Response);
			case SmtpStatusCode.AuthenticationRequired:
				throw new ServiceNotAuthenticatedException (response.Response);
			case SmtpStatusCode.Ok:
				OnMessageSent (new MessageSentEventArgs (message, response.Response));
				break;
			}
		}

		async Task ResetAsync (bool doAsync, CancellationToken cancellationToken)
		{
			try {
				var response = await SendCommandAsync ("RSET", doAsync, cancellationToken).ConfigureAwait (false);
				if (response.StatusCode != SmtpStatusCode.Ok)
					Disconnect (uri.Host, uri.Port, GetSecureSocketOptions (uri), false);
			} catch (SmtpCommandException) {
				// do not disconnect
			} catch {
				Disconnect (uri.Host, uri.Port, GetSecureSocketOptions (uri), false);
			}
		}

		/// <summary>
		/// Prepare the message for transport with the specified constraints.
		/// </summary>
		/// <remarks>
		/// <para>Prepares the message for transport with the specified constraints.</para>
		/// <para>Typically, this involves calling <see cref="MimeMessage.Prepare(EncodingConstraint, int)"/> on
		/// the message with the provided constraints.</para>
		/// </remarks>
		/// <param name="options">The format options.</param>
		/// <param name="message">The message.</param>
		/// <param name="constraint">The encoding constraint.</param>
		/// <param name="maxLineLength">The max line length supported by the server.</param>
		protected virtual void Prepare (FormatOptions options, MimeMessage message, EncodingConstraint constraint, int maxLineLength)
		{
			if (!message.Headers.Contains (HeaderId.DomainKeySignature) &&
				!message.Headers.Contains (HeaderId.DkimSignature) &&
				!message.Headers.Contains (HeaderId.ArcSeal)) {
				// prepare the message
				message.Prepare (constraint, maxLineLength);
			} else {
				// Note: we do not want to risk reformatting of headers to the international
				// UTF-8 encoding, so disable it.
				options.International = false;
			}
		}

		static async Task<long> GetSizeAsync (FormatOptions options, MimeMessage message, bool doAsync, CancellationToken cancellationToken)
		{
			using (var measure = new MeasuringStream ()) {
				if (doAsync)
					await message.WriteToAsync (options, measure, cancellationToken).ConfigureAwait (false);
				else
					message.WriteTo (options, measure, cancellationToken);

				return measure.Length;
			}
		}

		/// <summary>
		/// Get the size of the message.
		/// </summary>
		/// <remarks>
		/// <para>Calculates the size of the message in bytes.</para>
		/// <para>This method is called by <a href="Overload_MailKit_MailTransport_Send.htm">Send</a>
		/// methods in the following conditions:</para>
		/// <list type="bullet">
		/// <item>The SMTP server supports the <c>SIZE=</c> parameter in the <c>MAIL FROM</c> command.</item>
		/// <item>The <see cref="ITransferProgress"/> parameter is non-null.</item>
		/// <item>The SMTP server supports the <c>CHUNKING</c> extension.</item>
		/// </list>
		/// </remarks>
		/// <returns>The size of the message, in bytes.</returns>
		/// <param name="options">The formatting options.</param>
		/// <param name="message">The message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		protected virtual long GetSize (FormatOptions options, MimeMessage message, CancellationToken cancellationToken)
		{
			return GetSizeAsync (options, message, false, cancellationToken).GetAwaiter ().GetResult ();
		}

		/// <summary>
		/// Asynchronously get the size of the message.
		/// </summary>
		/// <remarks>
		/// <para>Asynchronously calculates the size of the message in bytes.</para>
		/// <para>This method is called by <a href="Overload_MailKit_MailTransport_SendAsync.htm">SendAsync</a>
		/// methods in the following conditions:</para>
		/// <list type="bullet">
		/// <item>The SMTP server supports the <c>SIZE=</c> parameter in the <c>MAIL FROM</c> command.</item>
		/// <item>The <see cref="ITransferProgress"/> parameter is non-null.</item>
		/// <item>The SMTP server supports the <c>CHUNKING</c> extension.</item>
		/// </list>
		/// </remarks>
		/// <returns>The size of the message, in bytes.</returns>
		/// <param name="options">The formatting options.</param>
		/// <param name="message">The message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		protected virtual Task<long> GetSizeAsync (FormatOptions options, MimeMessage message, CancellationToken cancellationToken)
		{
			return GetSizeAsync (options, message, true, cancellationToken);
		}

		async Task SendAsync (FormatOptions options, MimeMessage message, MailboxAddress sender, IList<MailboxAddress> recipients, bool doAsync, CancellationToken cancellationToken, ITransferProgress progress)
		{
			CheckDisposed ();

			if (!IsConnected)
				throw new ServiceNotConnectedException ("The SmtpClient is not connected.");

			var format = options.Clone ();
			format.NewLineFormat = NewLineFormat.Dos;
			format.EnsureNewLine = true;

			if (format.International && (Capabilities & SmtpCapabilities.UTF8) == 0)
				format.International = false;

			if (format.International && (Capabilities & SmtpCapabilities.EightBitMime) == 0)
				throw new NotSupportedException ("The SMTP server does not support the 8BITMIME extension.");

			EncodingConstraint constraint;
			long size;

			if ((Capabilities & SmtpCapabilities.BinaryMime) != 0)
				constraint = EncodingConstraint.None;
			else if ((Capabilities & SmtpCapabilities.EightBitMime) != 0)
				constraint = EncodingConstraint.EightBit;
			else
				constraint = EncodingConstraint.SevenBit;

			Prepare (format, message, constraint, MaxLineLength);

			// figure out which SMTP extensions we need to use
			var visitor = new ContentTransferEncodingVisitor (capabilities);
			visitor.Visit (message);

			var extensions = visitor.SmtpExtensions;

			if (format.International)
				extensions |= SmtpExtension.UTF8;

			if ((Capabilities & (SmtpCapabilities.Chunking | SmtpCapabilities.Size)) != 0 || progress != null) {
				if (doAsync)
					size = await GetSizeAsync (format, message, cancellationToken);
				else
					size = GetSize (format, message, cancellationToken);
			} else {
				size = -1;
			}

			try {
				// Note: if PIPELINING is supported, MailFrom() and RcptTo() will
				// queue their commands instead of sending them immediately.
				await MailFromAsync (format, message, sender, extensions, size, doAsync, cancellationToken).ConfigureAwait (false);

				int accepted = 0;
				for (int i = 0; i < recipients.Count; i++) {
					if (await RcptToAsync (format, message, recipients[i], doAsync, cancellationToken).ConfigureAwait (false))
						accepted++;
				}

				if (queued.Count > 0) {
					// Note: if PIPELINING is supported, this will flush all outstanding
					// MAIL FROM and RCPT TO commands to the server and then process all
					// of their responses.
					await FlushCommandQueueAsync (message, sender, recipients, doAsync, cancellationToken).ConfigureAwait (false);
				} else if (accepted == 0) {
					OnNoRecipientsAccepted (message);
				}

				if ((extensions & SmtpExtension.BinaryMime) != 0 || (PreferSendAsBinaryData && (Capabilities & SmtpCapabilities.BinaryMime) != 0))
					await BdatAsync (format, message, size, doAsync, cancellationToken, progress).ConfigureAwait (false);
				else
					await DataAsync (format, message, size, doAsync, cancellationToken, progress).ConfigureAwait (false);
			} catch (ServiceNotAuthenticatedException) {
				// do not disconnect
				throw;
			} catch (SmtpCommandException) {
				await ResetAsync (doAsync, cancellationToken).ConfigureAwait (false);
				throw;
			} catch {
				Disconnect (uri.Host, uri.Port, GetSecureSocketOptions (uri) , false);
				throw;
			}
		}

		/// <summary>
		/// Send the specified message.
		/// </summary>
		/// <remarks>
		/// <para>Sends the specified message.</para>
		/// <para>The sender address is determined by checking the following
		/// message headers (in order of precedence): Resent-Sender,
		/// Resent-From, Sender, and From.</para>
		/// <para>If either the Resent-Sender or Resent-From addresses are present,
		/// the recipients are collected from the Resent-To, Resent-Cc, and
		/// Resent-Bcc headers, otherwise the To, Cc, and Bcc headers are used.</para>
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\SmtpExamples.cs" region="SendMessageWithOptions"/>
		/// </example>
		/// <param name="options">The formatting options.</param>
		/// <param name="message">The message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="options"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="message"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="SmtpClient"/> has been disposed.
		/// </exception>
		/// <exception cref="ServiceNotConnectedException">
		/// The <see cref="SmtpClient"/> is not connected.
		/// </exception>
		/// <exception cref="ServiceNotAuthenticatedException">
		/// Authentication is required before sending a message.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>A sender has not been specified.</para>
		/// <para>-or-</para>
		/// <para>No recipients have been specified.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// Internationalized formatting was requested but is not supported by the server.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation has been canceled.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="SmtpCommandException">
		/// The SMTP command failed.
		/// </exception>
		/// <exception cref="SmtpProtocolException">
		/// An SMTP protocol exception occurred.
		/// </exception>
		public override void Send (FormatOptions options, MimeMessage message, CancellationToken cancellationToken = default (CancellationToken), ITransferProgress progress = null)
		{
			if (options == null)
				throw new ArgumentNullException (nameof (options));

			if (message == null)
				throw new ArgumentNullException (nameof (message));

			var recipients = GetMessageRecipients (message);
			var sender = GetMessageSender (message);

			if (sender == null)
				throw new InvalidOperationException ("No sender has been specified.");

			if (recipients.Count == 0)
				throw new InvalidOperationException ("No recipients have been specified.");

			SendAsync (options, message, sender, recipients, false, cancellationToken, progress).GetAwaiter ().GetResult ();
		}

		/// <summary>
		/// Send the specified message using the supplied sender and recipients.
		/// </summary>
		/// <remarks>
		/// Sends the message by uploading it to an SMTP server using the supplied sender and recipients.
		/// </remarks>
		/// <param name="options">The formatting options.</param>
		/// <param name="message">The message.</param>
		/// <param name="sender">The mailbox address to use for sending the message.</param>
		/// <param name="recipients">The mailbox addresses that should receive the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="options"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="message"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="sender"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="recipients"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="SmtpClient"/> has been disposed.
		/// </exception>
		/// <exception cref="ServiceNotConnectedException">
		/// The <see cref="SmtpClient"/> is not connected.
		/// </exception>
		/// <exception cref="ServiceNotAuthenticatedException">
		/// Authentication is required before sending a message.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>A sender has not been specified.</para>
		/// <para>-or-</para>
		/// <para>No recipients have been specified.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// Internationalized formatting was requested but is not supported by the server.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation has been canceled.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="SmtpCommandException">
		/// The SMTP command failed.
		/// </exception>
		/// <exception cref="SmtpProtocolException">
		/// An SMTP protocol exception occurred.
		/// </exception>
		public override void Send (FormatOptions options, MimeMessage message, MailboxAddress sender, IEnumerable<MailboxAddress> recipients, CancellationToken cancellationToken = default (CancellationToken), ITransferProgress progress = null)
		{
			if (options == null)
				throw new ArgumentNullException (nameof (options));

			if (message == null)
				throw new ArgumentNullException (nameof (message));

			if (sender == null)
				throw new ArgumentNullException (nameof (sender));

			if (recipients == null)
				throw new ArgumentNullException (nameof (recipients));

			var unique = new HashSet<string> (StringComparer.OrdinalIgnoreCase);
			var rcpts = new List<MailboxAddress> ();

			AddUnique (rcpts, unique, recipients);

			if (rcpts.Count == 0)
				throw new InvalidOperationException ("No recipients have been specified.");

			SendAsync (options, message, sender, rcpts, false, cancellationToken, progress).GetAwaiter ().GetResult ();
		}

		#endregion

		async Task<InternetAddressList> ExpandAsync (string alias, bool doAsync, CancellationToken cancellationToken)
		{
			if (alias == null)
				throw new ArgumentNullException (nameof (alias));

			if (alias.Length == 0)
				throw new ArgumentException ("The alias cannot be empty.", nameof (alias));

			if (alias.IndexOfAny (new [] { '\r', '\n' }) != -1)
				throw new ArgumentException ("The alias cannot contain newline characters.", nameof (alias));

			CheckDisposed ();

			if (!IsConnected)
				throw new ServiceNotConnectedException ("The SmtpClient is not connected.");

			var response = await SendCommandAsync (string.Format ("EXPN {0}", alias), doAsync, cancellationToken).ConfigureAwait (false);

			if (response.StatusCode != SmtpStatusCode.Ok)
				throw new SmtpCommandException (SmtpErrorCode.UnexpectedStatusCode, response.StatusCode, response.Response);

			var lines = response.Response.Split ('\n');
			var list = new InternetAddressList ();

			for (int i = 0; i < lines.Length; i++) {
				InternetAddress address;

				if (InternetAddress.TryParse (lines[i], out address))
					list.Add (address);
			}

			return list;
		}

		/// <summary>
		/// Expand a mailing address alias.
		/// </summary>
		/// <remarks>
		/// Expands a mailing address alias.
		/// </remarks>
		/// <returns>The expanded list of mailbox addresses.</returns>
		/// <param name="alias">The mailing address alias.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="alias"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="alias"/> is an empty string.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="SmtpClient"/> has been disposed.
		/// </exception>
		/// <exception cref="ServiceNotConnectedException">
		/// The <see cref="SmtpClient"/> is not connected.
		/// </exception>
		/// <exception cref="ServiceNotAuthenticatedException">
		/// Authentication is required before verifying the existence of an address.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation has been canceled.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="SmtpCommandException">
		/// The SMTP command failed.
		/// </exception>
		/// <exception cref="SmtpProtocolException">
		/// An SMTP protocol exception occurred.
		/// </exception>
		public InternetAddressList Expand (string alias, CancellationToken cancellationToken = default (CancellationToken))
		{
			return ExpandAsync (alias, false, cancellationToken).GetAwaiter ().GetResult ();
		}

		async Task<MailboxAddress> VerifyAsync (string address, bool doAsync, CancellationToken cancellationToken)
		{
			if (address == null)
				throw new ArgumentNullException (nameof (address));

			if (address.Length == 0)
				throw new ArgumentException ("The address cannot be empty.", nameof (address));

			if (address.IndexOfAny (new [] { '\r', '\n' }) != -1)
				throw new ArgumentException ("The address cannot contain newline characters.", nameof (address));

			CheckDisposed ();

			if (!IsConnected)
				throw new ServiceNotConnectedException ("The SmtpClient is not connected.");

			var response = await SendCommandAsync (string.Format ("VRFY {0}", address), doAsync, cancellationToken).ConfigureAwait (false);

			if (response.StatusCode == SmtpStatusCode.Ok)
				return MailboxAddress.Parse (response.Response);

			throw new SmtpCommandException (SmtpErrorCode.UnexpectedStatusCode, response.StatusCode, response.Response);
		}

		/// <summary>
		/// Verify the existence of a mailbox address.
		/// </summary>
		/// <remarks>
		/// Verifies the existence a mailbox address with the SMTP server, returning the expanded
		/// mailbox address if it exists.
		/// </remarks>
		/// <returns>The expanded mailbox address.</returns>
		/// <param name="address">The mailbox address.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="address"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="address"/> is an empty string.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="SmtpClient"/> has been disposed.
		/// </exception>
		/// <exception cref="ServiceNotConnectedException">
		/// The <see cref="SmtpClient"/> is not connected.
		/// </exception>
		/// <exception cref="ServiceNotAuthenticatedException">
		/// Authentication is required before verifying the existence of an address.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation has been canceled.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="SmtpCommandException">
		/// The SMTP command failed.
		/// </exception>
		/// <exception cref="SmtpProtocolException">
		/// An SMTP protocol exception occurred.
		/// </exception>
		public MailboxAddress Verify (string address, CancellationToken cancellationToken = default (CancellationToken))
		{
			return VerifyAsync (address, false, cancellationToken).GetAwaiter ().GetResult ();
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="SmtpClient"/> and
		/// optionally releases the managed resources.
		/// </summary>
		/// <remarks>
		/// Releases the unmanaged resources used by the <see cref="SmtpClient"/> and
		/// optionally releases the managed resources.
		/// </remarks>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources;
		/// <c>false</c> to release only the unmanaged resources.</param>
		protected override void Dispose (bool disposing)
		{
			if (disposing && !disposed) {
				disposed = true;
				Disconnect (null, 0, SecureSocketOptions.None, false);
			}

			base.Dispose (disposed);
		}
	}
}
