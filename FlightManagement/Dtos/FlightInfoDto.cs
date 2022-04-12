using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Dtos
{
    public class FlightInfoDto
    {
        public string AirlineName { get; set; }
        public string FlightCode { get; set; }
        public string FromCity { get; set; }
        public string FromAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string ToCity { get; set; }
        public string ToAirport { get; set; }
        public int BusinessTickets { get; set; }
        public int EconomyTickets { get; set; }
        public double EconomyTicketPrice { get; set; }
        public double BusinessTicketPrice { get; set; }
        public int EconomyTicketPriceId { get; set; }
        public int BusinessTicketPriceId { get; set; }
        public int SelectedTicketId { get; set; }
    }
}
