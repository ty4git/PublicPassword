using System;
using System.Threading.Tasks;
using PublicPassword.Implementation.QrCode;
using PublicPassword.Implementation.Encryption;

namespace PublicPassword.Implementation.Passworder;

internal class Decryptor : IPassworder
{
    private readonly DecryptorConfiguration _configuration;

    public Decryptor(DecryptorConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task Do()
    {
        var qrCodeData = QrCoder.ReadFromFile(_configuration.InputFile);
        var text = await CryptoAlgorithm.Decrypt(_configuration.Password, qrCodeData);
        await Console.Out.WriteLineAsync(text);
    }
}