using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EmployeeManagement.Web.Controllers;
using EmployeeManagement.Web.Data;
using EmployeeManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmployeeManagement.Tests;

public class EmployeesControllerTests
{
    private static AppDbContext NewDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Index_ReturnsEmployeesSortedByName()
    {
        using var db = NewDb();
        var dept = new Department { Name = "IT", Location = "Block A" };
        db.Employees.AddRange(
            new Employee { FullName = "Zara", Email = "z@test.com", Department = dept },
            new Employee { FullName = "Adam", Email = "a@test.com", Department = dept });
        db.SaveChanges();

        var controller = new EmployeesController(db).WithTempData();
        var result = await controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EmployeeListViewModel>(view.Model);
        Assert.Equal(2, model.Employees.Count);
        Assert.Equal("Adam", model.Employees[0].FullName);
    }

    [Fact]
    public async Task Create_ValidEmployee_SavesAndRedirects()
    {
        using var db = NewDb();
        var controller = new EmployeesController(db).WithTempData();
        var emp = new Employee { FullName = "Nina", Email = "nina@test.com", DepartmentId = 1, MonthlySalary = 5000 };

        var result = await controller.Create(emp);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(1, db.Employees.Count());
    }

    [Fact]
    public async Task Create_InvalidModel_ReturnsViewAndDoesNotSave()
    {
        using var db = NewDb();
        var controller = new EmployeesController(db).WithTempData();
        controller.ModelState.AddModelError("FullName", "Required");

        var result = await controller.Create(new Employee());

        Assert.IsType<ViewResult>(result);
        Assert.Equal(0, db.Employees.Count());
    }

    [Fact]
    public void Employee_WithBadData_FailsValidation()
    {
        var emp = new Employee { FullName = "", Email = "not-an-email", MonthlySalary = -5 };
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(emp, new ValidationContext(emp), results, true);

        Assert.Contains(results, r => r.MemberNames.Contains("FullName"));
        Assert.Contains(results, r => r.MemberNames.Contains("Email"));
        Assert.Contains(results, r => r.MemberNames.Contains("MonthlySalary"));
    }

    [Fact]
    public async Task Edit_Get_UnknownId_ReturnsNotFound()
    {
        using var db = NewDb();
        var controller = new EmployeesController(db).WithTempData();

        var result = await controller.Edit(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ValidData_UpdatesEmployee()
    {
        using var db = NewDb();
        var emp = new Employee { FullName = "Old Name", Email = "old@test.com", DepartmentId = 1, MonthlySalary = 1000 };
        db.Employees.Add(emp);
        db.SaveChanges();
        var controller = new EmployeesController(db).WithTempData();

        var updated = new Employee
        {
            Id = emp.Id,
            FullName = "New Name",
            Email = "new@test.com",
            DepartmentId = 2,
            MonthlySalary = 2000
        };
        var result = await controller.Edit(emp.Id, updated);

        Assert.IsType<RedirectToActionResult>(result);
        var saved = db.Employees.Find(emp.Id)!;
        Assert.Equal("New Name", saved.FullName);
        Assert.Equal(2000, saved.MonthlySalary);
    }

    [Fact]
    public async Task Delete_Post_RemovesEmployee()
    {
        using var db = NewDb();
        var emp = new Employee { FullName = "Temp", Email = "temp@test.com", DepartmentId = 1, MonthlySalary = 1000 };
        db.Employees.Add(emp);
        db.SaveChanges();
        var controller = new EmployeesController(db).WithTempData();

        var result = await controller.DeleteConfirmed(emp.Id);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(0, db.Employees.Count());
    }

    [Fact]
    public async Task Delete_Get_UnknownId_ReturnsNotFound()
    {
        using var db = NewDb();
        var controller = new EmployeesController(db).WithTempData();

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }
}