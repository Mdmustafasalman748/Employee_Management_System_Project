using System.Collections.Generic;

namespace EmployeeManagement.Web.Models;

public class Department
{
    public int Id { get; set; }   //Primary key
    public string Name { get; set; } = "";
    public string Location { get; set; } = "";

    public List<Employee> Employees { get; set; } = new();
}