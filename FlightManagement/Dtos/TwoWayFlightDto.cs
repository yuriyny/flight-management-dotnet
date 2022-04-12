using FlightManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Dtos
{
    public class TwoWayFlightDto
    {
        public string FromCity { get; set; }
        public string FromAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ReturnTime { get; set; }
        public DateTime DepartureTimeEnd { get; set; }
        public DateTime ReturnTimeEnd { get; set; }
        public string ToCity { get; set; }
        public string ToAirport { get; set; }
        //public bool IsReturning { get; set; }
        public TicketClass TicketClass { get; set; }
    }
}
