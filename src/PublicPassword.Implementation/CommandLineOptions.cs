using CommandLine;

namespace PublicPassword.Implementation
{
    internal class CommandLineOptions
    {
        [Option(shortName: 'e',
            longName: "encrypting-text",
            HelpText = "Text for encryption",
            MetaValue = "string")]
        public string EncryptingText { get; set; }

        [Option(shortName: 'p',
            longName: "password",
            HelpText = "Password that used for encryption or decryption of text",
            MetaValue = "string")]
        public string Password { get; set; }
    }
}
