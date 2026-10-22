using System.Collections.Generic;

namespace EmployeeManagement.Web.Models;

public class EmployeeListViewModel
{
    public List<Employee> Employees { get; set; } = new();
    public string? Search { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }

    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}