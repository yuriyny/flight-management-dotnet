using FlightManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Dtos
{
    public class OneWayFlightDto
    {
        public string FromCity { get; set; }
        public string FromAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime DepartureTimeEnd { get; set; }
        public string ToCity { get; set; }
        public string ToAirport { get; set; }
        public TicketClass TicketClass { get; set; }
    }
}
