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
    [Route("api/customer/cars")]
    [ApiController]
    public class CustomerCarsController : ControllerBase
    {
        private readonly CarServiceDbContext _context;

        public CustomerCarsController(CarServiceDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetMyCars()
        {
            var customerId = GetCustomerId();

            if (customerId == null)
            {
                return Unauthorized();
            }


            var cars = await _context.Cars
                .Where(car => car.CustomerId == customerId)
                .Select(car => new
                {
                    id = car.Id,

                    brand = car.Brand,

                    model = car.Model,

                    year = car.Year,

                    plateNumber = car.PlateNumber,

                    color = car.Color,

                    mileage = car.Mileage
                })
                .ToListAsync();


            return Ok(cars);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarDetails(int id)
        {
            var customerId = GetCustomerId();

            if (customerId == null)
            {
                return Unauthorized();
            }


            var car = await _context.Cars
                .FirstOrDefaultAsync(car =>
                    car.Id == id &&
                    car.CustomerId == customerId
                );


            if (car == null)
            {
                return NotFound(new
                {
                    message = "Car not found."
                });
            }


            var lastService = await (
                from booking in _context.Bookings

                join service in _context.Services
                    on booking.ServiceId equals service.Id

                where
                    booking.CarId == id &&
                    booking.CustomerId == customerId &&
                    booking.Status == "Completed"

                orderby booking.Date descending

                select new
                {
                    bookingId = booking.Id,

                    serviceName = service.Name,

                    date = booking.Date,

                    status = booking.Status
                }

            ).FirstOrDefaultAsync();


            var currentBooking = await (
                from booking in _context.Bookings

                join service in _context.Services
                    on booking.ServiceId equals service.Id

                where
                    booking.CarId == id &&
                    booking.CustomerId == customerId &&
                    booking.Status != "Completed"

                orderby booking.Date descending

                select new
                {
                    bookingId = booking.Id,

                    serviceName = service.Name,

                    date = booking.Date,

                    time = booking.Time,

                    status = booking.Status
                }

            ).FirstOrDefaultAsync();


            return Ok(new
            {
                id = car.Id,

                brand = car.Brand,

                model = car.Model,

                year = car.Year,

                plateNumber = car.PlateNumber,

                color = car.Color,

                mileage = car.Mileage,

                lastService,

                currentBooking
            });
        }


        [HttpPost]
        public async Task<IActionResult> AddCar(
            AddCarRequest request)
        {
            var customerId = GetCustomerId();

            if (customerId == null)
            {
                return Unauthorized();
            }


            if (string.IsNullOrWhiteSpace(request.Brand) ||
                string.IsNullOrWhiteSpace(request.Model) ||
                string.IsNullOrWhiteSpace(request.PlateNumber))
            {
                return BadRequest(new
                {
                    message =
                        "Brand, model and plate number are required."
                });
            }


            if (request.Year < 1980 ||
                request.Year > DateTime.Now.Year + 1)
            {
                return BadRequest(new
                {
                    message = "Invalid car year."
                });
            }


            if (request.Mileage < 0)
            {
                return BadRequest(new
                {
                    message = "Mileage cannot be negative."
                });
            }


            var plateExists = await _context.Cars
                .AnyAsync(car =>
                    car.PlateNumber == request.PlateNumber
                );


            if (plateExists)
            {
                return BadRequest(new
                {
                    message =
                        "This plate number is already registered."
                });
            }


            var car = new Car
            {
                CustomerId = customerId.Value,

                Brand = request.Brand,

                Model = request.Model,

                Year = request.Year,

                PlateNumber = request.PlateNumber,

                Color = request.Color,

                Mileage = request.Mileage
            };


            _context.Cars.Add(car);

            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Car added successfully.",

                car
            });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(
            int id,
            UpdateCarRequest request)
        {
            var customerId = GetCustomerId();

            if (customerId == null)
            {
                return Unauthorized();
            }


            var car = await _context.Cars
                .FirstOrDefaultAsync(car =>
                    car.Id == id &&
                    car.CustomerId == customerId
                );


            if (car == null)
            {
                return NotFound(new
                {
                    message = "Car not found."
                });
            }


            if (string.IsNullOrWhiteSpace(request.Brand) ||
                string.IsNullOrWhiteSpace(request.Model) ||
                string.IsNullOrWhiteSpace(request.PlateNumber))
            {
                return BadRequest(new
                {
                    message =
                        "Brand, model and plate number are required."
                });
            }


            if (request.Year < 1980 ||
                request.Year > DateTime.Now.Year + 1)
            {
                return BadRequest(new
                {
                    message = "Invalid car year."
                });
            }


            if (request.Mileage < 0)
            {
                return BadRequest(new
                {
                    message = "Mileage cannot be negative."
                });
            }


            var plateExists = await _context.Cars
                .AnyAsync(otherCar =>
                    otherCar.PlateNumber ==
                        request.PlateNumber
                    &&
                    otherCar.Id != id
                );


            if (plateExists)
            {
                return BadRequest(new
                {
                    message =
                        "This plate number is already registered."
                });
            }


            car.Brand = request.Brand;

            car.Model = request.Model;

            car.Year = request.Year;

            car.PlateNumber = request.PlateNumber;

            car.Color = request.Color;

            car.Mileage = request.Mileage;


            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Car updated successfully.",

                car
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var customerId = GetCustomerId();

            if (customerId == null)
            {
                return Unauthorized();
            }


            var car = await _context.Cars
                .FirstOrDefaultAsync(car =>
                    car.Id == id &&
                    car.CustomerId == customerId
                );


            if (car == null)
            {
                return NotFound(new
                {
                    message = "Car not found."
                });
            }


            var hasBookings = await _context.Bookings
                .AnyAsync(booking =>
                    booking.CarId == id
                );


            if (hasBookings)
            {
                return BadRequest(new
                {
                    message =
                        "This car has bookings and cannot be deleted."
                });
            }


            _context.Cars.Remove(car);

            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Car deleted successfully."
            });
        }


        private int? GetCustomerId()
        {
            var id = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );


            if (int.TryParse(id, out int customerId))
            {
                return customerId;
            }


            return null;
        }
    }
}