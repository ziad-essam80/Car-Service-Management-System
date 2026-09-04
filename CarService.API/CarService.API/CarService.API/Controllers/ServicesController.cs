using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarService.API.Data;
using CarService.API.Models;

namespace CarService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly CarServiceDbContext _context;

        public ServicesController(CarServiceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            return await _context.Services
                .OrderByDescending(service => service.Id)
                .ToListAsync();
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableServices()
        {
            var services = await _context.Services
                .Where(service => service.IsActive)
                .Select(service => new
                {
                    id = service.Id,
                    name = service.Name,
                    description = service.Description,
                    price = service.Price,
                    imageUrl = service.ImageUrl
                })
                .ToListAsync();

            return Ok(services);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetService(int id)
        {
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound(new
                {
                    message = "Service not found."
                });
            }

            return service;
        }

        [HttpPost]
        public async Task<ActionResult<Service>> AddService(Service service)
        {
            service.IsActive = true;

            _context.Services.Add(service);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetService),
                new { id = service.Id },
                service
            );
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(
            int id,
            Service service)
        {
            if (id != service.Id)
            {
                return BadRequest(new
                {
                    message = "Service ID does not match."
                });
            }

            var oldService =
                await _context.Services.FindAsync(id);

            if (oldService == null)
            {
                return NotFound(new
                {
                    message = "Service not found."
                });
            }

            oldService.Name = service.Name;
            oldService.Price = service.Price;
            oldService.Description = service.Description;
            oldService.ImageUrl = service.ImageUrl;
            oldService.IsActive = service.IsActive;

            await _context.SaveChangesAsync();

            return Ok(oldService);
        }

        [HttpPut("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var service =
                await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound(new
                {
                    message = "Service not found."
                });
            }

            service.IsActive = !service.IsActive;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = service.Id,
                name = service.Name,
                isActive = service.IsActive
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service =
                await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound(new
                {
                    message = "Service not found."
                });
            }

            var usedInBookings =
                await _context.Bookings
                    .AnyAsync(booking =>
                        booking.ServiceId == id
                    );

            if (usedInBookings)
            {
                return BadRequest(new
                {
                    message =
                        "This service is already used in bookings. Disable it instead of deleting it."
                });
            }

            _context.Services.Remove(service);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}