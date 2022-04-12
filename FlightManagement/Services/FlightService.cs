using FlightManagement.Data;
using FlightManagement.Dtos;
using FlightManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Services
{
    public class FlightService
    {
        private readonly IAuthRepository _authRepository;
        private DataContext _context;
        private UserService _userService;

        public FlightService(IAuthRepository authRepository, DataContext context, UserService userService)
        {
            _authRepository = authRepository;
            _context = context;
            _userService = userService;
        }

        public async Task AddFlight(FlightDto flightDto)
        {
            var _flight = new Flight()
            {
                AirlineName = flightDto.AirlineName.ToUpper(),
                FlightCode = flightDto.FlightCode.ToUpper(),
                FromAirport = flightDto.FromAirport.ToUpper(),
                FromCity = flightDto.FromCity.ToUpper(),
                DepartureTime = flightDto.DepartureTime,
                ToAirport = flightDto.ToAirport.ToUpper(),
                ToCity = flightDto.ToCity.ToUpper(),
                ArrivalTime = flightDto.ArrivalTime

            };

            await _context.Flights.AddAsync(_flight);
            await _context.SaveChangesAsync();

        }

        public async Task AddTicket(TicketDto ticketDto)
        {
            Flight f = await _context.Flights.Where(x => x.FlightCode == ticketDto.FlightCode).SingleAsync();
            Console.WriteLine(f.FlightCode);
            //Flight f = _context.Flights.Find(ticketDto.FlightId);

            var _ticket = new Ticket()
            {
                Flight = f,
                TicketClass = ticketDto.TicketClass,
                Price = ticketDto.Price
            };

            await _context.Tickets.AddAsync(_ticket);
            await _context.SaveChangesAsync();

        }

        public async Task<List<Flight>> GettAllFlights()
        {
            return await _context.Flights.ToListAsync();
        }

        public async Task<List<Flight>> GettDateSpecificFlights(OneWayFlightDto oneWayFlightDto)
        {
            return await _context.Flights.Where(x => x.DepartureTime.Date >= oneWayFlightDto.DepartureTime && x.DepartureTime.Date <= oneWayFlightDto.DepartureTimeEnd && x.FromAirport.ToUpper() == oneWayFlightDto.FromAirport.ToUpper() && x.ToAirport.ToUpper() == oneWayFlightDto.ToAirport.ToUpper()).ToListAsync();
        }

        public async Task<List<FlightInfoDto>> GetFlightInfo(OneWayFlightDto oneWayFlightDto)
        {
            List<FlightInfoDto> flightInfoList = new List<FlightInfoDto>();
            List<Flight> flights = await _context.Flights.Where(x => x.DepartureTime.Date >= oneWayFlightDto.DepartureTime && x.DepartureTime.Date <= oneWayFlightDto.DepartureTimeEnd && x.FromCity.ToUpper() == oneWayFlightDto.FromCity.ToUpper() && x.ToCity.ToUpper() == oneWayFlightDto.ToCity.ToUpper()).ToListAsync();
            foreach (Flight f in flights)
            {
                //get all tickets for the flight
                List<Ticket> bust = await _context.Tickets.Where(x => x.FlightId == f.Id && x.IsBooked == false && x.TicketClass == TicketClass.Business).ToListAsync();
                List<Ticket> ecot = await _context.Tickets.Where(x => x.FlightId == f.Id && x.IsBooked == false && x.TicketClass == TicketClass.Economy).ToListAsync();
                //Console.WriteLine(bust.Price);
                Ticket lowestBusiness = bust.OrderBy(x => x.Price).FirstOrDefault();
                Ticket lowestEconomy = ecot.OrderBy(x => x.Price).FirstOrDefault();
                int businessNum = bust.Count;
                int ecoNum = ecot.Count;
                FlightInfoDto fInfo = new FlightInfoDto
                {
                    EconomyTickets = ecoNum,
                    BusinessTickets = businessNum,
                    AirlineName = f.AirlineName,
                    FlightCode = f.FlightCode,
                    FromAirport = f.FromAirport,
                    FromCity = f.FromCity,
                    DepartureTime = f.DepartureTime,
                    ArrivalTime = f.ArrivalTime,
                    ToAirport = f.ToAirport,
                    ToCity = f.ToCity,
                    BusinessTicketPrice = -1,
                    BusinessTicketPriceId = -1,
                    EconomyTicketPrice = -1,
                    EconomyTicketPriceId = -1

                };

                if (lowestBusiness != null)
                {
                    fInfo.BusinessTicketPrice = lowestBusiness.Price;
                    fInfo.BusinessTicketPriceId = lowestBusiness.Id;
                }
                if (lowestEconomy != null)
                {
                    fInfo.EconomyTicketPrice = lowestEconomy.Price;
                    fInfo.EconomyTicketPriceId = lowestEconomy.Id;
                }
                if (lowestBusiness != null || lowestEconomy != null)
                {
                    flightInfoList.Add(fInfo);
                }
                
            }
            return flightInfoList;
        }


        public async Task<TwoWayFlightInfoDto> GetTwoWayFlightInfo(TwoWayFlightDto twoWayFlightDto)
        {
            List<FlightInfoDto> flightInfoList = new List<FlightInfoDto>();
            List<FlightInfoDto> returnFlightInfoList = new List<FlightInfoDto>();
            List<Flight> flights = await _context.Flights.Where(x => x.DepartureTime.Date >= twoWayFlightDto.DepartureTime && x.DepartureTime.Date <= twoWayFlightDto.DepartureTimeEnd && x.FromCity == twoWayFlightDto.FromCity && x.ToCity == twoWayFlightDto.ToCity).ToListAsync();
            List<Flight> returningFlights = await _context.Flights.Where(x => x.DepartureTime.Date >= twoWayFlightDto.ReturnTime && x.DepartureTime.Date <= twoWayFlightDto.ReturnTimeEnd && x.FromCity == twoWayFlightDto.ToCity && x.ToCity == twoWayFlightDto.FromCity).ToListAsync();

            TwoWayFlightInfoDto result = new TwoWayFlightInfoDto();
            foreach (Flight f in flights) 
            {
                //get all tickets for the flight
                List<Ticket> bust = await _context.Tickets.Where(x => x.FlightId == f.Id && x.IsBooked == false && x.TicketClass == TicketClass.Business).ToListAsync();
                List<Ticket> ecot = await _context.Tickets.Where(x => x.FlightId == f.Id && x.IsBooked == false && x.TicketClass == TicketClass.Economy).ToListAsync();
                //Console.WriteLine(bust.Price);
                Ticket lowestBusiness = bust.OrderBy(x => x.Price).FirstOrDefault();
                Ticket lowestEconomy = ecot.OrderBy(x => x.Price).FirstOrDefault();
                //var min = yourList.OrderBy(p => p.Col1).First();
                //Console.WriteLine(lowestBusiness.Price);
                //var oldest = People.OrderBy(p => p.DateOfBirth ?? DateTime.MaxValue).First();
                int businessNum = bust.Count;
                int ecoNum = ecot.Count;
                FlightInfoDto fInfo = new FlightInfoDto
                {
                    EconomyTickets = ecoNum,
                    BusinessTickets = businessNum,
                    AirlineName = f.AirlineName,
                    FlightCode = f.FlightCode,
                    FromAirport = f.FromAirport,
                    FromCity = f.ToCity,
                    DepartureTime = f.DepartureTime,
                    ArrivalTime = f.ArrivalTime,
                    ToAirport = f.ToAirport,
                    ToCity = f.ToCity,
                    BusinessTicketPrice = -1,
                    BusinessTicketPriceId = -1,
                    EconomyTicketPrice = -1,
                    EconomyTicketPriceId = -1

                };

                if (lowestBusiness != null)
                {
                    fInfo.BusinessTicketPrice = lowestBusiness.Price;
                    fInfo.BusinessTicketPriceId = lowestBusiness.Id;
                }
                if (lowestEconomy != null)
                {
                    fInfo.EconomyTicketPrice = lowestEconomy.Price;
                    fInfo.EconomyTicketPriceId = lowestEconomy.Id;
                }
                if (lowestBusiness != null || lowestEconomy != null)
                {
                    flightInfoList.Add(fInfo);
                }

            }
            foreach (Flight f in returningFlights)
            {
                //get all tickets for the flight
                List<Ticket> bust = await _context.Tickets.Where(x => x.FlightId == f.Id && x.IsBooked == false && x.TicketClass == TicketClass.Business).ToListAsync();
                List<Ticket> ecot = await _context.Tickets.Where(x => x.FlightId == f.Id && x.IsBooked == false && x.TicketClass == TicketClass.Economy).ToListAsync();
                //Console.WriteLine(bust.Price);
                Ticket lowestBusiness = bust.OrderBy(x => x.Price).FirstOrDefault();
                Ticket lowestEconomy = ecot.OrderBy(x => x.Price).FirstOrDefault();
                int businessNum = bust.Count;
                int ecoNum = ecot.Count;
                FlightInfoDto fInfo = new FlightInfoDto
                {
                    EconomyTickets = ecoNum,
                    BusinessTickets = businessNum,
                    AirlineName = f.AirlineName,
                    FlightCode = f.FlightCode,
                    FromAirport = f.FromAirport,
                    FromCity = f.ToCity,
                    DepartureTime = f.DepartureTime,
                    ArrivalTime = f.ArrivalTime,
                    ToAirport = f.ToAirport,
                    ToCity = f.ToCity,
                    BusinessTicketPrice = -1,
                    BusinessTicketPriceId = -1,
                    EconomyTicketPrice = -1,
                    EconomyTicketPriceId = -1

                };

                if (lowestBusiness != null)
                {
                    fInfo.BusinessTicketPrice = lowestBusiness.Price;
                    fInfo.BusinessTicketPriceId = lowestBusiness.Id;
                }
                if (lowestEconomy != null)
                {
                    fInfo.EconomyTicketPrice = lowestEconomy.Price;
                    fInfo.EconomyTicketPriceId = lowestEconomy.Id;
                }
                if (lowestBusiness != null || lowestEconomy != null)
                {
                    returnFlightInfoList.Add(fInfo);
                }

            }
            result.DepartureFlights = flightInfoList;
            result.ReturnFlights = returnFlightInfoList;
            return result;
        }

        public async Task AddReservation(AddReservationDto addReservationDto)
        {
            Console.WriteLine(addReservationDto.ReturnTicketId);
            Ticket t = await _context.Tickets.FindAsync(addReservationDto.SelectedTicketId);
            Ticket rt = await _context.Tickets.FindAsync(addReservationDto.ReturnTicketId);
            t.IsBooked = true;
            if (rt != null)
            {
                rt.IsBooked = true;
            }

            User u = await _context.Users.FindAsync(_userService.GetUserId());
            var reservation = new Reservation
            {
                ReservationTime = DateTime.Now,
                Ticket = t,
                ReturnTicketId = addReservationDto.ReturnTicketId,
                User = u
            };
            //await _context.Tickets.Update(t);
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TwoWayReservationDto>> GetUserReservations()
        {
            List<Reservation> userReservations = await _context.Reservations.Where(x => x.UserId == _userService.GetUserId()).ToListAsync();
            //List<ReservationDto> reservationDtos = new List<ReservationDto>();
            List<TwoWayReservationDto> reservationDtos = new List<TwoWayReservationDto>();
            foreach (Reservation r in userReservations)
            {
                Ticket tik = await _context.Tickets.Include(x => x.Flight).Where(z => z.Id == r.TicketId).FirstOrDefaultAsync();
                //Console.WriteLine(tik.Flight.FlightCode);
                TwoWayReservationDto twrdto = new TwoWayReservationDto();
                ReservationDto rdto = new ReservationDto
                {
                    ReservationTime = r.ReservationTime,
                    UserId = r.UserId,
                    TicketId = r.TicketId,
                    AirlineName = tik.Flight.AirlineName.ToUpper(),
                    FromAirport = tik.Flight.FromAirport.ToUpper(),
                    FromCity = tik.Flight.FromCity.ToUpper(),
                    FlightCode = tik.Flight.FlightCode.ToUpper(),
                    TicketClass = tik.TicketClass,
                    ToAirport = tik.Flight.ToAirport.ToUpper(),
                    ToCity = tik.Flight.ToCity.ToUpper(),
                    DepartureTime = tik.Flight.DepartureTime,
                    ArrivalTime = tik.Flight.ArrivalTime,
                    TicketPrice = tik.Price,
                    ReturnTicketId = -1,
                    ReturnTicketPrice = -1,
                    FlightDurationDays = (tik.Flight.ArrivalTime - tik.Flight.DepartureTime).Days,
                    FlightDurationHours = (tik.Flight.ArrivalTime - tik.Flight.DepartureTime).Hours,
                    FlightDurationMinutes = (tik.Flight.ArrivalTime - tik.Flight.DepartureTime).Minutes

                };
                if (r.ReturnTicketId > 0)
                {
                    Ticket returnTicket = await _context.Tickets.Include(x => x.Flight).Where(z => z.Id == r.ReturnTicketId).FirstOrDefaultAsync();
                    ReservationDto rrdto = new ReservationDto
                    {
                        ReservationTime = r.ReservationTime,
                        UserId = r.UserId,
                        TicketId = r.ReturnTicketId,
                        AirlineName = returnTicket.Flight.AirlineName.ToUpper(),
                        FromAirport = returnTicket.Flight.FromAirport.ToUpper(),
                        FromCity = returnTicket.Flight.FromCity.ToUpper(),
                        FlightCode = returnTicket.Flight.FlightCode.ToUpper(),
                        TicketClass = returnTicket.TicketClass,
                        ToAirport = returnTicket.Flight.ToAirport.ToUpper(),
                        ToCity = returnTicket.Flight.ToCity.ToUpper(),
                        DepartureTime = returnTicket.Flight.DepartureTime,
                        ArrivalTime = returnTicket.Flight.ArrivalTime,
                        TicketPrice = returnTicket.Price,
                        ReturnTicketId = r.ReturnTicketId,
                        ReturnTicketPrice = returnTicket.Price,
                        FlightDurationDays = (returnTicket.Flight.ArrivalTime - returnTicket.Flight.DepartureTime).Days,
                        FlightDurationHours = (returnTicket.Flight.ArrivalTime - returnTicket.Flight.DepartureTime).Hours,
                        FlightDurationMinutes = (returnTicket.Flight.ArrivalTime - returnTicket.Flight.DepartureTime).Minutes

                    };
                    twrdto.ReturnReservation = rrdto;
                }
                twrdto.DepartureReservation = rdto;
                reservationDtos.Add(twrdto);
            }

            return reservationDtos;

        }

        public async Task<List<FlightDetailDto>> GetFlightsDetails()
        {
            var flights = await _context.Flights.ToListAsync();
            List<FlightDetailDto> flightDetailDtos = new List<FlightDetailDto>();
            foreach (Flight f in flights)
            {
                List<Ticket> bust = await _context.Tickets.Where(x => x.FlightId == f.Id/* && x.IsBooked == false*/ && x.TicketClass == TicketClass.Business).ToListAsync();
                List<Ticket> ecot = await _context.Tickets.Where(x => x.FlightId == f.Id /*&& x.IsBooked == false*/ && x.TicketClass == TicketClass.Economy).ToListAsync();
                FlightDetailDto fddto = new FlightDetailDto
                {
                    FlightId = f.Id,
                    AirlineName = f.AirlineName.ToUpper(),
                    ArrivalTime = f.ArrivalTime,
                    FlightCode = f.FlightCode.ToUpper(),
                    FromAirport = f.FromAirport.ToUpper(),
                    FromCity = f.FromCity.ToUpper(),
                    ToAirport = f.ToAirport.ToUpper(),
                    ToCity = f.ToCity.ToUpper(),
                    DepartureTime = f.DepartureTime,
                    BusinessTickets = bust.Count,
                    EconomyTickets = ecot.Count
                };

                flightDetailDtos.Add(fddto);
            }
            return flightDetailDtos;
        }

        public async Task<List<TicketDetailsDto>> GetTicketsDetails()
        {
            var tickets = await _context.Tickets.Include(x => x.Flight).ToListAsync();
            List<TicketDetailsDto> ticketDetailsDtos = new List<TicketDetailsDto>();
            foreach (Ticket t in tickets)
            {
                User user = null;
                if(t.IsBooked)
                {
                    var reservation = await _context.Reservations.Include(x => x.User).Where(x => x.TicketId == t.Id || x.ReturnTicketId == t.Id).FirstOrDefaultAsync();
                    if(reservation != null)
                    {
                        user = reservation.User;
                    }
                }
                TicketDetailsDto tddto = new TicketDetailsDto
                {
                    TicketId = t.Id,
                    FlightCode = t.Flight.FlightCode,
                    Price = t.Price,
                    TicketClass = t.TicketClass,
                    UserId = (user == null) ? -1 : user.Id,
                    Username = (user == null) ? "" : user.Username

                };
                ticketDetailsDtos.Add(tddto);
            }
            return ticketDetailsDtos;
        }
    }
        
}
