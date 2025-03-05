using System;
using System.Collections.Generic;

namespace demo_hard.Models;

public partial class Client
{
    public int Id { get; set; }

    public string Fio { get; set; } = null!;

    public int ClientCode { get; set; }

    public string Passport { get; set; } = null!;

    public DateOnly? Birthday { get; set; }

    public string Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }
}
