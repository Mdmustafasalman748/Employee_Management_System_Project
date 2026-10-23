using EmployeeManagement.Web.Controllers;
using EmployeeManagement.Web.Data;
using EmployeeManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Xunit;

namespace EmployeeManagement.Tests;

public class EmployeesSearchPagingTests
{
    private static AppDbContext NewDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static Department NewDept() => new Department { Name = "Test", Location = "Test" };

    [Fact]
    public async Task Index_Search_ReturnsOnlyMatchingEmployees()
    {
        using var db = NewDb();
        var dept = NewDept();
        db.Employees.AddRange(
            new Employee { FullName = "Alice Smith", Email = "alice@test.com", Department = dept },
            new Employee { FullName = "Bob Jones", Email = "bob@test.com", Department = dept });
        db.SaveChanges();
        var controller = new EmployeesController(db).WithTempData();

        var result = await controller.Index("Smith", 1);

        var model = Assert.IsType<EmployeeListViewModel>(Assert.IsType<ViewResult>(result).Model);
        Assert.Single(model.Employees);
        Assert.Equal("Alice Smith", model.Employees[0].FullName);
    }

    [Fact]
    public async Task Index_SecondPage_ReturnsRemainingEmployees()
    {
        using var db = NewDb();
        var dept = NewDept();
        for (int i = 1; i <= 7; i++)
            db.Employees.Add(new Employee { FullName = $"Employee {i}", Email = $"e{i}@test.com", Department = dept });
        db.SaveChanges();
        var controller = new EmployeesController(db).WithTempData();

        var result = await controller.Index(null, 2);

        var model = Assert.IsType<EmployeeListViewModel>(Assert.IsType<ViewResult>(result).Model);
        Assert.Equal(2, model.Employees.Count);   // 7 employees, 5 per page
        Assert.Equal(2, model.TotalPages);
        Assert.Equal(2, model.Page);
    }

    [Fact]
    public async Task Index_PageNumberTooHigh_IsClampedToLastPage()
    {
        using var db = NewDb();
        db.Employees.Add(new Employee { FullName = "Only One", Email = "one@test.com", Department = NewDept() });
        db.SaveChanges();
        var controller = new EmployeesController(db).WithTempData();

        var result = await controller.Index(null, 999);

        var model = Assert.IsType<EmployeeListViewModel>(Assert.IsType<ViewResult>(result).Model);
        Assert.Equal(1, model.Page);
        Assert.Single(model.Employees);
    }
}