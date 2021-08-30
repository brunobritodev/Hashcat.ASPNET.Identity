using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CustomWeakPasswordDemo.Data;

namespace CustomWeakPasswordDemo.Services
{
    public interface ICustomUserService
    {
        Task<User> Login(string user, string password);
        Task<IEnumerable<string>> RegisterUser(User user, string password);
        Task<User> FindByNameAsync(string username);
    }
    
}