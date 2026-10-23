using System.Diagnostics;
using EmployeeManagement.Web.Data;
using EmployeeManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var employees = await _db.Employees
            .Include(e => e.Department)
            .AsNoTracking()
            .ToListAsync();

        var summaries = employees
            .GroupBy(e => e.Department?.Name ?? "Unassigned")
            .Select(g => new DepartmentSummary
            {
                Name = g.Key,
                EmployeeCount = g.Count(),
                TotalMonthlySalary = g.Sum(e => e.MonthlySalary)
            })
            .OrderBy(s => s.Name)
            .ToList();

        var model = new DashboardViewModel
        {
            TotalEmployees = employees.Count,
            TotalDepartments = await _db.Departments.CountAsync(),
            AverageMonthlySalary = employees.Count == 0 ? 0 : employees.Average(e => e.MonthlySalary),
            Departments = summaries,
            RecentHires = employees.OrderByDescending(e => e.JoinedOn).Take(3).ToList()
        };

        return View(model);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}