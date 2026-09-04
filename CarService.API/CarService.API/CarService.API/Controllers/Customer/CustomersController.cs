using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarService.API.Data;
using CarService.API.Models;

namespace CarService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CarServiceDbContext _context;

        public CustomersController(CarServiceDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var activeDate =
                DateOnly.FromDateTime(
                    DateTime.Today.AddDays(-90)
                );


            var customers = await _context.Customers
                .Select(customer => new
                {
                    id = customer.Id,

                    name = customer.FullName,

                    email = customer.Email,

                    phone = customer.Phone,

                    cars = _context.Cars
                        .Count(car =>
                            car.CustomerId == customer.Id
                        ),

                    lastVisit = _context.Bookings
                        .Where(booking =>
                            booking.CustomerId == customer.Id
                        )
                        .OrderByDescending(booking =>
                            booking.Date
                        )
                        .Select(booking =>
                            (DateOnly?)booking.Date
                        )
                        .FirstOrDefault(),

                    status = _context.Bookings.Any(
                        booking =>
                            booking.CustomerId == customer.Id &&
                            booking.Date >= activeDate
                    )
                    ? "Active"
                    : "Inactive"
                })
                .ToListAsync();


            return Ok(customers);
        }


        // GET: api/customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }


        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<Customer>> AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCustomer),
                new { id = customer.Id },
                customer
            );
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(
            int id,
            [FromQuery] bool force = false)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Customer not found."
                });
            }


            var carsCount = await _context.Cars
                .CountAsync(c => c.CustomerId == id);

            var bookingsCount = await _context.Bookings
                .CountAsync(b => b.CustomerId == id);


            if (!force && (carsCount > 0 || bookingsCount > 0))
            {
                return Conflict(new
                {
                    message = "This customer has related data. Confirm permanent deletion.",
                    cars = carsCount,
                    bookings = bookingsCount,
                    requiresForce = true
                });
            }


            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var bookings = await _context.Bookings
                    .Where(b => b.CustomerId == id)
                    .ToListAsync();


                var bookingIds = bookings
                    .Select(b => b.Id)
                    .ToList();


                var invoices = await _context.Invoices
                    .Where(i => bookingIds.Contains(i.BookingId))
                    .ToListAsync();


                var serviceJobs = await _context.ServiceJobs
                    .Where(j => bookingIds.Contains(j.BookingId))
                    .ToListAsync();


                var cars = await _context.Cars
                    .Where(c => c.CustomerId == id)
                    .ToListAsync();


                _context.Invoices.RemoveRange(invoices);

                _context.ServiceJobs.RemoveRange(serviceJobs);

                _context.Bookings.RemoveRange(bookings);

                _context.Cars.RemoveRange(cars);

                _context.Customers.Remove(customer);


                await _context.SaveChangesAsync();

                await transaction.CommitAsync();


                return Ok(new
                {
                    message = "Customer and all related data deleted permanently."
                });
            }
            catch
            {
                await transaction.RollbackAsync();

                return StatusCode(500, new
                {
                    message = "An error occurred while deleting the customer."
                });
            }
        }


        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(customer =>
                customer.Id == id
            );
        }

        [HttpGet("total-count")]
        public async Task<IActionResult> GetTotalCustomers()
        {
            var count = await _context.Customers.CountAsync();

            return Ok(new
            {
                totalCustomers = count
            });
        }


        [HttpGet("active-count")]
        public async Task<IActionResult> GetActiveCustomers()
        {
            var date = DateOnly.FromDateTime(
                DateTime.Today.AddDays(-90)
            );

            var count = await _context.Bookings
                .Where(b => b.Date >= date)
                .Select(b => b.CustomerId)
                .Distinct()
                .CountAsync();

            return Ok(new
            {
                activeCustomers = count
            });
        }


        [HttpGet("new-count")]
        public async Task<IActionResult> GetNewCustomers()
        {
            var today = DateTime.Today;

            var firstDay =
                new DateOnly(today.Year, today.Month, 1);

            var nextMonth =
                firstDay.AddMonths(1);


            var count = await _context.Bookings
                .GroupBy(b => b.CustomerId)
                .Where(group =>
                    group.Min(b => b.Date) >= firstDay &&
                    group.Min(b => b.Date) < nextMonth
                )
                .CountAsync();


            return Ok(new
            {
                newCustomers = count
            });
        }


        [HttpGet("registered-vehicles")]
        public async Task<IActionResult> GetRegisteredVehicles()
        {
            var cars = await _context.Cars.CountAsync();

            var customers =
                await _context.Customers.CountAsync();


            double carsPerCustomer = 0;


            if (customers > 0)
            {
                carsPerCustomer =
                    Math.Round(
                        (double)cars / customers,
                        1
                    );
            }


            return Ok(new
            {
                registeredVehicles = cars,
                carsPerCustomer = carsPerCustomer
            });
        }

    }
}