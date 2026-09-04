using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarService.API.Data;
using CarService.API.Models;

namespace CarService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly CarServiceDbContext _context;

        public OrdersController(CarServiceDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceJob>>> GetOrders()
        {
            return await _context.ServiceJobs.ToListAsync();
        }


        [HttpGet("details")]
        public async Task<IActionResult> GetServiceJobsDetails()
        {
            var jobs = await (
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
                    into technicianGroup

                from technician in technicianGroup.DefaultIfEmpty()

                orderby job.Id descending

                select new
                {
                    id = job.Id,

                    bookingId = booking.Id,

                    customerId = customer.Id,

                    customerName = customer.FullName,

                    carId = car.Id,

                    vehicle =
                        car.Brand + " " + car.Model,

                    plateNumber = car.PlateNumber,

                    serviceId = service.Id,

                    serviceName = service.Name,

                    date = booking.Date,

                    time = booking.Time,

                    status = booking.Status,

                    technicianId = job.TechnicianId,

                    technicianName =
                        technician != null
                            ? technician.FullName
                            : "Not Assigned",

                    inspectionNotes =
                        job.InspectionNotes,

                    finalCost =
                        job.FinalCost,

                    totalPrice =
                        booking.TotalPrice
                }

            ).ToListAsync();

            return Ok(jobs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceJob>> GetOrder(int id)
        {
            var order = await _context.ServiceJobs.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceJob>> AddOrder(ServiceJob order)
        {
            _context.ServiceJobs.Add(order);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetOrder),
                new { id = order.Id },
                order
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(
            int id,
            ServiceJob order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State =
                EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order =
                await _context.ServiceJobs.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            _context.ServiceJobs.Remove(order);

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPut("{id}/next-status")]
        public async Task<IActionResult> NextStatus(int id)
        {
            var order = await _context.ServiceJobs.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(order.BookingId);

            if (booking == null)
            {
                return NotFound();
            }

            switch (booking.Status)
            {
                case "Booked":
                    booking.Status = "Confirmed";
                    break;

                case "Confirmed":
                    booking.Status = "Vehicle Received";
                    break;

                case "Vehicle Received":
                    booking.Status = "Inspection";
                    break;

                case "Inspection":
                    booking.Status = "In Service";
                    break;

                case "In Service":
                    booking.Status = "Quality Check";
                    break;

                case "Quality Check":
                    booking.Status = "Ready for Pickup";
                    break;

                case "Ready for Pickup":
                    booking.Status = "Completed";
                    break;

                case "Completed":
                    return BadRequest(new
                    {
                        message = "Order is already completed."
                    });

                default:
                    return BadRequest(new
                    {
                        message = "Invalid order status."
                    });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = order.Id,
                status = booking.Status
            });
        }


        private bool OrderExists(int id)
        {
            return _context.ServiceJobs
                .Any(order => order.Id == id);
        }
    }
}