using System;
using System.Collections.Generic;

namespace demo_hard.Model;

public partial class Employee
{
    public int Id { get; set; }

    public int Role { get; set; }

    public string? Login { get; set; }

    public string? Fio { get; set; }

    public string? Password { get; set; }

    public DateTime? LastEnter { get; set; }

    public string? EnterStatus { get; set; }

    public string? Photo { get; set; }
}
