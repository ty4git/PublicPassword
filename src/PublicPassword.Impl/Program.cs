using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ZXing;

namespace PublicPassword.Impl
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var mode = CommandArgsParser.GetMode(args);
            var configuration = GetConfiguration(args);

            var passworder = Passworder.Create(mode, configuration);
            await passworder.Do();

            return 0;
        }

        private static IConfiguration GetConfiguration(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            return configuration;
        }

    }
}