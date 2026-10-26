using System;
using System.Reflection;
using EmployeeManagement.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace EmployeeManagement.Tests;

public class RoleTests
{
    [Theory]
    [InlineData(typeof(EmployeesController), "Delete")]
    [InlineData(typeof(EmployeesController), "DeleteConfirmed")]
    [InlineData(typeof(DepartmentsController), "Delete")]
    [InlineData(typeof(DepartmentsController), "DeleteConfirmed")]
    public void DeleteActions_RequireAdminRole(Type controller, string action)
    {
        var method = controller.GetMethod(action)!;
        var attribute = method.GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("Admin", attribute!.Roles);
    }

    [Fact]
    public void UsersController_IsAdminOnly()
    {
        var attribute = typeof(UsersController).GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("Admin", attribute!.Roles);
    }
}