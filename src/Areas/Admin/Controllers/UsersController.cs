using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ctf_sandbox.Services;

namespace ctf_sandbox.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly IRoleService _roleService;

    public UsersController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _roleService.GetAllUsers();
        var roles = await _roleService.GetAllRoles();
        var userRoles = await _roleService.GetUserRoles();

        ViewBag.Roles = roles;
        ViewBag.UserRoles = userRoles;
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> AddRole(string userId, string roleId)
    {
        var result = await _roleService.AddUserToRole(userId, roleId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveRole(string userId, string roleId)
    {
        var result = await _roleService.RemoveUserFromRole(userId, roleId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        if (!string.IsNullOrWhiteSpace(roleName))
        {
            await _roleService.CreateRole(roleName);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        await _roleService.DeleteRole(roleId);
        return RedirectToAction(nameof(Index));
    }
}