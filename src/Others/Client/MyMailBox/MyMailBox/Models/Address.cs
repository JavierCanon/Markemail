using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMailBox.Models
{
    public class Address
    {
        public String name = String.Empty;
        public String address = String.Empty;

        public Address(String name, String address)
        {
            this.name = name;
            this.address = address;
        }

    }
}
