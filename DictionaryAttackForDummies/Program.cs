using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DictionaryAttackForDummies
{
    /// <summary>
    /// Demo purposes only.
    ///
    /// Use hashcat instead
    /// </summary>
    class Program
    {

        static void Main(string[] args)
        {
            var hashes = new HashSet<string>() {
                "dc647eb65e6711e155375218212b3964",
                "eb61eead90e3b899c6bcbe27ac581660",
                "958152288f2d2303ae045cffc43a02cd",
                "2c9341ca4cf3d87b9e4eb905d6a3ec45",
                "75b71aa6842e450f12aca00fdf54c51d",
                "031cbcccd3ba6bd4d1556330995b8d08",
                "b5af0b804ff7238bce48adef1e0c213f",
                "75b71aa6842e450f12aca00fdf54c51d"
            };

            var elapsedTime = Stopwatch.StartNew();
            var dictionary = new Dictionary(Path.Combine(Directory.GetCurrentDirectory(), "rockyou.txt"));
            var password = dictionary.GetPassword();

            do
            {
                string hashToRemove = null;
                foreach (var hash in hashes)
                {
                    if (hash.Equals(Md5PasswordHasher.Hash(password)))
                    {
                        hashToRemove = hash;
                        Console.WriteLine($"{hash}:{password}");
                    }
                }

                if (!string.IsNullOrEmpty(hashToRemove))
                    hashes.Remove(hashToRemove);

                password = dictionary.GetPassword();
            } while (password is not null || !hashes.Any());

           
            Console.WriteLine($"Elapsed time: {elapsedTime.Elapsed.TotalSeconds}");
        }


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

        }
    }
}
