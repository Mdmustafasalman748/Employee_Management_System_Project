using System.Collections.Generic;

namespace EmployeeManagement.Web.Models;

public class DashboardViewModel
{
    public int TotalEmployees { get; set; }
    public int TotalDepartments { get; set; }
    public decimal AverageMonthlySalary { get; set; }
    public List<DepartmentSummary> Departments { get; set; } = new();
    public List<Employee> RecentHires { get; set; } = new();
}

public class DepartmentSummary
{
    public string Name { get; set; } = "";
    public int EmployeeCount { get; set; }
    public decimal TotalMonthlySalary { get; set; }
}