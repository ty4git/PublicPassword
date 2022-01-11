using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CommandLine;
using Microsoft.Extensions.Configuration;

namespace PublicPassword.Impl
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var password = configuration["password"];
            var encryptingText = configuration["encrypting-text"];

            using var aesAlg = Aes.Create();
            aesAlg.Key = Encoding.ASCII.GetBytes(password);
            aesAlg.IV = new[] { }; //todo: should be random
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(
                memoryStream, 
                aesAlg.CreateEncryptor(),
                CryptoStreamMode.Write);
            using streamWriter

            //CommandLine.Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed();
        }
    }
}