using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMailBox.Models
{
    public class MailSettings
    {
        public List<Address> resendTo = null;
        public String date = String.Empty;

        public MailSettings(List<Address> resendTo, String date)
        {
            this.resendTo = resendTo;
            this.date = date;
        }
    }
}
