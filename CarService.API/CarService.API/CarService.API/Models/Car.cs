using System;
using System.Collections.Generic;

namespace CarService.API.Models;

public partial class Car
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int Year { get; set; }

    public string PlateNumber { get; set; } = null!;

    public string? Color { get; set; }

    public int? Mileage { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Customer Customer { get; set; } = null!;
}
