using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using CarService.API.Data;
using CarService.API.DTOs;
using CarService.API.Models;

namespace CarService.API.Controllers
{
    [Authorize(Roles = "Customer")]
    [Route("api/customer/bookings")]
    [ApiController]
    public class CustomerBookingsController : ControllerBase
    {
        private readonly CarServiceDbContext _context;

        public CustomerBookingsController(
            CarServiceDbContext context)
        {
            _context = context;
        }


        // ============================================
        // CREATE BOOKING
        // ============================================

        [HttpPost]
        public async Task<IActionResult> CreateBooking(
            CreateBookingRequest request)
        {
            var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(idValue, out int customerId))
            {
                return Unauthorized();
            }


            var car = await _context.Cars
                .FirstOrDefaultAsync(car =>
                    car.Id == request.CarId &&
                    car.CustomerId == customerId
                );


            if (car == null)
            {
                return BadRequest(new
                {
                    message =
                        "Car not found or does not belong to this customer."
                });
            }


            var service = await _context.Services
                .FirstOrDefaultAsync(service =>
                    service.Id == request.ServiceId &&
                    service.IsActive
                );


            if (service == null)
            {
                return BadRequest(new
                {
                    message = "Service is not available."
                });
            }


            var today =
                DateOnly.FromDateTime(DateTime.Today);


            if (request.Date < today)
            {
                return BadRequest(new
                {
                    message =
                        "Booking date cannot be in the past."
                });
            }


            var appointmentExists =
                await _context.Bookings.AnyAsync(
                    booking =>
                        booking.Date == request.Date &&
                        booking.Time == request.Time &&
                        booking.Status != "Completed"
                );


            if (appointmentExists)
            {
                return BadRequest(new
                {
                    message =
                        "This appointment time is already booked."
                });
            }


            var booking = new Booking
            {
                CustomerId = customerId,
                CarId = request.CarId,
                ServiceId = request.ServiceId,
                Date = request.Date,
                Time = request.Time,
                Status = "Booked",
                TotalPrice = service.Price
            };

            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync();

            var serviceJob = new ServiceJob
            {
                BookingId = booking.Id,
                TechnicianId = null,
                Status = "Booked",
                InspectionNotes = null,
                FinalCost = null
            };

            _context.ServiceJobs.Add(serviceJob);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Booking created successfully",
                bookingId = booking.Id
            });

        }

            // ============================================
            // MY BOOKINGS
            // ============================================

            [HttpGet]
        public async Task<IActionResult> GetMyBookings()
        {
            var customerId = GetCustomerId();

            if (customerId == null)
            {
                return Unauthorized();
            }


            var bookings = await (
                from booking in _context.Bookings

                join car in _context.Cars
                    on booking.CarId equals car.Id

                join service in _context.Services
                    on booking.ServiceId equals service.Id

                where booking.CustomerId == customerId

                orderby booking.Date descending

                select new
                {
                    id = booking.Id,

                    carId = car.Id,

                    vehicle =
                        car.Brand + " " + car.Model,

                    plateNumber =
                        car.PlateNumber,

                    serviceId =
                        service.Id,

                    serviceName =
                        service.Name,

                    date =
                        booking.Date,

                    time =
                        booking.Time,

                    status =
                        booking.Status,

                    totalPrice =
                        booking.TotalPrice
                }

            ).ToListAsync();


            return Ok(bookings);
        }


        // ============================================
        // BOOKING DETAILS
        // ============================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingDetails(int id)
        {
            var customerId = GetCustomerId();

            if (customerId == null)
            {
                return Unauthorized();
            }


            var booking = await (
                from b in _context.Bookings

                join car in _context.Cars
                    on b.CarId equals car.Id

                join service in _context.Services
                    on b.ServiceId equals service.Id

                where
                    b.Id == id &&
                    b.CustomerId == customerId

                select new
                {
                    id = b.Id,

                    vehicle = new
                    {
                        id = car.Id,

                        brand = car.Brand,

                        model = car.Model,

                        year = car.Year,

                        plateNumber =
                            car.PlateNumber,

                        color =
                            car.Color
                    },

                    service = new
                    {
                        id = service.Id,

                        name = service.Name,

                        description =
                            service.Description,

                        estimatedTime =
                            service.EstimatedTime
                    },

                    date =
                        b.Date,

                    time =
                        b.Time,

                    status =
                        b.Status,

                    totalPrice =
                        b.TotalPrice
                }

            ).FirstOrDefaultAsync();


            if (booking == null)
            {
                return NotFound(new
                {
                    message = "Booking not found."
                });
            }


            return Ok(booking);
        }


        // ============================================
        // TRACK SERVICE
        // ============================================

        [HttpGet("{id}/tracking")]
        public async Task<IActionResult> TrackService(int id)
        {
            var customerId = GetCustomerId();

            if (customerId == null)
            {
                return Unauthorized();
            }


            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b =>
                    b.Id == id &&
                    b.CustomerId == customerId
                );


            if (booking == null)
            {
                return NotFound(new
                {
                    message = "Booking not found."
                });
            }


            var statuses = new[]
            {
                "Booked",
                "Confirmed",
                "Vehicle Received",
                "Inspection",
                "In Service",
                "Quality Check",
                "Ready for Pickup",
                "Completed"
            };


            var currentIndex =
                Array.IndexOf(
                    statuses,
                    booking.Status
                );


            var tracking = statuses
                .Select((status, index) => new
                {
                    name = status,

                    completed =
                        index < currentIndex,

                    current =
                        index == currentIndex,

                    pending =
                        index > currentIndex
                })
                .ToList();


            var serviceJob =
                await _context.ServiceJobs
                    .FirstOrDefaultAsync(
                        job =>
                            job.BookingId == booking.Id
                    );


            return Ok(new
            {
                bookingId =
                    booking.Id,

                currentStatus =
                    booking.Status,

                inspectionNotes =
                    serviceJob?.InspectionNotes,

                finalCost =
                    serviceJob?.FinalCost,

                tracking
            });
        }


        // ============================================
        // MAINTENANCE HISTORY
        // ============================================

        [HttpGet("history")]
        public async Task<IActionResult> GetMaintenanceHistory()
        {
            var customerId = GetCustomerId();

            if (customerId == null)
            {
                return Unauthorized();
            }


            var history = await (
                from booking in _context.Bookings

                join car in _context.Cars
                    on booking.CarId equals car.Id

                join service in _context.Services
                    on booking.ServiceId equals service.Id

                where
                    booking.CustomerId == customerId &&
                    booking.Status == "Completed"

                orderby booking.Date descending

                select new
                {
                    bookingId =
                        booking.Id,

                    vehicle =
                        car.Brand + " " + car.Model,

                    plateNumber =
                        car.PlateNumber,

                    service =
                        service.Name,

                    date =
                        booking.Date,

                    totalPrice =
                        booking.TotalPrice,

                    status =
                        booking.Status
                }

            ).ToListAsync();


            return Ok(history);
        }


        // ============================================
        // HELPER
        // ============================================

        private int? GetCustomerId()
        {
            var id =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );


            if (int.TryParse(
                id,
                out int customerId))
            {
                return customerId;
            }


            return null;
        }
    }
}