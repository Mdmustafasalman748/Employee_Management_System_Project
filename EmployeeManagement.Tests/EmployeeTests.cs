using EmployeeManagement.Web.Models;
using Xunit;

namespace EmployeeManagement.Tests;

public class EmployeeTests
{
    [Fact]
    public void AnnualSalary_IsTwelveTimesMonthlySalary()
    {
        var emp = new Employee { FullName = "Jim", MonthlySalary = 5000 };
        Assert.Equal(60000, emp.AnnualSalary);
    }
}