using System;
using System.Collections.Generic;

namespace CarService.API.Models;

public partial class Invoice
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int ServiceJobId { get; set; }

    public string? Items { get; set; }

    public decimal Total { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;

    public virtual ServiceJob ServiceJob { get; set; } = null!;
}
