using System;
using System.Collections.Generic;

namespace UniversityApp.Models;

public partial class Employee
{
    public int TabNumber { get; set; }

    public string DeptCode { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string JobTitle { get; set; } = null!;

    public decimal Salary { get; set; }

    public int? BossTabNumber { get; set; }

    public virtual ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();

    public virtual Employee? BossTabNumberNavigation { get; set; }

    public virtual DepartmentHead? DepartmentHead { get; set; }

    public virtual Department DeptCodeNavigation { get; set; } = null!;

    public virtual Engineer? Engineer { get; set; }

    public virtual ICollection<Employee> InverseBossTabNumberNavigation { get; set; } = new List<Employee>();

    public virtual Teacher? Teacher { get; set; }
}
