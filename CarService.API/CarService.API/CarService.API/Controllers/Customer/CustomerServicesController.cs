using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using CarService.API.Data;

namespace CarService.API.Controllers
{
    [Authorize(Roles = "Customer")]
    [Route("api/customer/services")]
    [ApiController]
    public class CustomerServicesController : ControllerBase
    {
        private readonly CarServiceDbContext _context;

        public CustomerServicesController(CarServiceDbContext context)
        {
            _context = context;
        }


        // GET: api/customer/services
        [HttpGet]
        public async Task<IActionResult> GetServices()
        {
            var services = await _context.Services
                .Where(service => service.IsActive)
                .Select(service => new
                {
                    id = service.Id,

                    name = service.Name,

                    description = service.Description,

                    price = service.Price,

                    estimatedTime = service.EstimatedTime,

                    imageUrl = service.ImageUrl
                })
                .ToListAsync();


            return Ok(services);
        }


        // GET: api/customer/services/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceDetails(int id)
        {
            var service = await _context.Services
                .Where(service =>
                    service.Id == id &&
                    service.IsActive
                )
                .Select(service => new
                {
                    id = service.Id,

                    name = service.Name,

                    description = service.Description,

                    price = service.Price,

                    estimatedTime = service.EstimatedTime,

                    imageUrl = service.ImageUrl
                })
                .FirstOrDefaultAsync();


            if (service == null)
            {
                return NotFound(new
                {
                    message = "Service not found."
                });
            }


            return Ok(service);
        }
    }
}