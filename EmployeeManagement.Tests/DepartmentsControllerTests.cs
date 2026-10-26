using EmployeeManagement.Web.Controllers;
using EmployeeManagement.Web.Data;
using EmployeeManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Xunit;

namespace EmployeeManagement.Tests;

public class DepartmentsControllerTests
{
    private static AppDbContext NewDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Create_ValidDepartment_SavesAndRedirects()
    {
        using var db = NewDb();
        var controller = new DepartmentsController(db).WithTempData();

        var result = await controller.Create(new Department { Name = "Legal", Location = "Block D" });

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(1, db.Departments.Count(d => d.Name == "Legal"));
    }

    [Fact]
    public async Task Edit_Post_ValidData_UpdatesDepartment()
    {
        using var db = NewDb();
        var dept = new Department { Name = "Old", Location = "Old Block" };
        db.Departments.Add(dept);
        db.SaveChanges();
        var controller = new DepartmentsController(db).WithTempData();

        var result = await controller.Edit(dept.Id, new Department { Id = dept.Id, Name = "New", Location = "New Block" });

        Assert.IsType<RedirectToActionResult>(result);
        var saved = db.Departments.Find(dept.Id)!;
        Assert.Equal("New", saved.Name);
        Assert.Equal("New Block", saved.Location);
    }

    [Fact]
    public async Task DeleteConfirmed_EmptyDepartment_IsDeleted()
    {
        using var db = NewDb();
        var dept = new Department { Name = "Temp", Location = "Nowhere" };
        db.Departments.Add(dept);
        db.SaveChanges();
        var controller = new DepartmentsController(db).WithTempData();

        var result = await controller.DeleteConfirmed(dept.Id);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(0, db.Departments.Count(d => d.Name == "Temp"));
    }

    [Fact]
    public async Task DeleteConfirmed_DepartmentWithEmployees_IsNotDeleted()
    {
        using var db = NewDb();
        var dept = new Department { Name = "Busy", Location = "Block E" };
        db.Employees.Add(new Employee { FullName = "Sam", Email = "sam@test.com", Department = dept, MonthlySalary = 1000 });
        db.SaveChanges();
        var controller = new DepartmentsController(db).WithTempData();

        var result = await controller.DeleteConfirmed(dept.Id);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal("Delete", view.ViewName);
        Assert.False(controller.ModelState.IsValid);
        Assert.Equal(1, db.Departments.Count(d => d.Name == "Busy"));
    }
}