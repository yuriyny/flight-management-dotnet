using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Dtos
{
    public class FlightDto
    {
        public string AirlineName { get; set; }
        public string FlightCode { get; set; }
        public string FromCity { get; set; }
        public string FromAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string ToCity { get; set; }
        public string ToAirport { get; set; }
        public int Tickets { get; set; }
    }
}
