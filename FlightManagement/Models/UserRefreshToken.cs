using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Models
{
    public class UserRefreshToken
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}
