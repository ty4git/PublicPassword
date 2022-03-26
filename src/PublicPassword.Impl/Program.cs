using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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

            await Test2(configuration);

            //var passworder = Passworder.Create(mode, configuration);
            //await passworder.Do();

            return 0;
        }

        private static void Test()
        {
            //var data = Encoding.UTF8.GetBytes("ET hi man");
            //using var qrCodeGenerator = new QRCodeGenerator();
            //var qrCodeData = qrCodeGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

            //var qrCode = new QRCode(qrCodeData);
            //var qrCodeImage = qrCode.GetGraphic(16);
            //qrCodeImage.Save("tq1.png", ImageFormat.Png);



            var barcodeWriter = new BarcodeWriter { Format = BarcodeFormat.QR_CODE };
            var outQrImage = barcodeWriter.Write(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes("hi")));
            outQrImage.Save("tq1.png", ImageFormat.Png);



            var qrImage = Image.FromFile("tq1.png") as Bitmap;
            var barcodeReader = new BarcodeReader();
            var qr = barcodeReader.Decode(qrImage);

            Console.WriteLine($"{nameof(qr)}/{nameof(qr.RawBytes)}: {string.Join("|", qr.RawBytes)}");
            Console.WriteLine($"{nameof(qr)}/{nameof(qr.Text)}: {qr.Text}");
            Console.WriteLine(string.Join("|", Encoding.UTF8.GetBytes(qr.Text)));
        }

        private static async Task Test2(IConfiguration configuration)
        {
            var encryptor = new Encryptor(configuration);
            await encryptor.Do();

            var decryptor = new Decryptor(configuration);
            await decryptor.Do();
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

        internal abstract class Passworder
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

        internal class Encryptor : Passworder
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

        internal class Decryptor : Passworder
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
                if (!new[] { ".png", ".txt", string.Empty }.Contains(inputFile.Extension))
                {
                    throw new Exception(@"""input-file"" param can have extensions "".png, .txt"" only.");
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

    internal class CommandArgsParser
    {
        public static Program.Passworder.Mode GetMode(string[] args)
        {
            const string encryptCommandName = "encrypt";
            const string decryptCommandName = "decrypt";

            if (args.Length > 0
                && new[] { encryptCommandName, decryptCommandName }.Contains(args[0]))
            {
                return args[0] == encryptCommandName
                    ? Program.Passworder.Mode.Encrypt
                    : Program.Passworder.Mode.Decrypt;
            }

            throw new Exception(
                "There is no command or it's incorrect command." +
                @$" Should be ""{encryptCommandName}"" or ""{decryptCommandName}""");
        }
    }
}