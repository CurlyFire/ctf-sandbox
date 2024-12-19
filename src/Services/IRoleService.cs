using Microsoft.AspNetCore.Identity;

namespace ctf_sandbox.Services;

public interface IRoleService
{
    Task<List<IdentityRole>> GetAllRoles();
    Task<List<IdentityUser>> GetUsersInRole(string roleId);
    Task<bool> AddUserToRole(string userId, string roleId);
    Task<bool> RemoveUserFromRole(string userId, string roleId);
    Task<bool> CreateRole(string roleName);
    Task<bool> DeleteRole(string roleId);
    Task<List<IdentityUser>> GetAllUsers();
    Task<Dictionary<string, List<string>>> GetUserRoles();
}