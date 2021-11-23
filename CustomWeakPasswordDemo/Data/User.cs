using System;
using System.ComponentModel.DataAnnotations;

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
            Password = password;
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

            // MD5 is in hashcat format. If plain text....
            return Password;
        }

        public string Alg()
        {
            if (HashType == "WeakOSS1")
            {
                return "PBKDF2 with HMAC-SHA-256, 16 bit salt, 32 bits subkey";
            }
            else if (HashType == "MD5")
            {
                return "MD5";
            }

            return "Plain Text";
        }
    }
}