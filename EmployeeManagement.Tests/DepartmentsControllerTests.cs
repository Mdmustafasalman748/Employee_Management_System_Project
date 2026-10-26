using EmployeeManagement.Web.Data;
using EmployeeManagement.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EmployeeManagement.Web.Controllers;

[Authorize]
public class DepartmentsController : Controller
{
    private readonly AppDbContext _db;

    public DepartmentsController(AppDbContext db)
    {
        _db = db;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var departments = await _db.Departments
            .Include(d => d.Employees)
            .OrderBy(d => d.Name)
            .ToListAsync();
        return View(departments);
    }

    public IActionResult Create() => View(new Department());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Location")] Department department)
    {
        if (!ModelState.IsValid) return View(department);

        _db.Departments.Add(department);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Department {department.Name} was added.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var department = await _db.Departments.FindAsync(id);
        if (department == null) return NotFound();
        return View(department);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location")] Department department)
    {
        if (id != department.Id) return NotFound();
        if (!ModelState.IsValid) return View(department);

        var existing = await _db.Departments.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name = department.Name;
        existing.Location = department.Location;
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Department {existing.Name} was updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var department = await _db.Departments
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id);
        if (department == null) return NotFound();
        return View(department);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var department = await _db.Departments
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id);
        if (department == null) return RedirectToAction(nameof(Index));

        if (department.Employees.Any())
        {
            ModelState.AddModelError("", "This department still has employees. Move or delete them first.");
            return View("Delete", department);
        }

        _db.Departments.Remove(department);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Department {department.Name} was deleted.";
        return RedirectToAction(nameof(Index));
    }
}