using CarService.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarService.API.Controllers
{
    [ApiController]
    [Route("api/public-chat")]
    public class PublicChatController : ControllerBase
    {
        private readonly CarServiceDbContext _context;


        public PublicChatController(CarServiceDbContext context)
        {
            _context = context;
        }


        // =========================================================
        // CHAT
        // =========================================================

        [HttpPost]
        public async Task<IActionResult> Chat(
            [FromBody] ChatRequest request)
        {
            // ==========================================
            // VALIDATION
            // ==========================================

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new
                {
                    reply = "Please enter a question."
                });
            }


            string message = request.Message
                .Trim()
                .ToLower();


            // ==========================================
            // BOOKING
            // ==========================================

            if (
                message.Contains("book") ||
                message.Contains("booking") ||
                message.Contains("appointment") ||
                message.Contains("reserve")
            )
            {
                return Ok(new
                {
                    reply =
                        "To book a service, click Get Started, create an account, add your car, select a service and choose an available appointment."
                });
            }


            // ==========================================
            // LOGIN
            // ==========================================

            if (
                message.Contains("login") ||
                message.Contains("log in") ||
                message.Contains("sign in")
            )
            {
                return Ok(new
                {
                    reply =
                        "You can log in by clicking the Login button at the top of the website."
                });
            }


            // ==========================================
            // REGISTER
            // ==========================================

            if (
                message.Contains("register") ||
                message.Contains("sign up") ||
                message.Contains("create account") ||
                message.Contains("new account")
            )
            {
                return Ok(new
                {
                    reply =
                        "Click Get Started to create a new account."
                });
            }


            // ==========================================
            // GET ACTIVE SERVICES FROM DATABASE
            // ==========================================

            var services = await _context.Services

                .Where(s => s.IsActive)

                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description,
                    s.Price,
                    s.EstimatedTime,
                    s.Status,
                    s.ImageUrl,
                    s.IsActive
                })

                .ToListAsync();


            // ==========================================
            // FIND A SPECIFIC SERVICE IN USER QUESTION
            // ==========================================

            var selectedService = services
                .FirstOrDefault(s =>
                    IsServiceMentioned(
                        message,
                        s.Name
                    )
                );


            // ==========================================
            // SPECIFIC SERVICE
            // ==========================================

            if (selectedService != null)
            {
                // ======================================
                // SERVICE PRICE
                // ======================================

                if (
                    message.Contains("price") ||
                    message.Contains("cost") ||
                    message.Contains("how much")
                )
                {
                    return Ok(new
                    {
                        reply =
                            $"{selectedService.Name} costs {selectedService.Price} EGP.",

                        type = "service",

                        data = selectedService
                    });
                }


                // ======================================
                // SERVICE DURATION
                // ======================================

                if (
                    message.Contains("time") ||
                    message.Contains("how long") ||
                    message.Contains("duration") ||
                    message.Contains("minutes") ||
                    message.Contains("take")
                )
                {
                    return Ok(new
                    {
                        reply =
                            $"{selectedService.Name} takes approximately {selectedService.EstimatedTime} minutes.",

                        type = "service",

                        data = selectedService
                    });
                }


                // ======================================
                // SERVICE DESCRIPTION
                // ======================================

                if (
                    message.Contains("what is") ||
                    message.Contains("about") ||
                    message.Contains("description") ||
                    message.Contains("include") ||
                    message.Contains("includes") ||
                    message.Contains("included")
                )
                {
                    if (
                        string.IsNullOrWhiteSpace(
                            selectedService.Description
                        )
                    )
                    {
                        return Ok(new
                        {
                            reply =
                                $"I don't have a description for {selectedService.Name} at the moment.",

                            type = "service",

                            data = selectedService
                        });
                    }


                    return Ok(new
                    {
                        reply =
                            selectedService.Description,

                        type = "service",

                        data = selectedService
                    });
                }


                // ======================================
                // SERVICE AVAILABILITY
                // ======================================

                if (
                    message.Contains("available") ||
                    message.Contains("availability") ||
                    message.Contains("do you provide") ||
                    message.Contains("do you offer") ||
                    message.Contains("have")
                )
                {
                    return Ok(new
                    {
                        reply =
                            $"Yes, {selectedService.Name} is currently available.",

                        type = "service",

                        data = selectedService
                    });
                }


                // ======================================
                // GENERAL SPECIFIC SERVICE RESPONSE
                // ======================================

                return Ok(new
                {
                    reply =
                        $"{selectedService.Name} is available for {selectedService.Price} EGP and takes approximately {selectedService.EstimatedTime} minutes.",

                    type = "service",

                    data = selectedService
                });
            }


            // ==========================================
            // ALL SERVICE PRICES
            // ==========================================

            if (
                message.Contains("prices") ||
                message.Contains("service prices") ||
                message.Contains("how much are your services") ||
                message.Contains("cost of services")
            )
            {
                if (!services.Any())
                {
                    return Ok(new
                    {
                        reply =
                            "There are no services available at the moment."
                    });
                }


                return Ok(new
                {
                    reply =
                        "Here are our current service prices:",

                    type = "services",

                    data = services
                });
            }


            // ==========================================
            // LIST ALL SERVICES
            // ==========================================

            if (
                message.Contains("service") ||
                message.Contains("services") ||
                message.Contains("what services") ||
                message.Contains("what do you provide") ||
                message.Contains("what do you offer") ||
                message.Contains("available services")
            )
            {
                if (!services.Any())
                {
                    return Ok(new
                    {
                        reply =
                            "There are no services available at the moment."
                    });
                }


                return Ok(new
                {
                    reply =
                        "Here are our available services:",

                    type = "services",

                    data = services
                });
            }


            // ==========================================
            // OUT OF SCOPE
            // ==========================================

            return Ok(new
            {
                reply =
                    "Sorry, I can only help with questions related to CarService and our available services."
            });
        }


        // =========================================================
        // CHECK IF USER MENTIONED A SERVICE
        // =========================================================

        private bool IsServiceMentioned(
            string message,
            string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                return false;
            }


            string normalizedServiceName =
                serviceName
                    .Trim()
                    .ToLower();


            // ==========================================
            // EXACT SERVICE NAME
            // ==========================================

            if (
                message.Contains(
                    normalizedServiceName
                )
            )
            {
                return true;
            }


            // ==========================================
            // SPLIT SERVICE NAME INTO WORDS
            // ==========================================

            var serviceWords =
                normalizedServiceName
                    .Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries
                    )
                    .Where(word =>
                        word.Length >= 3 &&
                        !IsIgnoredWord(word)
                    )
                    .ToList();


            // ==========================================
            // CHECK EACH IMPORTANT WORD
            // ==========================================

            foreach (var word in serviceWords)
            {
                if (message.Contains(word))
                {
                    return true;
                }
            }


            return false;
        }


        // =========================================================
        // IGNORE GENERIC SERVICE WORDS
        // =========================================================

        private bool IsIgnoredWord(string word)
        {
            string[] ignoredWords =
            {
                "service",
                "services",
                "repair",
                "maintenance",
                "car",
                "vehicle",
                "system",
                "check"
            };


            return ignoredWords.Contains(
                word.ToLower()
            );
        }
    }


    // =============================================================
    // CHAT REQUEST
    // =============================================================

    public class ChatRequest
    {
        public string Message { get; set; }
            = string.Empty;
    }
}