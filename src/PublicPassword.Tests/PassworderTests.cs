using Microsoft.Extensions.Configuration;
using PublicPassword.Impl;
using Xunit;

namespace PublicPassword.Tests
{
    public class PassworderTests
    {
        [Fact]
        public void CreatesEncryptor_WhenModeIsEncrypt()
        {
            var encryptor = Passworder.Create(Passworder.Mode.Encrypt, new ConfigurationManager());

            Assert.IsType<Encryptor>(encryptor);
        }
    }
}
