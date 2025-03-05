using System;
using System.Collections.Generic;

namespace demo_hard.Models;

public partial class Employee
{
    public int EmployeId { get; set; }

    public int RoleId { get; set; }

    public string Fio { get; set; } = null!;

    public string EmployeLogin { get; set; } = null!;

    public string EmployePassword { get; set; } = null!;

    public string? EmployePhoto { get; set; }
}
