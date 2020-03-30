using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eron.mailClient.Application.Core.Enums;
using Eron.MailClient.Business.Core.Infrastructure;
using Eron.MailClient.Business.Core.Services.ImapService.Dto;
using Eron.MailClient.SharedKernel.Modules.Helpers.AppSettingsHelper;
using Eron.MailClient.SharedKernel.Modules.ValueObjects;
using MailKit;
using MailKit.Net.Imap;

namespace Eron.MailClient.Business.Core.Services.ImapService
{
    public interface IImapAppService: IApplicationService
    {
        IPagedListResult<EmailSummeryDto> GetEmailsByFolder(EmailDefaultFolderType folderType);
    }

    public class ImapAppService : IImapAppService
    {
        private readonly int _imapPort = ApplicationSettingsHelper.AppSetting<int>("MailServer.ImapPort");
        private readonly string _imapServer = ApplicationSettingsHelper.AppSetting("MailServer.ImapServer");
        private readonly int _pop3Port = ApplicationSettingsHelper.AppSetting<int>("MailServer.Pop3Port");
        private readonly string _pop3Server = ApplicationSettingsHelper.AppSetting("MailServer.Pop3Server");

        public IPagedListResult<EmailSummeryDto> GetEmailsByFolder(EmailDefaultFolderType folderType)
        {
            using (var client = new ImapClient())
            {
                // For demo-purposes, accept all SSL certificates
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(_imapServer, _imapPort, true);

                client.Authenticate("joey", "password");

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                Console.WriteLine("Total messages: {0}", inbox.Count);
                Console.WriteLine("Recent messages: {0}", inbox.Recent);

                for (int i = 0; i < inbox.Count; i++)
                {
                    var message = inbox.GetMessage(i);
                    Console.WriteLine("Subject: {0}", message.Subject);
                }

                client.Disconnect(true);
            }

            throw new NotImplementedException();
        }
    }
}
