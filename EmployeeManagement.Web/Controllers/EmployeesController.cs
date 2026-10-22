using EmployeeManagement.Web.Data;
using EmployeeManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

namespace EmployeeManagement.Web.Controllers;

public class EmployeesController : Controller
{
    private readonly AppDbContext _db;

    public EmployeesController(AppDbContext db)
    {
        _db = db;
    }

    private const int PageSize = 5;

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var query = _db.Employees.Include(e => e.Department).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(e => e.FullName.Contains(term) || e.Email.Contains(term));
        }

        var total = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var employees = await query
            .OrderBy(e => e.FullName)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return View(new EmployeeListViewModel
        {
            Employees = employees,
            Search = search,
            Page = page,
            TotalPages = totalPages
        });
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDepartments();
        return View(new Employee { JoinedOn = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("FullName,Email,DepartmentId,MonthlySalary,JoinedOn")] Employee employee)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDepartments(employee.DepartmentId);
            return View(employee);
        }

        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _db.Employees.FindAsync(id);
        if (employee == null) return NotFound();

        await PopulateDepartments(employee.DepartmentId);
        return View(employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,FullName,Email,DepartmentId,MonthlySalary,JoinedOn")] Employee employee)
    {
        if (id != employee.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            await PopulateDepartments(employee.DepartmentId);
            return View(employee);
        }

        var existing = await _db.Employees.FindAsync(id);
        if (existing == null) return NotFound();

        existing.FullName = employee.FullName;
        existing.Email = employee.Email;
        existing.DepartmentId = employee.DepartmentId;
        existing.MonthlySalary = employee.MonthlySalary;
        existing.JoinedOn = employee.JoinedOn;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _db.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null) return NotFound();

        return View(employee);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await _db.Employees.FindAsync(id);
        if (employee != null)
        {
            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDepartments(int? selectedId = null)
    {
        var departments = await _db.Departments.OrderBy(d => d.Name).ToListAsync();
        ViewBag.Departments = new SelectList(departments, "Id", "Name", selectedId);
    }
}