using EmployeeManagement.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _users;

    public UsersController(UserManager<IdentityUser> users)
    {
        _users = users;
    }

    public async Task<IActionResult> Index()
    {
        var all = await _users.Users.OrderBy(u => u.Email).ToListAsync();
        var model = new List<UserRoleViewModel>();

        foreach (var u in all)
        {
            model.Add(new UserRoleViewModel
            {
                Id = u.Id,
                Email = u.Email ?? "",
                IsAdmin = await _users.IsInRoleAsync(u, "Admin")
            });
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAdmin(string id)
    {
        var user = await _users.FindByIdAsync(id);
        if (user == null) return NotFound();

        if (user.UserName == User.Identity?.Name)
        {
            TempData["Error"] = "You can't change your own role.";
            return RedirectToAction(nameof(Index));
        }

        if (await _users.IsInRoleAsync(user, "Admin"))
        {
            await _users.RemoveFromRoleAsync(user, "Admin");
            TempData["Success"] = $"{user.Email} is no longer an Admin.";
        }
        else
        {
            await _users.AddToRoleAsync(user, "Admin");
            TempData["Success"] = $"{user.Email} is now an Admin.";
        }
        return RedirectToAction(nameof(Index));
    }
}