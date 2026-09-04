using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarService.API.Data;

namespace CarService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly CarServiceDbContext _context;

        public DashboardController(CarServiceDbContext context)
        {
            _context = context;
        }


        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var data = new
            {
                totalCars = await _context.Cars.CountAsync(),
                totalServices = await _context.Services.CountAsync(),
                totalBookings = await _context.Bookings.CountAsync(),
                totalInvoices = await _context.Invoices.CountAsync()
            };

            return Ok(data);
        }


        [HttpGet("service-overview")]
        public async Task<IActionResult> GetServiceOverview()
        {
            var data = await _context.Bookings
                .GroupBy(b => b.ServiceId)
                .Select(group => new
                {
                    ServiceId = group.Key,
                    Value = group.Count()
                })
                .OrderByDescending(x => x.Value)
                .Take(5)
                .Join(
                    _context.Services,
                    booking => booking.ServiceId,
                    service => service.Id,
                    (booking, service) => new
                    {
                        name = service.Name,
                        value = booking.Value
                    }
                )
                .ToListAsync();

            return Ok(data);
        }


        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointments()
        {
            var data = await (
                from booking in _context.Bookings

                join car in _context.Cars
                    on booking.CarId equals car.Id

                join customer in _context.Customers
                    on booking.CustomerId equals customer.Id

                join service in _context.Services
                    on booking.ServiceId equals service.Id

                orderby booking.Id descending

                select new
                {
                    id = booking.Id,
                    time = booking.Time,
                    vehicle = car.Brand + " " + car.Model,
                    customer = customer.FullName,
                    service = service.Name,
                    status = booking.Status
                }
            )
            .Take(4)
            .ToListAsync();

            return Ok(data);
        }


        [HttpGet("service-jobs")]
        public async Task<IActionResult> GetServiceJobs()
        {
            var data = await (
                from job in _context.ServiceJobs

                join booking in _context.Bookings
                    on job.BookingId equals booking.Id

                join car in _context.Cars
                    on booking.CarId equals car.Id

                join customer in _context.Customers
                    on booking.CustomerId equals customer.Id

                join service in _context.Services
                    on booking.ServiceId equals service.Id

                join technician in _context.Technicians
                    on job.TechnicianId equals technician.Id

                orderby job.Id descending

                select new
                {
                    id = job.Id,
                    vehicle = car.Brand + " " + car.Model,
                    customer = customer.FullName,
                    service = service.Name,
                    mechanic = technician.FullName,
                    status = job.Status
                }
            )
            .Take(5)
            .ToListAsync();

            return Ok(data);
        }


        [HttpGet("mechanics-performance")]
        public async Task<IActionResult> GetMechanicsPerformance()
        {
            var data = await (
                from job in _context.ServiceJobs

                join technician in _context.Technicians
                    on job.TechnicianId equals technician.Id

                group job by new
                {
                    technician.Id,
                    technician.FullName
                }
                into mechanicGroup

                orderby mechanicGroup.Count() descending

                select new
                {
                    id = mechanicGroup.Key.Id,
                    name = mechanicGroup.Key.FullName,
                    value = mechanicGroup.Count()
                }
            )
            .Take(5)
            .ToListAsync();

            return Ok(data);
        }
    }
}