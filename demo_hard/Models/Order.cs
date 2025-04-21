using System;
using System.Collections.Generic;

namespace demo_hard.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string? OrderCode { get; set; }

    public TimeOnly? Time { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? RentTime { get; set; }

    public DateOnly? StartDate { get; set; }

    public int? ClientId { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
