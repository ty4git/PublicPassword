using Microsoft.Extensions.Configuration;
using PublicPassword.Implementation.Passworder;
using Xunit;

namespace PublicPassword.Tests;

public class PassworderTests
{
    [Fact]
    public void CreatesEncryptor_WhenModeIsEncrypt()
    {
        var cfg = new ConfigurationManager();
        cfg.AddJsonFile("appsettings.json");
        var encryptor = Builder.CreatePassworder(Mode.Encrypt, cfg);

        Assert.IsAssignableFrom<IPassworder>(encryptor);
        Assert.IsType<Encryptor>(encryptor);
    }
}