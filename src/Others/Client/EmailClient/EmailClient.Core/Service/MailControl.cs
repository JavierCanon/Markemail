using EmailClient.Core.Model;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace EmailClient.Core.Service
{
    /// <summary>
    /// This class contains the business logic of the Mail service
    /// </summary>
    public class MailControl
    {
        private readonly MailBoxProperties _properties;
        private readonly IList<Email> _emails;
        public delegate void ConnectionStatusChanged();
        public delegate void Notification(string message);

        /// <summary>
        /// This event fires if an IMAP connection starts
        /// </summary>
        public event ConnectionStatusChanged connectionStart;

        /// <summary>
        /// This event fires if an IMAP connection successfully finishes
        /// </summary>
        public event ConnectionStatusChanged connectionFinish;

        /// <summary>
        /// General purpose notification event. For example: new emails have been received.
        /// </summary>
        public event Notification notification;

        /// <summary>
        /// Gets, sets the dispatcher timer of the Mailbox
        /// </summary>
        public DispatcherTimer Timer { get; private set; } = new DispatcherTimer();

        /// <summary>
        /// A delegate for handling the the download time status changes on the UI thread
        /// </summary>
        /// <param name="state">The elapased time in milliseconds</param>
        private delegate void handleDwnldTime(object state);

        public MailControl(MailBoxProperties properties, IList<Email> emails)
        {
            _properties = properties;
            _emails = emails;
        }

        /// <summary>
        /// Sets up a connection to the Email server.
        /// </summary>
        /// <param name="client">ImapClient object of the MailKit package</param>
        /// <returns>A collection of email unique IDs.</returns>
        private void SetConnection(ImapClient client)
        {
            client.Connect(_properties.Imap, _properties.ImapPort, SecureSocketOptions.SslOnConnect);
            client.Authenticate(_properties.Account, _properties.Password);
            client.Inbox.Open(FolderAccess.ReadWrite);
        }

        /// <summary>
        /// Downloads email from the server
        /// </summary>
        /// <returns>A collection with the downloaded emails</returns>
        private List<Email> GetEmails()
        {
            var downloaded = new List<Email>();
            using (var client = new ImapClient())
            {
                SetConnection(client);
                foreach (var uid in client.Inbox.Search(SearchQuery.All))
                {
                    try
                    {
                        var email = client.Inbox.GetMessage(uid);
                        downloaded.Add(new Email(uid, email));
                    }
                    catch (MessageNotFoundException e)
                    {
                        Console.WriteLine($"Exception: {e.Message}");
                    }
                }
                client.Disconnect(true);
            }
            return downloaded;
        }

        /// <summary>
        /// Downloads the the emails asynchronously and adds them to the collection observed by the UI thread.
        /// After the download refreshes the download time indicator textbox on the UI.
        /// It fires the right events (connectionStart, connectionFinish) at the beginning and end of the IMAP connection.
        /// </summary>
        private async void GetEmailsAsync()
        {
            connectionStart();
            var downloaded = await Task<List<Email>>.Factory.StartNew(GetEmails);
            connectionFinish();
            ManageEmailChanges(downloaded);
        }

        /// <summary>
        /// Adds the new and removes the deleted emails from the Emails collection
        /// If there are new messages it fires a notification event towards the UI.
        /// </summary>
        /// <param name="downloaded">List of the current emails that are downloaded from the IMAP server.</param>
        private void ManageEmailChanges(List<Email> downloaded)
        {
            var toAdd = downloaded.Where(email => !_emails.Contains(email)).ToList();
            var toRemove = _emails.Where(email => !downloaded.Contains(email)).ToList();
            toAdd.ForEach(email => _emails.Add(email));
            toRemove.ForEach(email => _emails.Remove(email));
            if (toAdd.Count > 0)
            {
                notification($"{toAdd.Count} new message(s) received!");
            }
        }

        /// <summary>
        /// Starts a dispatcher timer, that schedules the the periodical email downloading
        /// </summary>
        public void StartGettingEmails()
        {
            Timer.Interval = TimeSpan.FromMilliseconds(_properties.ConnectionFreq);
            Timer.Tick += (sender, args) => GetEmailsAsync();
            Timer.Start();
        }

        /// <summary>
        /// Stops the dispatcher timer, that schedules the the periodical email downloading
        /// </summary>
        public void StopGettingEmails()
        {
            Timer.Stop();
        }

        /// <summary>
        /// Converter method. Creates a new send ready MimeMessage object
        /// </summary>
        /// <param name="toAddress">Recipients email address</param>
        /// <param name="subject">Subject of the email</param>
        /// <param name="content">Content of the email</param>
        /// <returns>A MimeMessage object, that is ready to send</returns>
        public MimeMessage CreateNewEmailToSend(string toAddress, string subject, string content)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_properties.UserName, _properties.Account));
            message.To.Add(new MailboxAddress("", toAddress));
            message.Subject = subject;
            var builder = new BodyBuilder
            {
                TextBody = content
            };
            message.Body = builder.ToMessageBody();
            return message;
        }

        /// <summary>
        /// Sends an Email message
        /// </summary>
        /// <param name="state">MimeMessage object, it has to be explicitly casted</param>
        private void SendEmail(object state)
        {
            using (var client = new SmtpClient())
            {
                client.Connect(_properties.Smtp, _properties.SmtpPort, true);
                client.Authenticate(_properties.Account, _properties.Password);
                client.Send((MimeMessage)state);
                client.Disconnect(true);
            }
        }

        /// <summary>
        /// Sends email asynchronously
        /// </summary>
        /// <param name="email">Mime Message object, created by the CreateNewEmailToSend() method</param>
        public async void SendEmailAsync(MimeMessage email)
        {
            await Task.Factory.StartNew(SendEmail, email);
        }

        /// <summary>
        /// Delete emails from the IMAP server
        /// This method will be calles asnycrhronously by the DeleteEmailsAsync method
        /// </summary>
        /// <param name="state">collection of the UniquId-s (explicit casting needed)</param>
        private void DeleteEmails(object state)
        {
            var uids = (IList<UniqueId>)state;
            using (var client = new ImapClient())
            {
                SetConnection(client);
                client.Inbox.AddFlags(uids, MessageFlags.Deleted, true);
                client.Inbox.Expunge();
                client.Disconnect(true);
            }
        }

        /// <summary>
        /// Deletes emails asnychronously from the IMAP server
        /// </summary>
        /// <param name="uids">Collection of the UniqueId objects of the emails marks for deletion</param>
        public async void DeleteEmailsAsync(IList<UniqueId> uids)
        {
            await Task.Factory.StartNew(DeleteEmails, uids);
        }
    }
}
