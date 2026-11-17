using System;
using System.Collections.Generic;

namespace UniversityApp.Models;

public partial class DepartmentHead
{
    public int TabNumber { get; set; }

    public int Experience { get; set; }

    public virtual Employee TabNumberNavigation { get; set; } = null!;
}
