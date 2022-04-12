using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Dtos
{
    public class TwoWayReservationDto
    {
        public ReservationDto DepartureReservation { get; set; }
        public ReservationDto ReturnReservation { get; set; }
    }
}
