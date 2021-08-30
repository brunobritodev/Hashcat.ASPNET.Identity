using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomWeakPasswordDemo.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomWeakPasswordDemo.Services
{
   
    public class Md5PasswordHasher
    {
        public static string Hash(ReadOnlySpan<char> input)
        {
            var encoding = System.Text.Encoding.UTF8;
            var inputByteCount = encoding.GetByteCount(input);
            using var md5 = System.Security.Cryptography.MD5.Create();

            Span<byte> bytes = inputByteCount < 1024
                ? stackalloc byte[inputByteCount]
                : new byte[inputByteCount];
            Span<byte> destination = stackalloc byte[md5.HashSize / 8];

            encoding.GetBytes(input, bytes);

            // checking the result is not required because this only returns false if "(destination.Length < HashSizeValue/8)", which is never true in this case
            md5.TryComputeHash(bytes, destination, out int _bytesWritten);

            return BitConverter.ToString(destination.ToArray()).Replace("-", "").ToLower();
        }

        public static bool Verify(string hashedPass, string password)
        {
            var userHashedPass = Hash(password);
            return hashedPass.Equals(userHashedPass);
        }
    }
}
