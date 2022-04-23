using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace PublicPassword.Implementation
{
    public abstract class Passworder
    {
        protected readonly IConfiguration _configuration;

        public static Passworder Create(Mode mode, IConfiguration configuration)
        {
            return mode == Mode.Encrypt ? new Encryptor(configuration) : new Decryptor(configuration);
        }

        protected Passworder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public abstract Task Do();

        public enum Mode
        {
            Encrypt = 1,
            Decrypt = 2
        }
    }

    public class Encryptor : Passworder
    {
        public Encryptor(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override async Task Do()
        {
            var (password, text, outFile) = GetParams();
            var encryptedData = await CryptoAlgorithm.Encrypt(password, text);
            QrCoder.SaveToFile(encryptedData, outFile);
        }

        private (string Password, string Text, FileInfo OutFile) GetParams()
        {
            var paramNames = (
                Password: "password",
                Text: "text",
                OutFile: "out-file");

            var password = GetParamValue(paramNames.Password);
            var text = GetParamValue(paramNames.Text);
            var outFile = GetParamValue(paramNames.OutFile);

            var outFileInfo = new FileInfo(outFile);
            if (!new[] { ".png", string.Empty }.Contains(outFileInfo.Extension))
            {
                throw new Exception(
                    @$"""{paramNames.OutFile}"" param can have extensions "".png"" only.");
            }

            return (password, text, outFileInfo);

            string GetParamValue(string paramName)
            {
                var value = _configuration[paramName];
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception(
                        @$"The ""{paramName}"" is empty or null but should be.");
                }

                return value;
            }
        }
    }

    public class Decryptor : Passworder
    {
        public Decryptor(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override async Task Do()
        {
            var (password, inputFile) = GetParams();
            var qrCodeData = QrCoder.ReadFromFile(inputFile);
            var text = await CryptoAlgorithm.Decrypt(password, qrCodeData);
            await Console.Out.WriteLineAsync(text);
        }

        private (string Password, FileInfo InputFile) GetParams()
        {
            var names = (Password: "password", InputFile: "input-file");

            var password = GetParamValue(names.Password);
            var inputFilePath = GetParamValue(names.InputFile);

            var inputFile = new FileInfo(inputFilePath);
            if (!new[] { ".png", string.Empty }.Contains(inputFile.Extension))
            {
                throw new Exception(@"""input-file"" param can have extensions "".png only.");
            }

            return (password, inputFile);

            string GetParamValue(string paramName)
            {
                var value = _configuration[paramName];
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception(
                        @$"The ""{paramName}"" is empty or null but should be.");
                }

                return value;
            }
        }
    }
}
