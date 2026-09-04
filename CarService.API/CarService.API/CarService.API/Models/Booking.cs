using System;
using System.Collections.Generic;

namespace CarService.API.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int CarId { get; set; }

    public int ServiceId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public string Status { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public virtual Car Car { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual Service Service { get; set; } = null!;

    public virtual ICollection<ServiceJob> ServiceJobs { get; set; } = new List<ServiceJob>();
}
