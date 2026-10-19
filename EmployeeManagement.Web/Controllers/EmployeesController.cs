using EmployeeManagement.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EmployeeManagement.Web.Controllers;

public class EmployeesController : Controller
{
    private readonly AppDbContext _db;

    public EmployeesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var employees = await _db.Employees
            .Include(e => e.Department)
            .OrderBy(e => e.FullName)
            .ToListAsync();

        return View(employees);
    }
}