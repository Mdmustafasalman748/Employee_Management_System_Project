using EmployeeManagement.Web.Models;
using Xunit;
using EmployeeManagement.Web.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace EmployeeManagement.Tests;

public class EmployeeTests
{
    [Fact]
    public void CanSaveEmployeeWithDepartment()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new AppDbContext(options);
        var dept = new Department { Name = "Finance", Location = "Block C" };
        db.Employees.Add(new Employee { FullName = "Jim", Email = "jim@test.com", Department = dept });
        db.SaveChanges();

        Assert.Equal(1, db.Employees.Count());
        Assert.Equal("Finance", db.Employees.Include(e => e.Department).First().Department!.Name);
    }
}