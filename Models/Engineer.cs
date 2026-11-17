using System;
using System.Collections.Generic;

namespace UniversityApp.Models;

public partial class Engineer
{
    public int TabNumber { get; set; }

    public string Profession { get; set; } = null!;

    public virtual Employee TabNumberNavigation { get; set; } = null!;
}
