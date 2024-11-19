using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PublicPassword.Implementation;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var mode = CommandArgsParser.GetMode(args);
        var configuration = GetConfiguration(args);

        var passworder = Passworder.Builder.CreatePassworder(mode, configuration);

        try
        {
            await passworder.Do();
        }
        catch (FileNotFoundException ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
        }
        
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