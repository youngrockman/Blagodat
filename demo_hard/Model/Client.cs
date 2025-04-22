using System;
using System.Collections.Generic;

namespace demo_hard.Model;

public partial class Client
{
    public string? Fio { get; set; }

    public int? ClientCode { get; set; }

    public int ClientId { get; set; }

    public string? Passport { get; set; }

    public DateOnly? Birthday { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public int? Role { get; set; }
}
