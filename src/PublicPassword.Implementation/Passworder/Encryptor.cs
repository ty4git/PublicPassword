using System.Threading.Tasks;
using PublicPassword.Implementation.Encryption;
using PublicPassword.Implementation.QrCode;

namespace PublicPassword.Implementation.Passworder;

public class Encryptor : IPassworder
{
    private readonly EncryptorConfiguration _configuration;

    public Encryptor(EncryptorConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task Do()
    {
        var encryptedData = await CryptoAlgorithm.Encrypt(_configuration.Password, _configuration.Text);
        QrCoder.SaveToFile(encryptedData, _configuration.OutFile);
    }
}