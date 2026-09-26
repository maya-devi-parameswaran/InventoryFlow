using InventoryFlow.Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace InventoryFlow.Infrastructure.Identity;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        string[] roles = [Roles.Admin, Roles.WarehouseStaff, Roles.Customer];

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole(roleName));
            }
        }
    }
}