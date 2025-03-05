using System;
using System.Collections.Generic;

namespace demo_hard.Models;

public partial class Order
{
    public int Id { get; set; }

    public string OrderCode { get; set; } = null!;

    public DateOnly OrderDate { get; set; }

    public TimeOnly OrderTime { get; set; }

    public int ClientCode { get; set; }

    public int ServiceId { get; set; }

    public string Status { get; set; } = null!;

    public DateOnly? DateClose { get; set; }

    public int RentalTime { get; set; }
}
