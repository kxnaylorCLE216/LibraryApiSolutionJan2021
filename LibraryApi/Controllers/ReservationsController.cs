using AutoMapper;
using AutoMapper.QueryableExtensions;
using LibraryApi.Domain;
using LibraryApi.Filters;
using LibraryApi.Models.Reservations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Controllers
{
    public class ReservationsController : ControllerBase
    {
        private readonly LibraryDataContext _context;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _config;
        private readonly IProcessReservation _reservationProccessor;

        public ReservationsController(LibraryDataContext context, IMapper mapper, MapperConfiguration config, IProcessReservation reservationProccessor)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
            _reservationProccessor = reservationProccessor;
        }

        // POST /reservations
        [HttpPost("/reservations")]
        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 5)]
        [ValidateModel]
        public async Task<ActionResult> CreateReservation([FromBody] PostReservationRequest request)
        {
            // Update the domain (POST is unsafe - it does work. What work will we do?)
            // -- Create and Process a new Reservation (in our synch model)
            // -- Save it to the database.
            // PostReservationRequest -> Reservation -> GetReservationDetailsResponse

            //var reservation = new Reservation
            //{
            //    For = request.For,
            //    Books = request.Books,
            //    Status = ReservationStatus.Accepted,
            //    CreatedAt = DateTime.Now
            //};

            // Fake Processing the Order
            //  - Each book takes 1 second.
            var reservation = _mapper.Map<Reservation>(request);
            //var numberOfBooks = reservation.Books.Split(',').Length;
            //for (var t = 0; t < numberOfBooks; t++)
            //{
            //    await Task.Delay(1000);
            //}
            //reservation.Status = ReservationStatus.Accepted;

            //Tell sonething else - somehow - to work on this outside the request/Response cycle.

            reservation.Status = ReservationStatus.Pending;
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            await _reservationProccessor.ProcessReservation(reservation);

            //var response = new GetReservationDetailsResponse
            //{
            //    Id = reservation.Id,
            //    For = reservation.For,
            //    Books = reservation.Books,
            //    Status = reservation.Status
            //};

            var response = _mapper.Map<GetReservationDetailsResponse>(reservation);

            // Return:
            //  - 201 Created
            //  - Location Header
            //  - A copy of what they'd get if they followed that header.
            //  - Bonus: A cache header!
            return CreatedAtRoute("reservation#getareservation", new { id = response.Id }, response);
        }

        // GET /reservations/{id}
        [HttpGet("/reservations/{id:int}", Name = "reservation#getareservation")]
        public async Task<ActionResult<GetReservationDetailsResponse>> GetAReservation(int id)
        {
            var storedReservation = await _context.Reservations
                .ProjectTo<GetReservationDetailsResponse>(_config)
                .SingleOrDefaultAsync(r => r.Id == id);

            var response = _mapper.Map<GetReservationDetailsResponse>(storedReservation);
            return this.Maybe(response);
        }
        // Async

        //When the BW Accepts a reservation
        [HttpPost("/reservations/accepted")]
        public async Task<ActionResult> ReservationAccepted([FromBody] GetReservationDetailsResponse request)
        {
            var reservation = await _context.Reservations
                .SingleOrDefaultAsync(r => r.Id == request.Id && r.Status == ReservationStatus.Pending);

            if (reservation == null)
            {
                return BadRequest("No pending Reservation with that Id");
            }
            else
            {
                reservation.Status = ReservationStatus.Accepted;
                await _context.SaveChangesAsync();
            }

            return Accepted();
        }

        //When the BW Accepts a reservation
        [HttpPost("/reservations/rejected")]
        public async Task<ActionResult> ReservationRejected([FromBody] GetReservationDetailsResponse request)
        {
            var reservation = await _context.Reservations
               .SingleOrDefaultAsync(r => r.Id == request.Id && r.Status == ReservationStatus.Pending);

            if (reservation == null)
            {
                return BadRequest("No pending Reservation with that Id");
            }
            else
            {
                reservation.Status = ReservationStatus.Rejected;
                await _context.SaveChangesAsync();
            }

            return Accepted();
        }

        [HttpGet("/reservations/pending")]
        public async Task<ActionResult> GetPendingReservations()
        {
            var reservations = await _context.Reservations
                .Where(res => res.Status == ReservationStatus.Pending)
                .ToListAsync();

            return Ok(new { data = reservations });
        }

        [HttpGet("/reservations/rejected")]
        public async Task<ActionResult> GetRejectedReservations()
        {
            var reservations = await _context.Reservations
                .Where(res => res.Status == ReservationStatus.Rejected)
                .ToListAsync();

            return Ok(new { data = reservations });
        }

        [HttpGet("/reservations/accepted")]
        public async Task<ActionResult> GetAcceptedReservations()
        {
            var reservations = await _context.Reservations
                .Where(res => res.Status == ReservationStatus.Accepted)
                .ToListAsync();

            return Ok(new { data = reservations });
        }
        [HttpGet("/reservations")]
        public async Task<ActionResult> GetAllReservations()
        {
            var reservations = await _context.Reservations.ToListAsync();

            return Ok(new { data = reservations });
        }
    }
}