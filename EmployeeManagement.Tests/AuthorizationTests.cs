using System.Reflection;
using EmployeeManagement.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace EmployeeManagement.Tests;

public class AuthorizationTests
{
    [Theory]
    [InlineData(typeof(EmployeesController))]
    [InlineData(typeof(DepartmentsController))]
    public void ManagementControllers_RequireLogin(Type controller)
    {
        Assert.NotNull(controller.GetCustomAttribute<AuthorizeAttribute>());
    }

    [Theory]
    [InlineData(typeof(EmployeesController))]
    [InlineData(typeof(DepartmentsController))]
    public void ListPages_ArePublic(Type controller)
    {
        var index = controller.GetMethod("Index")!;
        Assert.NotNull(index.GetCustomAttribute<AllowAnonymousAttribute>());
    }
}