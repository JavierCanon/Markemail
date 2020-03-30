using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit;

namespace MyMailBox.Models
{
    public class MailPreview
    {
        public String mailObject { get; set; }
        public String mailFrom { get; set; }
        public String date { get; set; }
        private UniqueId uniqueId;
        private List<Address> listFrom = null;

        public MailPreview(String mailObject, List<Address> listFrom, String date, UniqueId index)
            : this(mailObject, listFrom, date)
        {
            this.uniqueId = index;
        }

        protected MailPreview(String mailObject, List<Address> listFrom, String date)
        {
            this.mailObject = mailObject;
            this.listFrom = listFrom;
            this.mailFrom = getAddressStringFromAddressList(listFrom);
            this.date = date;
        }

        protected String getAddressStringFromAddressList(List<Address> listAddress)
        {
            String allAddress = String.Empty;
            foreach(Address address in listAddress)
            {
                if (allAddress != String.Empty)
                {
                    allAddress += ", ";
                }
                allAddress += address.name + " <" + address.address + ">";
            }
            return allAddress;
        }

        public UniqueId getUniqueID()
        {
            return (this.uniqueId);
        }
    }
}
