using System;
using System.Collections.Generic;

namespace CarService.API.Models;

public partial class ServiceJob
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int? TechnicianId { get; set; }

    public string Status { get; set; } = null!;

    public string? InspectionNotes { get; set; }

    public decimal? FinalCost { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual Technician? Technician { get; set; }
}
