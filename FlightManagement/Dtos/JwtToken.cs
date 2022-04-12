using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Dtos
{
    public class JwtToken
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
