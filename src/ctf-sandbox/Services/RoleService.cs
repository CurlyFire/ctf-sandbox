using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ctf_sandbox.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public RoleService(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<List<IdentityRole>> GetAllRoles()
    {
        return await _roleManager.Roles.ToListAsync();
    }

    public async Task<List<IdentityUser>> GetUsersInRole(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null) return new List<IdentityUser>();

        var users = await _userManager.GetUsersInRoleAsync(role.Name);
        return users.ToList();
    }

    public async Task<bool> AddUserToRole(string userId, string roleId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var role = await _roleManager.FindByIdAsync(roleId);
        
        if (user == null || role == null) return false;
        
        if (!await _userManager.IsInRoleAsync(user, role.Name))
        {
            var result = await _userManager.AddToRoleAsync(user, role.Name);
            return result.Succeeded;
        }
        
        return true;
    }

    public async Task<bool> RemoveUserFromRole(string userId, string roleId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var role = await _roleManager.FindByIdAsync(roleId);
        
        if (user == null || role == null) return false;
        
        if (await _userManager.IsInRoleAsync(user, role.Name))
        {
            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
            return result.Succeeded;
        }
        
        return true;
    }

    public async Task<bool> CreateRole(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            return result.Succeeded;
        }
        return false;
    }

    public async Task<bool> DeleteRole(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null) return false;

        var result = await _roleManager.DeleteAsync(role);
        return result.Succeeded;
    }

    public async Task<List<IdentityUser>> GetAllUsers()
    {
        return await _userManager.Users.ToListAsync();
    }

    public async Task<Dictionary<string, List<string>>> GetUserRoles()
    {
        var users = await GetAllUsers();
        var userRoles = new Dictionary<string, List<string>>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles[user.Id] = roles.ToList();
        }

        return userRoles;
    }
}