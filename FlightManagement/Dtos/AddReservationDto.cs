using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Dtos
{
    public class AddReservationDto
    {
        public int SelectedTicketId { get; set; }
        public int ReturnTicketId { get; set; }
    }
}
