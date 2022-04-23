using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PublicPassword.Implementation
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