using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using CustomWeakPasswordDemo.Services;

namespace CustomWeakPasswordDemo.Data
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string HashType { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
        }

        public void UpdateUser()
        {
            Username = Username.ToLower();
        }

        public void SetPassword(string password)
        {
            if (HashType == "WeakOSS1")
            {
                Password = SecureIdentity.Password.PasswordHasher.Hash(password);
            }
            else
            {
                Password = Md5PasswordHasher.Hash(password);
            }
        }

        public bool CheckPassword(string password)
        {
            if (HashType == "WeakOSS1")
            {
                return SecureIdentity.Password.PasswordHasher.Verify(Password, password);
            }
            else
            {
                return Md5PasswordHasher.Verify(Password, password);
            }
        }

        public string GetHashcatFormat()
        {
            if (HashType == "WeakOSS1")
            {
                var passwordData = Password.Split(".");
                var iter = passwordData[0];
                var salt = passwordData[1];
                var subkey = passwordData[2];
                return $"sha256:{iter}:{salt}:{subkey}";
            }
            else
            {
                return Password;
            }
        }

        public string Alg()
        {
            if (HashType == "WeakOSS1")
            {
                return "PBKDF2 with HMAC-SHA-256, 16 bit salt, 32 bits subkey";
            }
            else
            {
                return "MD5";
            }
        }
    }
}