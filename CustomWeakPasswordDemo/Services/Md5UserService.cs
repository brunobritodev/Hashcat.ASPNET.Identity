using CustomWeakPasswordDemo.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomWeakPasswordDemo.Services
{
    /// <summary>
    /// For god sake. Never use this implementation.
    ///
    /// User Argon2 instead
    /// </summary>
    class Md5UserService : ICustomUserService
    {
        private readonly ApplicationDbContext _context;

        public Md5UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> Login(string user, string password)
        {
            var userDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.ToLower());
            if (userDb == null)
                return null;

            if (Md5PasswordHasher.Verify(userDb.Password, password))
                return userDb;

            return null;
        }

        public async Task<IEnumerable<string>> RegisterUser(User user, string password)
        {
            user.SetPassword(Md5PasswordHasher.Hash(password));

            var userExist = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username.ToLower());
            if (userExist == null)
            {
                user.UpdateUser();
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return new List<string>();
            }
            else
            {
                return new List<string>() { "User already exist" };
            }
        }

        public Task<User> FindByNameAsync(string username)
        {
            return _context.Users.FirstOrDefaultAsync(f => f.Username == username.ToLower());
        }
    }
}