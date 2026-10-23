using EmployeeManagement.Web.Controllers;
using EmployeeManagement.Web.Data;
using EmployeeManagement.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Xunit;

namespace EmployeeManagement.Tests;

public class FlashMessageTests
{
    private static AppDbContext NewDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateEmployee_SetsSuccessMessage()
    {
        using var db = NewDb();
        var controller = new EmployeesController(db).WithTempData();

        await controller.Create(new Employee { FullName = "Nina", Email = "nina@test.com", DepartmentId = 1, MonthlySalary = 5000 });

        var message = Assert.IsType<string>(controller.TempData["Success"]);
        Assert.Contains("Nina", message);
    }

    [Fact]
    public async Task DeleteDepartmentWithEmployees_DoesNotSetSuccessMessage()
    {
        using var db = NewDb();
        var dept = new Department { Name = "Busy", Location = "Block E" };
        db.Employees.Add(new Employee { FullName = "Sam", Email = "sam@test.com", Department = dept, MonthlySalary = 1000 });
        db.SaveChanges();
        var controller = new DepartmentsController(db).WithTempData();

        await controller.DeleteConfirmed(dept.Id);

        Assert.False(controller.TempData.ContainsKey("Success"));
    }
}