using System.Collections.Generic;

namespace TaskNest.Infrastructure.Persistence
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Seed Roles
            var roles = config.GetSection("Roles").Get<string[]>() ?? [];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seed Admin Users
            var adminSection = config.GetSection("AdminUsers");
            var adminEmails = adminSection.GetSection("Emails").Get<string[]>() ?? [];
            var adminPassword = adminSection["Password"] ?? "Admin@123_";

            foreach (var email in adminEmails)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    var newUser = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(newUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newUser, RoleEnum.Admin.ToString());

                    }
                    else
                    {
                        throw new Exception($"Failed to create admin user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");

                    }
                }
            }
        }
    }
}
