using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Dtos
{
    public class TwoWayFlightInfoDto
    {
        public List<FlightInfoDto> DepartureFlights { get; set; }
        public List<FlightInfoDto> ReturnFlights { get; set; }
    }
}
