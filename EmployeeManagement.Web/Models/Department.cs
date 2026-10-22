using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Web.Models;

public class Department
{
    public int Id { get; set; }                    // Primary key

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    [Required, StringLength(100)]
    public string Location { get; set; } = "";

    public List<Employee> Employees { get; set; } = new();
}