using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;

namespace PublicPassword.Impl
{
    internal static class QrCoder
    {
        public static Bitmap CreateImage(byte[] data)
        {
            var writer = new BarcodeWriter { Format = BarcodeFormat.QR_CODE };
            var qrCode = writer.Write(Convert.ToBase64String(data));
            return qrCode;
        }

        public static void SaveToFile(byte[] data, FileInfo outFile)
        {
            var image = CreateImage(data);
            image.Save(outFile.FullName, ImageFormat.Png);
        }

        public static byte[] ReadFromFile(FileInfo inputFile)
        {
            var image = Image.FromFile(inputFile.FullName) as Bitmap
                        ?? throw new Exception(@"The input file has incorrect content. File should contain ""png"" image.");

            var qrCode = new BarcodeReader().Decode(image);
            var data = Convert.FromBase64String(qrCode.Text);

            //todo: logger
            Console.WriteLine($"qr code data: {string.Join("|", data)}");

            return data;
        }
    }
}
