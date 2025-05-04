using LabTracker.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LabTracker.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        foreach (var role in Enum.GetNames(typeof(Role)))
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }

    // public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
    // {
    //     var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
    //     var config = serviceProvider.GetRequiredService<IConfiguration>();
    //
    //     var email = config["Seed:AdminEmail"] ?? "admin@example.com";
    //     var password = config["Seed:AdminPassword"] ?? "Admin123!";
    //
    //     var user = await userManager.FindByEmailAsync(email);
    //     if (user == null)
    //     {
    //         user = new User { UserName = email, Email = email };
    //         var result = await userManager.CreateAsync(user, password);
    //         if (!result.Succeeded) throw new Exception("Failed to create admin user");
    //     }
    //
    //     if (!await userManager.IsInRoleAsync(user, nameof(Role.Administrator)))
    //     {
    //         await userManager.AddToRoleAsync(user, nameof(Role.Administrator));
    //     }
    // }
}