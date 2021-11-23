using CustomWeakPasswordDemo.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomWeakPasswordDemo.Services
{
    /// <summary>
    /// Typical first day exercise in Computers Science
    /// </summary>
    class FirstClassUserService : ICustomUserService
    {
        private readonly ApplicationDbContext _context;

        public FirstClassUserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<User> Login(string user, string password)
        {
            var userDb =
                from usuario in _context.Users
                where usuario.Username == user && usuario.Password == password
                select usuario;


            return userDb.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<string>> RegisterUser(User user, string password)
        {
            user.SetPassword(password);

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