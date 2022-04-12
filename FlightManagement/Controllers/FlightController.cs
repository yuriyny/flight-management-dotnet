using FlightManagement.Dtos;
using FlightManagement.Models;
using FlightManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FlightController : Controller
    {
        private readonly FlightService _flightService;
        public FlightController(FlightService flightService)
        {
            _flightService = flightService;
        }

        [HttpPost("add-flight")]
        public async Task<IActionResult> AddFlight([FromBody] FlightDto flightDto)
        {
            await _flightService.AddFlight(flightDto);
            return Ok();
        }

        [HttpPost("add-ticket")]
        public async Task<IActionResult> AddTicket([FromBody] TicketDto ticketDto)
        {
            await _flightService.AddTicket(ticketDto);
            return Ok();
        }

        [HttpGet("get-all-flights")]
        public async Task<ActionResult<IEnumerable<Flight>>> GetAllFlights()
        {
            var fl = await _flightService.GettAllFlights();
            return Ok(fl);
        }


        [HttpPost("get-date-flights")]
        public async Task<ActionResult<IEnumerable<Flight>>> GettDateSpecificFlights(OneWayFlightDto oneWayFlightDto)
        {
            var fl = await _flightService.GettDateSpecificFlights(oneWayFlightDto);
            return Ok(fl);
        }

        [HttpPost("get-flights-info")]
        public async Task<ActionResult<IEnumerable<FlightInfoDto>>> GetFlightInfo(OneWayFlightDto oneWayFlightDto)
        {
            var fl = await _flightService.GetFlightInfo(oneWayFlightDto);
            return Ok(fl);
        }

        [HttpPost("get-two-way-flights-info")]
        public async Task<ActionResult<IEnumerable<TwoWayFlightInfoDto>>> GetTwoWayFlightInfo(TwoWayFlightDto twoWayFlightDto)
        {
            var fl = await _flightService.GetTwoWayFlightInfo(twoWayFlightDto);
            return Ok(fl);
        }
        [HttpPost("add-reservation")]
        public async Task<ActionResult> AddReservation(AddReservationDto addReservationDto)
        {
            Console.WriteLine(addReservationDto.SelectedTicketId);
            await _flightService.AddReservation(addReservationDto);
            return Ok();
        }

        [HttpGet("get-user-reservations")]
        public async Task<ActionResult<IEnumerable<TwoWayReservationDto>>> GetUserReservations()
        {
            var ur = await _flightService.GetUserReservations();
            return Ok(ur);
        }
        [AllowAnonymous]
        [HttpGet("get-flights-details")]
        public async Task<ActionResult<IEnumerable<FlightDetailDto>>> GetFlightsDetails()
        {
            var flightsDetails = await _flightService.GetFlightsDetails();
            return Ok(flightsDetails);
        }

        [AllowAnonymous]
        [HttpGet("get-tickets-details")]
        public async Task<ActionResult<IEnumerable<TicketDetailsDto>>> GetTicketsDetails()
        {
            var ticketsDetails = await _flightService.GetTicketsDetails();
            return Ok(ticketsDetails);
        }
    }


}