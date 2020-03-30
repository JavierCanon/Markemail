using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMailBox.Models
{
    public class MailContent
    {
        public String body = String.Empty;
        public String subject = String.Empty;
        public List<Address> listFrom = null;
        public List<Address> listTo = null;

        public MailContent(String body, String subject, List<Address> listFrom, List<Address> listTo)
        {
            this.body = body;
            this.subject = subject;
            this.listFrom = listFrom;
            this.listTo = listTo;
        }
    }
}
