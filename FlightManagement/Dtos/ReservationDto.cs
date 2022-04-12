using FlightManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Dtos
{
    public class ReservationDto
    {
        public DateTime ReservationTime { get; set; }
        public string AirlineName { get; set; }
        public string FlightCode { get; set; }
        public string FromCity { get; set; }
        public string FromAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int FlightDurationHours { get; set; }
        public int FlightDurationMinutes { get; set; }
        public int FlightDurationDays { get; set; }
        public string ToCity { get; set; }
        public string ToAirport { get; set; }
        public int TicketId { get; set; }
        public double TicketPrice { get; set; }
        public TicketClass TicketClass { get; set; }
        public int ReturnTicketId { get; set; }
        public double ReturnTicketPrice { get; set; }
        public int UserId { get; set; }
    }
}
