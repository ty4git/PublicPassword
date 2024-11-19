using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace PublicPassword.Implementation.Passworder;

internal class Configuration
{
    private readonly IConfiguration _globalConfiguration;

    public Configuration(IConfiguration globalConfiguration)
    {
        _globalConfiguration = globalConfiguration;
    }

    public EncryptorConfiguration GetForEncryptor()
    {
        var paramNames = (
            Password: "password",
            Text: "text",
            OutFile: "out-file");

        var password = GetParamValue(paramNames.Password);
        var text = GetParamValue(paramNames.Text);
        var outFilePath = GetParamValue(paramNames.OutFile);

        var outFile = GetFileInfo(outFilePath, paramNames.OutFile);

        var cfg = new EncryptorConfiguration
        {
            Password = password,
            Text = text,
            OutFile = outFile
        };
        return cfg;
    }

    public DecryptorConfiguration GetForDecryptor()
    {
        var paramNames = (Password: "password", InputFile: "input-file");

        var password = GetParamValue(paramNames.Password);
        var inputFilePath = GetParamValue(paramNames.InputFile);

        var inputFile = GetFileInfo(inputFilePath, paramNames.InputFile);

        var cfg = new DecryptorConfiguration
        {
            Password = password,
            InputFile = inputFile
        };

        return cfg;
    }

    private string GetParamValue(string paramName)
    {
        var value = _globalConfiguration[paramName];
        if (string.IsNullOrEmpty(value))
        {
            throw new Exception(
                @$"The ""{paramName}"" is empty or null but should be.");
        }

        return value;
    }

    private FileInfo GetFileInfo(string fileName, string paramName)
    {
        var file = new FileInfo(fileName);
        return file;
    }
}

public class EncryptorConfiguration
{
    public string Password { get; set; }

    public string Text { get; set; }

    public FileInfo OutFile { get; set; }
}

public class DecryptorConfiguration
{
    public string Password { get; set; }

    public FileInfo InputFile { get; set; }
}