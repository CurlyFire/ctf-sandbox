using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace ctf_sandbox.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        // Seed roles
        string[] roles = { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed admin user
        var adminEmail = configuration["AdminAccount:Email"];
        var adminPassword = configuration["AdminAccount:Password"];

        if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
        {
            throw new InvalidOperationException("Admin account configuration is missing in appsettings.json");
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }        
    }
}