using Microsoft.Extensions.Configuration;
using PublicPassword.Implementation.Passworder;
using Xunit;

namespace PublicPassword.Tests;

public class PassworderTests
{
    [Fact]
    public void CreatesEncryptor_WhenModeIsEncrypt()
    {
        var encryptor = Builder.CreatePassworder(Mode.Encrypt, new ConfigurationManager());

        Assert.IsAssignableFrom<IPassworder>(encryptor);
        Assert.IsType<Encryptor>(encryptor);
    }
}