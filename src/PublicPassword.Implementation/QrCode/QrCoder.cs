using System;
using System.IO;
using ZXing;
using ZXing.Common;
using SkiaSharp;
using ZXing.SkiaSharp;
using ZXing.SkiaSharp.Rendering;

namespace PublicPassword.Implementation.QrCode;

internal static class QrCoder
{
    public static SKBitmap CreateImage(byte[] data)
    {
        var writer = new BarcodeWriter<SKBitmap>
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new EncodingOptions { Height = 400, Width = 400 },
            Renderer = new SKBitmapRenderer()
        };
        var qrCode = writer.Write(Convert.ToBase64String(data));
        return qrCode;
    }

    public static void SaveToFile(byte[] data, FileInfo outFile)
    {
        var bitmap = CreateImage(data);
        using var image = SKImage.FromBitmap(bitmap);
        using var png = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.OpenWrite(outFile.FullName);
        png.SaveTo(stream);
    }

    public static byte[] ReadFromFile(FileInfo inputFile)
    {
        using var stream = File.OpenRead(inputFile.FullName);
        using var skImage = SKImage.FromEncodedData(stream);

        var bitmap = SKBitmap.FromImage(skImage);
        var qrCode = new BarcodeReader<SKBitmap>(bitmap => new SKBitmapLuminanceSource(bitmap)).Decode(bitmap);
        var data = Convert.FromBase64String(qrCode.Text);
        return data;
    }
}
