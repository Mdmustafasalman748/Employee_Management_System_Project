using System;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Web.Controllers;
using EmployeeManagement.Web.Data;
using EmployeeManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmployeeManagement.Tests;

public class EmployeesDetailsExportTests
{
    private static AppDbContext NewDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Details_UnknownId_ReturnsNotFound()
    {
        using var db = NewDb();
        var controller = new EmployeesController(db).WithTempData();

        var result = await controller.Details(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_KnownId_ReturnsEmployeeWithDepartment()
    {
        using var db = NewDb();
        var emp = new Employee
        {
            FullName = "Ann",
            Email = "ann@test.com",
            Department = new Department { Name = "IT", Location = "Block A" }
        };
        db.Employees.Add(emp);
        db.SaveChanges();
        var controller = new EmployeesController(db).WithTempData();

        var result = await controller.Details(emp.Id);

        var model = Assert.IsType<Employee>(Assert.IsType<ViewResult>(result).Model);
        Assert.Equal("Ann", model.FullName);
        Assert.Equal("IT", model.Department!.Name);
    }

    [Fact]
    public async Task ExportCsv_ReturnsCsvFile()
    {
        using var db = NewDb();
        db.Employees.Add(new Employee
        {
            FullName = "Ann",
            Email = "ann@test.com",
            Department = new Department { Name = "IT", Location = "Block A" }
        });
        db.SaveChanges();
        var controller = new EmployeesController(db).WithTempData();

        var result = await controller.ExportCsv();

        var file = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", file.ContentType);
        Assert.Contains("Ann", Encoding.UTF8.GetString(file.FileContents));
    }

    [Fact]
    public async Task ExportCsv_RespectsSearch()
    {
        using var db = NewDb();
        var dept = new Department { Name = "IT", Location = "Block A" };
        db.Employees.AddRange(
            new Employee { FullName = "Alice Smith", Email = "alice@test.com", Department = dept },
            new Employee { FullName = "Bob Jones", Email = "bob@test.com", Department = dept });
        db.SaveChanges();
        var controller = new EmployeesController(db).WithTempData();

        var result = await controller.ExportCsv("Smith");

        var text = Encoding.UTF8.GetString(Assert.IsType<FileContentResult>(result).FileContents);
        Assert.Contains("Alice", text);
        Assert.DoesNotContain("Bob", text);
    }
}