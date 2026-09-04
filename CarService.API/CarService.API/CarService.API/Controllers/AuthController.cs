using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using CarService.API.Data;
using CarService.API.DTOs;
using CarService.API.Models;

namespace CarService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CarServiceDbContext _context;

        public AuthController(CarServiceDbContext context)
        {
            _context = context;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == request.Email);

            if (admin == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }
            if (!admin.IsActive)
            {
                return Unauthorized(new
                {
                    message = "This account is inactive."
                });
            }

            var passwordHasher = new PasswordHasher<Admin>();

            var result = passwordHasher.VerifyHashedPassword(
                admin,
                admin.PasswordHash,
                request.Password
            );

            if (result == PasswordVerificationResult.Failed)
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
                    admin.Id.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    admin.FullName
                ),

                new Claim(
                    ClaimTypes.Email,
                    admin.Email
                ),

                new Claim(
                    ClaimTypes.Role,
                    admin.Role
                )
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            var properties = new AuthenticationProperties
            {
                IsPersistent = true,

                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                properties
            );

            return Ok(new
            {
                message = "Login successful.",

                admin = new
                {
                    id = admin.Id,

                    fullName = admin.FullName,

                    email = admin.Email,

                    role = admin.Role
                }
            });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentAdmin()
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
        [Authorize]
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
    }
}