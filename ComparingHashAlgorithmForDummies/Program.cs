using System;
using System.Diagnostics;
using NetDevPack.Utilities;
using Sodium;

namespace ComparingHashAlgorithmForDummies
{
    class Program
    {
        private static long _time = 20_000;
        static void Main(string[] args)
        {
            var password = "Sup3rSecr3t";
            Console.WriteLine($"How many times the Password {password} can be hashed in 20 seconds?");
            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine("            MD5 hashes");
            var (seconds, hashCount) = MD5(password);
            Console.WriteLine($"MD5 Hashs in {seconds}: {hashCount:N}");

            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine("            SHA256 hashes");

            (seconds, hashCount) = Sha256(password);
            Console.WriteLine($"SHA256 Hashs in {seconds}: {hashCount:N}");

            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine("            Argon2 hashes");

            (seconds, hashCount) = Argon2(password);
            Console.WriteLine($"Argon2 Hashs in {seconds}: {hashCount:N}");
        }

        private static (double, int) MD5(string password)
        {
            var seconds = Stopwatch.StartNew();
            var hashCount = 0;
            do
            {
                Md5PasswordHasher.Hash(password);
                hashCount++;
            } while (seconds.ElapsedMilliseconds < _time);

            seconds.Stop();
            return (seconds.Elapsed.TotalSeconds, hashCount);
        }


        private static (double, int) Sha256(string password)
        {
            var seconds = Stopwatch.StartNew();
            var hashCount = 0;
            do
            {
                password.ToSha256();
                hashCount++;
            } while (seconds.ElapsedMilliseconds < _time);

            seconds.Stop();
            return (seconds.Elapsed.TotalSeconds, hashCount);
        }

        private static (double, int) Argon2(string password)
        {
            var seconds = Stopwatch.StartNew();
            var hashCount = 0;
            do
            {
                PasswordHash.ArgonHashString(password, PasswordHash.StrengthArgon.Moderate);
                hashCount++;
            } while (seconds.ElapsedMilliseconds < _time);

            seconds.Stop();
            return (seconds.Elapsed.TotalSeconds, hashCount);
        }
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
