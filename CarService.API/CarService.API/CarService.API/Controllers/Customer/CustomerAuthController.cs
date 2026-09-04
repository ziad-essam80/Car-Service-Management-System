using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using CarService.API.Data;
using CarService.API.DTOs;
using CarService.API.Models;

namespace CarService.API.Controllers
{
    [Route("api/customer-auth")]
    [ApiController]
    public class CustomerAuthController : ControllerBase
    {
        private readonly CarServiceDbContext _context;

        public CustomerAuthController(CarServiceDbContext context)
        {
            _context = context;
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register(
            CustomerRegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Phone) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    message = "All fields are required."
                });
            }


            if (request.Password != request.ConfirmPassword)
            {
                return BadRequest(new
                {
                    message = "Passwords do not match."
                });
            }


            var emailExists = await _context.Customers
                .AnyAsync(customer =>
                    customer.Email == request.Email
                );


            if (emailExists)
            {
                return BadRequest(new
                {
                    message = "Email is already registered."
                });
            }


            var phoneExists = await _context.Customers
                .AnyAsync(customer =>
                    customer.Phone == request.Phone
                );


            if (phoneExists)
            {
                return BadRequest(new
                {
                    message = "Phone number is already registered."
                });
            }


            var customer = new Customer
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Password = request.Password,
                IsActive = true,
                CreatedAt = DateTime.Now
            };


            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Registration successful.",

                customer = new
                {
                    id = customer.Id,
                    fullName = customer.FullName,
                    email = customer.Email,
                    phone = customer.Phone
                }
            });
        }


        // =========================
        // LOGIN
        // =========================

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            CustomerLoginRequest request)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c =>
                    c.Email == request.Email
                );


            if (customer == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }


            if (!customer.IsActive)
            {
                return Unauthorized(new
                {
                    message = "This account is inactive."
                });
            }


            if (customer.Password != request.Password)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }


            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    customer.Id.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    customer.FullName
                ),

                new Claim(
                    ClaimTypes.Email,
                    customer.Email
                ),

                new Claim(
                    ClaimTypes.Role,
                    "Customer"
                )
            };


            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );


            var principal =
                new ClaimsPrincipal(identity);


            var properties =
                new AuthenticationProperties
                {
                    IsPersistent = true,

                    ExpiresUtc =
                        DateTimeOffset.UtcNow.AddHours(2)
                };


            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                properties
            );


            return Ok(new
            {
                message = "Login successful.",

                customer = new
                {
                    id = customer.Id,
                    fullName = customer.FullName,
                    email = customer.Email,
                    phone = customer.Phone,
                    role = "Customer"
                }
            });
        }


        // =========================
        // CURRENT CUSTOMER
        // =========================

        [Authorize(Roles = "Customer")]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                id = User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                ),

                fullName = User.FindFirstValue(
                    ClaimTypes.Name
                ),

                email = User.FindFirstValue(
                    ClaimTypes.Email
                ),

                role = User.FindFirstValue(
                    ClaimTypes.Role
                )
            });
        }


        // =========================
        // PROFILE
        // =========================

        [Authorize(Roles = "Customer")]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var idValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );


            if (!int.TryParse(idValue, out int customerId))
            {
                return Unauthorized();
            }


            var customer = await _context.Customers
                .Where(c => c.Id == customerId)
                .Select(c => new
                {
                    id = c.Id,
                    fullName = c.FullName,
                    email = c.Email,
                    phone = c.Phone,
                    createdAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();


            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Customer not found."
                });
            }


            return Ok(customer);
        }


        // =========================
        // EDIT PROFILE
        // =========================

        [Authorize(Roles = "Customer")]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(
            UpdateCustomerProfileRequest request)
        {
            var idValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );


            if (!int.TryParse(idValue, out int customerId))
            {
                return Unauthorized();
            }


            var customer =
                await _context.Customers.FindAsync(customerId);


            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Customer not found."
                });
            }


            var emailExists = await _context.Customers
                .AnyAsync(c =>
                    c.Email == request.Email &&
                    c.Id != customerId
                );


            if (emailExists)
            {
                return BadRequest(new
                {
                    message = "Email is already used by another customer."
                });
            }


            customer.FullName =
                request.FullName;

            customer.Email =
                request.Email;

            customer.Phone =
                request.Phone;


            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Profile updated successfully.",

                customer = new
                {
                    id = customer.Id,
                    fullName = customer.FullName,
                    email = customer.Email,
                    phone = customer.Phone
                }
            });
        }


        // =========================
        // LOGOUT
        // =========================

        [Authorize(Roles = "Customer")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );


            return Ok(new
            {
                message = "Logout successful."
            });
        }


        [Authorize(Roles = "Customer")]
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var idValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!int.TryParse(idValue, out int customerId))
            {
                return Unauthorized();
            }

            var totalCars = await _context.Cars
                .CountAsync(car => car.CustomerId == customerId);

            var activeBookings = await _context.Bookings
                .CountAsync(booking =>
                    booking.CustomerId == customerId &&
                    booking.Status != "Completed"
                );

            var completedBookings = await _context.Bookings
                .CountAsync(booking =>
                    booking.CustomerId == customerId &&
                    booking.Status == "Completed"
                );

            var upcomingBooking = await (
                from booking in _context.Bookings

                join car in _context.Cars
                    on booking.CarId equals car.Id

                join service in _context.Services
                    on booking.ServiceId equals service.Id

                where
                    booking.CustomerId == customerId &&
                    booking.Status != "Completed"

                orderby booking.Date, booking.Time

                select new
                {
                    bookingId = booking.Id,
                    vehicle = car.Brand + " " + car.Model,
                    plateNumber = car.PlateNumber,
                    serviceName = service.Name,
                    date = booking.Date,
                    time = booking.Time,
                    status = booking.Status
                }

            ).FirstOrDefaultAsync();

            var customer = await _context.Customers
                .Where(c => c.Id == customerId)
                .Select(c => new
                {
                    id = c.Id,
                    fullName = c.FullName,
                    email = c.Email,
                    phone = c.Phone
                })
                .FirstOrDefaultAsync();

            return Ok(new
            {
                customer,
                totalCars,
                activeBookings,
                completedBookings,
                upcomingBooking
            });
        }

    }
}