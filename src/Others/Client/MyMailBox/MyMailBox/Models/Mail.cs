using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMailBox.Models
{
    public class Mail : MailPreview
    {
        public String body = String.Empty;
        public List<Address> listTo = null;

        public Mail(MailSettings mailSettings, MailContent mailContent)
            : base(mailContent.subject, mailContent.listFrom, mailSettings.date)
        {
            this.body = mailContent.body;
            this.listTo = mailContent.listTo;
        }

        public String getStringListTo()
        {
            return getAddressStringFromAddressList(listTo);
        }
    }
}
