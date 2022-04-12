using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public TicketClass TicketClass { get; set; }
        public int FlightId { get; set; }
        public Flight Flight { get; set; }
        public double Price { get; set; }
        public bool IsBooked { get; set; }
        //public int UserId { get; set; }
        //public User User { get; set; }
    }
}
