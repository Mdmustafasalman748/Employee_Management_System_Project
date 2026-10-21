using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Web.Models;

public class Employee
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = "";

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = "";

    [Display(Name = "Department")]
    public int DepartmentId { get; set; }          // Foreign key
    public Department? Department { get; set; }   // Navigation property

    [Range(1, 1000000, ErrorMessage = "Salary must be between 1 and 1,000,000.")]
    [Display(Name = "Monthly Salary")]
    public decimal MonthlySalary { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Joined On")]
    public DateTime JoinedOn { get; set; }

    // Read-only property: only a getter
    public decimal AnnualSalary => MonthlySalary * 12;
}