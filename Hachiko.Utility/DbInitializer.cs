using Hachiko.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Hachiko.Utility
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { SD.Role_Customer, SD.Role_Admin };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            // Seed Admin User
            var adminEmail = "admin@hachiko.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "Admin User",
                    EmailConfirmed = true,
                    StreetAddress = "123 Street",
                    City = "Gotham City",
                    State = "New Jersey State",
                    PostalCode = "12345"
                };

                var result = await userManager.CreateAsync(adminUser, "Demo123@");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, SD.Role_Admin);
                }
            }

            // Seed Customer User
            var customerEmail = "customer@hachiko.com";
            var customerUser = await userManager.FindByEmailAsync(customerEmail);

            if (customerUser == null)
            {
                customerUser = new ApplicationUser
                {
                    UserName = customerEmail,
                    Email = customerEmail,
                    Name = "Customer User",
                    EmailConfirmed = true,
                    StreetAddress = "123 Street",
                    City = "Gotham City",
                    State = "New Jersey State",
                    PostalCode = "12345"
                };

                var result = await userManager.CreateAsync(customerUser, "Demo123@");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customerUser, SD.Role_Customer);
                }
            }
        }
    }
}