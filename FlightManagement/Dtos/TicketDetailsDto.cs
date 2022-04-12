using FlightManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Dtos
{
    public class TicketDetailsDto
    {
        public int TicketId { get; set; }
        public TicketClass TicketClass { get; set; }
        public string FlightCode { get; set; }
        public double Price { get; set; }
        public string Username { get; set; }
        public int UserId { get; set; }
    }
}
