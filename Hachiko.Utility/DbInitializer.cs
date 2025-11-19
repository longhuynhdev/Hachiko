using Hachiko.Models;
using Microsoft.AspNetCore.Identity;

namespace Hachiko.Utility
{
    public class DbInitializer : IDbInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DbInitializer(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task InitializeAsync()
        {
            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            string[] roles = { SD.Role_Customer, SD.Role_Admin };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private async Task SeedUsersAsync()
        {
            // Seed Admin User
            var adminEmail = "admin@hachiko.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

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

                var result = await _userManager.CreateAsync(adminUser, "Demo123@");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, SD.Role_Admin);
                }
            }

            // Seed Customer User
            var customerEmail = "customer@hachiko.com";
            var customerUser = await _userManager.FindByEmailAsync(customerEmail);

            if (customerUser == null)
            {
                customerUser = new ApplicationUser
                {
                    UserName = customerEmail,
                    Email = customerEmail,
                    Name = "Customer User",
                    PhoneNumber = "0123456789",
                    EmailConfirmed = true,
                    StreetAddress = "123 Street",
                    City = "Gotham City",
                    State = "New Jersey State",
                    PostalCode = "12345"
                };

                var result = await _userManager.CreateAsync(customerUser, "Demo123@");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(customerUser, SD.Role_Customer);
                }
            }
        }
    }
}