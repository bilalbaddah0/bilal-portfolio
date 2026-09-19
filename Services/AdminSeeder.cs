using Microsoft.AspNetCore.Identity;

namespace BilalPortfolio.Services;

public sealed class AdminSeeder(
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration,
    ILogger<AdminSeeder> logger)
{
    public const string AdminRole = "Admin";

    public async Task SeedAsync()
    {
        var email = configuration["Admin:Email"];
        var password = configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogInformation("Admin credentials are not configured; admin seeding was skipped.");
            return;
        }

        if (!await roleManager.RoleExistsAsync(AdminRole))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(AdminRole));
            EnsureSucceeded(result, "create the admin role");
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, password);
            EnsureSucceeded(result, "create the admin user");
        }

        if (!await userManager.IsInRoleAsync(user, AdminRole))
        {
            var result = await userManager.AddToRoleAsync(user, AdminRole);
            EnsureSucceeded(result, "assign the admin role");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string operation)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Unable to {operation}: {string.Join(", ", result.Errors.Select(error => error.Description))}");
        }
    }
}
