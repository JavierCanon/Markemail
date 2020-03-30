using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit;
using MimeKit;
using MailKit.Net.Imap;
using MailKit.Search;

namespace MyMailBox.Models
{
    public class Account
    {
        private int ID = -1;
        public String fullName = String.Empty;
        private String emailAddress = String.Empty;
        private String password = String.Empty;
        public Boolean rememberPassword = false;
        public String signature = String.Empty;
        private String typeMail = "imap";

        private String mailService = String.Empty;
        private String identity = String.Empty;
        private int port = 993;
        private Boolean useSSL = true;

        private ImapClient client;
        private Boolean isConnected = false;
        private Boolean testConnectionSucceed = false;

        public Account(String fullName, String emailAddress, String password, Boolean rememberPassword, Boolean useSSL = true, int port = 993)
        {
            this.fullName = fullName;
            this.emailAddress = emailAddress;
            this.password = password;
            this.rememberPassword = rememberPassword;
            this.port = port;
            this.useSSL = useSSL;
        }

        public Account(Account oldAccount, int id)
            : this(oldAccount.fullName, oldAccount.emailAddress, oldAccount.password, oldAccount.rememberPassword, oldAccount.useSSL, oldAccount.port)
        {
            this.ID = id;
            this.testConnectionSucceed = oldAccount.testConnectionSucceed;
        }

        public Account(String[] accountParameters)
        {
            //System.Diagnostics.Debug.WriteLine(accountParameters[0] + " + " + accountParameters[1] + " + " + accountParameters[2] + " + " + accountParameters[3] + " + " + accountParameters[4] + " + " + accountParameters[5] + " + ");
            this.ID = Int32.Parse(accountParameters[0]);
            fullName = accountParameters[1];
            emailAddress = accountParameters[2];
            if (accountParameters[3] == "true" || accountParameters[3] == "TRUE" || accountParameters[3] == "True")
            {
                useSSL = true;
            }
            else
            {
                useSSL = false;
            }
            port = Int32.Parse(accountParameters[4]);
            identity = accountParameters[5];
            mailService = accountParameters[6];
            typeMail = accountParameters[7].Substring(0, accountParameters[7].IndexOf('.'));
            signature = accountParameters[8];
            if (accountParameters[9] == "true" || accountParameters[9] == "TRUE" || accountParameters[9] == "True")
            {
                rememberPassword = true;
                password = accountParameters[10];

            }
            else
            {
                rememberPassword = false;
            }
            this.testConnection();
        }

        public Account() { }

        public int getID()
        {
            return ID;
        }

        public void initPassword()
        {
            password = String.Empty;
        }
        public String getPassword()
        {
            return this.password;
        }

        public int getPort()
        {
            return (this.port);
        }

        public Boolean getUseSSL()
        {
            return useSSL;
        }

        public String getMailService()
        {
            if (this.mailService == String.Empty)
            {
                this.mailService = emailAddress.Substring(emailAddress.IndexOf("@") + 1);
            }
            return this.mailService;
        }

        public String getServer()
        {
            return this.typeMail + "." + getMailService();
        }

        public String getEmail()
        {
            return emailAddress;
        }

        public void setEmail(String newEmail)
        {
            mailService = String.Empty;
            identity = String.Empty;
            emailAddress = newEmail;
        }

        public String getIdentity()
        {
            if (this.identity == String.Empty)
            {
                this.identity = emailAddress.Substring(0, emailAddress.IndexOf("@"));
            }
            return this.identity;
        }

        public Boolean connection()
        {
            if (testConnectionSucceed == false)
            {
                System.Diagnostics.Debug.WriteLine("Warning : Try to connect to mail server with [testConnection] first");
                return false;
            }
            if (isConnected)
            {
                return true;
            }
            var newClient = new ImapClient();
            if (doConnection(newClient))
            {
                this.client = newClient;
                this.isConnected = true;
                return true;
            } else
            {
                return false;
            }
        }

        public Boolean testConnection()
        {
            System.Diagnostics.Debug.WriteLine("Trying to connect to " + getServer());
            ImapClient newClient = new ImapClient();
            if (this.doConnection(newClient))
            {
                System.Diagnostics.Debug.WriteLine("Connection succeded to " + getServer());
                testConnectionSucceed = true;
                disconnection(newClient);
                return true;
            }
            System.Diagnostics.Debug.WriteLine("Fail to connect to " + getServer());
            testConnectionSucceed = false;
            return false;
        }

        private Boolean doConnection(ImapClient newClient)
        {
            try
            {
                newClient.Connect(this.getServer(), this.port, this.useSSL);
            }
            catch (MailKit.ServiceNotConnectedException)
            {
                System.Diagnostics.Debug.WriteLine("Error: Connection error");
                return false;
            }
            catch (System.Exception)
            {
                System.Diagnostics.Debug.WriteLine("Error: Connection error");
                return false;
            }

            var identity = this.getIdentity();

            try
            {
                newClient.AuthenticationMechanisms.Remove("XOAUTH2");
                newClient.Authenticate(identity, this.password);
            }
            catch (MailKit.Net.Imap.ImapProtocolException)
            {
                System.Diagnostics.Debug.WriteLine("Error: Authentificate error");
                return false;
            }
            return true;
        }

        public void disconnection(ImapClient newClient = null)
        {
            if (newClient != null)
            {
                newClient.Disconnect(true);
            }
            else if (client != null)
            {
                client.Disconnect(true);
            }
            isConnected = false;
        }

        public List<MailPreview> getAllMailPreview()
        {
            if (isConnected == false)
            {
                return null;
            }
            var inbox = client.Inbox;
            inbox.Open(FolderAccess.ReadOnly);

            List<MailPreview> listMails = new List<MailPreview>();
            
            foreach (var summary in inbox.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId))
            {
                List<Address> from = getNameFromInternetAddressList(summary.Envelope.From);
                String date = getDateToString(summary.Envelope.Date);
                listMails.Add(new MailPreview(summary.Envelope.Subject, from, date, summary.UniqueId));
            }
            System.Diagnostics.Debug.WriteLine("INFO : number of email : " + listMails.Count);
            return listMails;
        }

        public Mail getEmailById(UniqueId id)
        {
            var message = this.client.Inbox.GetMessage(id);
            String body = String.Empty;
            if (message.HtmlBody != null)
            {
                body = " <meta http-equiv='Content-Type' content='text/html;charset=UTF-8'>" + message.HtmlBody;
            }
            else if (message.TextBody != null)
            {
                body = message.TextBody;
            }
            Mail newMail = new Mail(
                new MailSettings(getNameFromInternetAddressList(message.ResentTo), getDateToString(message.Date)),
                new MailContent(body, message.Subject, getNameFromInternetAddressList(message.From), getNameFromInternetAddressList(message.To)));
            return newMail;
        }

        private List<Address> getNameFromInternetAddressList(InternetAddressList internetAddressList)
        {
            List<Address> list = new List<Address>();

            List<MailboxAddress> listAddress = internetAddressList.Mailboxes.ToList();
            foreach(MailboxAddress address in listAddress)
            {
                list.Add(new Address(address.Name, address.Address));
            }
            return list;
        }

        private String getDateToString(DateTimeOffset? dateTime)
        {
            if (dateTime == null)
            {
                return "";
            }
            return dateTime?.Day + "/" + dateTime?.Month + "/" + dateTime?.Year;
        }
    }
}
