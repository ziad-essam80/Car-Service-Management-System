using System;
using System.Collections.Generic;

namespace CarService.API.Models;

public partial class Technician
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Specialization { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<ServiceJob> ServiceJobs { get; set; } = new List<ServiceJob>();
}
