using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Bogus;
using CustomWeakPasswordDemo.Data;
using CustomWeakPasswordDemo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CustomWeakPasswordDemo.Configuration
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
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await db.Database.EnsureCreatedAsync();
            var userManager = scope.ServiceProvider.GetRequiredService<ICustomUserService>();

            await EnsureSeedIdentityData(userManager);
        }

        /// <summary>
        /// Generate default admin user / role
        /// </summary>
        private static async Task EnsureSeedIdentityData(
            ICustomUserService customUserManager)
        {
            
            // Create admin user
            if (await customUserManager.FindByNameAsync("teste@teste.com") != null) return;


            await customUserManager.RegisterUser(new User()
            {
                Username = "teste@teste.com",
                HashType = "MD5"
            }, "P@ssw0rd");

            var faker = new Faker();
            var hashOptions = new List<string>() { "MD5" };
            await customUserManager.RegisterUser(new User()
            {
                Username = new Faker().Person.Email,
                HashType = faker.PickRandom(hashOptions)
            }, "Password");

            await customUserManager.RegisterUser(new User()
            {
                Username = new Faker().Person.Email,
                HashType = faker.PickRandom(hashOptions)
            }, "HELLO");

            await customUserManager.RegisterUser(new User()
            {
                Username = new Faker().Person.Email,
                HashType = faker.PickRandom(hashOptions)
            }, "P455w0rd");

            await customUserManager.RegisterUser(new User()
            {
                Username = new Faker().Person.Email,
                HashType = faker.PickRandom(hashOptions)
            }, "Test1234");


            await customUserManager.RegisterUser(new User()
            {
                Username = new Faker().Person.Email,
                HashType = faker.PickRandom(hashOptions)
            }, "MYSECRET");
        }

    }
}
