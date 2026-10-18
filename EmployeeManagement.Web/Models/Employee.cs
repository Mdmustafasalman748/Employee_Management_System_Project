using System;

namespace EmployeeManagement.Web.Models;

public class Employee
{
	public int Id { get; set; }
	public string FullName { get; set; } = "";
	public string Email { get; set; } = "";
    public int DepartmentId {get; set; } // Foreign key
	public Department? Department {get;set;}  // Navigation property
	public decimal MonthlySalary { get; set; }
	public DateTime JoinedOn { get; set; }

	public decimal AnnualSalary => MonthlySalary * 12;
}