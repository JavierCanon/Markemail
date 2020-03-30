using System;

namespace EmailClient.Core.Model
{
    /// <summary>
    /// This class contains data to set up a new Mailbox connection
    /// </summary>
    [Serializable]
    public class MailBoxProperties
    {
        /// <summary>
        /// Gets the user name of the mailbox's owner.
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// Gets the name of the mailbox
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the IMAP server
        /// </summary>
        public string Imap { get; }

        /// <summary>
        /// Gets the port of the IMAP server
        /// </summary>
        public int ImapPort { get; }

        /// <summary>
        /// Gets the SMTP server
        /// </summary>
        public string Smtp { get; }

        /// <summary>
        /// Gets the port of the SMTP server
        /// </summary>
        public int SmtpPort { get; }

        /// <summary>
        /// Gets the own email address of the account
        /// </summary>
        public string Account { get; }

        /// <summary>
        /// Gets the password of the email account
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Gets the frequency of the connection
        /// </summary>
        public int ConnectionFreq { get; }

        /// <summary>
        /// Initializes a new Properties struct with the necessary data to set up a Mailbox connection
        /// </summary>
        /// <param name="name">Name of the connection</param>
        /// <param name="imap">IMAP server, for receiving emails</param>
        /// <param name="imapPort">IMAP port</param>
        /// <param name="smtp">SMTP server, for sending emails</param>
        /// <param name="smtpPort">SMTP port</param>
        /// <param name="account">The email address of the account</param>
        /// <param name="password">Third party application password of the email account</param>
        /// <param name="connectionFreq">Frequency in milliseconds of connecting to the IMAP server</param>
        public MailBoxProperties(string userName, string name, string imap, int imapPort, string smtp, int smtpPort, string account, string password, int connectionFreq)
        {
            UserName = userName;
            Name = name;
            Imap = imap;
            ImapPort = imapPort;
            Smtp = smtp;
            SmtpPort = smtpPort;
            Account = account;
            Password = password;
            ConnectionFreq = connectionFreq;
        }
    }
}
