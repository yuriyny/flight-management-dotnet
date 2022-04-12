using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Models
{
    public class Address
    {
        //[ForeignKey("User")]
        public int AddressId { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string  Country { get; set; }
        public int ZipCode { get; set; }
        public string Street { get; set; }
        public User User { get; set; }
    }
}
