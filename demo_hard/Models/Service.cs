using System.Collections.Generic;

namespace demo_hard.Models;

public partial class Service
{
    public string? ServiceName { get; set; }
    public int? CostPerHour { get; set; }
    public string ServiceCode { get; set; } = null!;
    public int ServiceId { get; set; }
    
    public virtual ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}