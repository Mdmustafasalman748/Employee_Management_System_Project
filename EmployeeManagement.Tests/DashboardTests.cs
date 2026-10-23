using EmployeeManagement.Web.Controllers;
using EmployeeManagement.Web.Data;
using EmployeeManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Xunit;

namespace EmployeeManagement.Tests;

public class DashboardTests
{
    private static AppDbContext NewDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Index_SummarisesEmployeesPerDepartment()
    {
        using var db = NewDb();
        var ops = new Department { Name = "Ops", Location = "Block F" };
        db.Employees.AddRange(
            new Employee { FullName = "A", Email = "a@test.com", Department = ops, MonthlySalary = 1000, JoinedOn = new DateTime(2025, 1, 1) },
            new Employee { FullName = "B", Email = "b@test.com", Department = ops, MonthlySalary = 3000, JoinedOn = new DateTime(2025, 6, 1) });
        db.SaveChanges();
        var controller = new HomeController(db);

        var result = await controller.Index();

        var model = Assert.IsType<DashboardViewModel>(Assert.IsType<ViewResult>(result).Model);
        Assert.Equal(2, model.TotalEmployees);
        Assert.Equal(2000, model.AverageMonthlySalary);
        var summary = Assert.Single(model.Departments);
        Assert.Equal("Ops", summary.Name);
        Assert.Equal(2, summary.EmployeeCount);
        Assert.Equal(4000, summary.TotalMonthlySalary);
        Assert.Equal("B", model.RecentHires[0].FullName);   // most recent first
    }

    [Fact]
    public async Task Index_NoEmployees_ReturnsZerosWithoutCrashing()
    {
        using var db = NewDb();
        var controller = new HomeController(db);

        var result = await controller.Index();

        var model = Assert.IsType<DashboardViewModel>(Assert.IsType<ViewResult>(result).Model);
        Assert.Equal(0, model.TotalEmployees);
        Assert.Equal(0, model.AverageMonthlySalary);
        Assert.Empty(model.Departments);
    }
}