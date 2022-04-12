using FlightManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Dtos
{
    public class TicketDto
    {
        public TicketClass TicketClass { get; set; }
        public string FlightCode { get; set; }
        public double Price { get; set; }
    }
}
