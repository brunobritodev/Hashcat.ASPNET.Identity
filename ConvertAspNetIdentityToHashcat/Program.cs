using System;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NetDevPack.Utilities;
using System.IO;
using System.Threading.Tasks;

namespace ConvertAspNetIdentityToHashcat
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }
        static async Task Main(string[] args)
        {
            SetupConfigurationBuilder();
            
            var hashDemo = "AQAAAAEAACcQAAAAEG7xx8smhzcYFaAhPSRj1rgxfAoqKBv4WM/4R+Z0SvFxtxuMkfgBS28p1MQzvV0OeQ==";
            var hashDemoBase64Decoded = hashDemo.FromBase64();
            var hex = BitConverter.ToString(hashDemoBase64Decoded).Replace("-", "").ToLower();
            
            /*
                Info got from: https://github.com/dotnet/aspnetcore/blob/867cec475d18892b828ac44a82d74eccfbbb0e49/src/Identity/Extensions.Core/src/PasswordHasher.cs

               Hex -> 01-00000001-00002710-00000010-6ef1c7cb2687371815a0213d2463d6b8-317c0a2a281bf858cff847e6744af171b71b8c91f8014b6f29d4c433bd5d0e79
             * version-> 01
               prf: 00000001
               Iter count 00002710
               Salt Length: 00000010
               Salt -> 6ef1c7cb2687371815a0213d2463d6b8
               Subkey -> 317c0a2a281bf858cff847e6744af171b71b8c91f8014b6f29d4c433bd5d0e791
             */

            Console.WriteLine($"Demo Hash: {hex}");
            await using var db = new SqlConnection(Configuration["ConnectionStrings:DefaultConnection"]);
            var hashedPasswords = await db.QueryAsync<string>("SELECT PasswordHash FROM AspNetUsers");

            Console.WriteLine("");
            Console.WriteLine("=======================================================================");
            Console.WriteLine("==========================HASHCAT FORMATED HASH========================");
            Console.WriteLine("=======================================================================");
            Console.WriteLine("");
            foreach (var hash in hashedPasswords)
            {
                var asphash = new AspNetIdentityHashInfo(hash);
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(asphash.ShaType);
                Console.ResetColor();
                Console.Write(":");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(asphash.IterCount);
                Console.ResetColor();
                Console.Write(":");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write(asphash.Salt);
                Console.ResetColor();
                Console.Write(":");
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine(asphash.SubKey);
                Console.ResetColor();
            }
        }

        private static void SetupConfigurationBuilder()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);

            Configuration = builder.Build();
        }

    }
}
