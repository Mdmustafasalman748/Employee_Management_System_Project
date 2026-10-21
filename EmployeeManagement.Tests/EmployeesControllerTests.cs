using EmployeeManagement.Web.Controllers;
using EmployeeManagement.Web.Data;
using EmployeeManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Xunit;

namespace EmployeeManagement.Tests;

public class EmployeesControllerTests
{
    [Fact]
    public async Task Index_ReturnsEmployeesSortedByName()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new AppDbContext(options);
        var dept = new Department { Name = "IT", Location = "Block A" };
        db.Employees.AddRange(
            new Employee { FullName = "Zara", Email = "z@test.com", Department = dept },
            new Employee { FullName = "Adam", Email = "a@test.com", Department = dept });
        db.SaveChanges();

        var controller = new EmployeesController(db);
        var result = await controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Employee>>(view.Model);
        Assert.Equal(2, model.Count);
        Assert.Equal("Adam", model[0].FullName);
    }
}   