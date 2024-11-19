using System;
using Microsoft.Extensions.Configuration;

namespace PublicPassword.Implementation.Passworder;

public class Builder
{
    public static IPassworder CreatePassworder(Mode mode, IConfiguration globalConfiguration)
    {
        if (mode == Mode.Encrypt)
        {
            var cfg = new Configuration(globalConfiguration).GetForEncryptor();
            var encryptor = new Encryptor(cfg);
            return encryptor;
        }
        else if (mode == Mode.Decrypt)
        {
            var cfg = new Configuration(globalConfiguration).GetForDecryptor();
            var decryptor = new Decryptor(cfg);
            return decryptor;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(mode), @$"The mode ""{mode}"" is out of possible values");
        }
    }
}