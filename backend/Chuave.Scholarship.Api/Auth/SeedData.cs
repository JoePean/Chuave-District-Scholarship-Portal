
using Chuave.Scholarship.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace Chuave.Scholarship.Api.Auth
{
    public static class SeedData
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            var roles = new[] { "Admin", "Applicant" };
            foreach (var r in roles)
                if (!await roleManager.RoleExistsAsync(r))
                    await roleManager.CreateAsync(new IdentityRole(r));

            var adminEmail = "admin@chuave.gov.pg";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser { Email = adminEmail, UserName = adminEmail, EmailConfirmed = true };
                var res = await userManager.CreateAsync(admin, "Admin!234");
                if (res.Succeeded) await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
