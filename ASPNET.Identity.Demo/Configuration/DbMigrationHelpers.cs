using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ASPNET.Identity.Demo.Data;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ASPNET.Identity.Demo.Configuration
{
    public static class DbMigrationHelpers
    {
        public static async Task EnsureSeedData(IServiceScope serviceScope)
        {
            var services = serviceScope.ServiceProvider;
            await EnsureSeedData(services);
        }

        public static async Task EnsureSeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await db.Database.EnsureCreatedAsync();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await EnsureSeedIdentityData(userManager, roleManager);
            }
        }

        /// <summary>
        /// Generate default admin user / role
        /// </summary>
        private static async Task EnsureSeedIdentityData(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {

            // Create admin role
            if (!await roleManager.RoleExistsAsync("Managers"))
            {
                var role = new IdentityRole("Managers");

                await roleManager.CreateAsync(role);
            }

            // Create admin user
            if (await userManager.FindByNameAsync("teste@teste.com") != null) return;

            var user = new IdentityUser()
            {
                UserName = "teste@teste.com",
                Email = "teste@teste.com",
                EmailConfirmed = true,
                LockoutEnd = null
            };

            var result = await userManager.CreateAsync(user, "P@ssw0rd");

            if (result.Succeeded)
            {
                await userManager.AddClaimAsync(user, new Claim("EmployeeFunction", "Manager"));
                await userManager.AddClaimAsync(user, new Claim("Department", "Sales"));
                await userManager.AddToRoleAsync(user, "Managers");
            }

            var faker = new Faker();
            var username = faker.Person.Email;
            await userManager.CreateAsync(new IdentityUser()
            {
                UserName = username,
                Email = username,
                EmailConfirmed = true,
                LockoutEnd = null
            }, "P455w0rd");
            
            username = faker.Person.Email;
            await userManager.CreateAsync(new IdentityUser()
            {
                UserName = username,
                Email = username,
                EmailConfirmed = true,
                LockoutEnd = null
            }, "Password");

            username = faker.Person.Email;
            await userManager.CreateAsync(new IdentityUser()
            {
                UserName = username,
                Email = username,
                EmailConfirmed = true,
                LockoutEnd = null
            }, "Test1234");

            username = faker.Person.Email;
            await userManager.CreateAsync(new IdentityUser()
            {
                UserName = username,
                Email = username,
                EmailConfirmed = true,
                LockoutEnd = null
            }, "MYSECRET");

            username = faker.Person.Email;
            await userManager.CreateAsync(new IdentityUser()
            {
                UserName = username,
                Email = username,
                EmailConfirmed = true,
                LockoutEnd = null
            }, "HELLO");
        }

    }
}
