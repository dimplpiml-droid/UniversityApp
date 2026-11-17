using System;
using System.Collections.Generic;

namespace UniversityApp.Models;

public partial class AppUser
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public int? TabNumber { get; set; }

    public int? StudentRegNumber { get; set; }

    public virtual Student? StudentRegNumberNavigation { get; set; }

    public virtual Employee? TabNumberNavigation { get; set; }
}
