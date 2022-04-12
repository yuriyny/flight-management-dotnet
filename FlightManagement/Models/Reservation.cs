using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime ReservationTime { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
        public int ReturnTicketId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
