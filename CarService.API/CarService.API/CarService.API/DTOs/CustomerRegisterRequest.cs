namespace CarService.API.DTOs
{
    public class CustomerRegisterRequest
    {
        public string FullName { get; set; } = "";

        public string Email { get; set; } = "";

        public string Phone { get; set; } = "";

        public string Password { get; set; } = "";

        public string ConfirmPassword { get; set; } = "";
    }

    public class CustomerLoginRequest
    {
        public string Email { get; set; } = "";

        public string Password { get; set; } = "";
    }


    public class UpdateCustomerProfileRequest
    {
        public string FullName { get; set; } = "";

        public string Email { get; set; } = "";

        public string Phone { get; set; } = "";
    }

    public class AddCarRequest
    {
        public string Brand { get; set; } = "";

        public string Model { get; set; } = "";

        public int Year { get; set; }

        public string PlateNumber { get; set; } = "";

        public string? Color { get; set; }

        public int Mileage { get; set; }
    }


    public class UpdateCarRequest
    {
        public string Brand { get; set; } = "";

        public string Model { get; set; } = "";

        public int Year { get; set; }

        public string PlateNumber { get; set; } = "";

        public string? Color { get; set; }

        public int Mileage { get; set; }
    }

    public class CreateBookingRequest
    {
        public int CarId { get; set; }

        public int ServiceId { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly Time { get; set; }
    }

}

namespace CarService.API.DTOs
{
    public class ServiceResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int EstimatedTime { get; set; }

        public string? Status { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; }
    }
}

namespace CarService.API.DTOs
{
    public class ChatRequest
    {
        public string Message { get; set; } = "";
    }
}
