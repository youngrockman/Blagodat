using System;
using System.Collections.Generic;

namespace demo_hard.Model;

public partial class Service
{
    public string? ServiceName { get; set; }

    public int? CostPerHour { get; set; }

    public string ServiceCode { get; set; } = null!;

    public int ServiceId { get; set; }

    public virtual ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();
}
