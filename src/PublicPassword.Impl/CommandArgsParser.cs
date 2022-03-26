using System;
using System.Linq;

namespace PublicPassword.Impl
{
    internal class CommandArgsParser
    {
        public static Passworder.Mode GetMode(string[] args)
        {
            const string encryptCommandName = "encrypt";
            const string decryptCommandName = "decrypt";

            if (args.Length > 0
                && new[] { encryptCommandName, decryptCommandName }.Contains(args[0]))
            {
                return args[0] == encryptCommandName
                    ? Passworder.Mode.Encrypt
                    : Passworder.Mode.Decrypt;
            }

            throw new Exception(
                "There is no command or it's incorrect command." +
                @$" Should be ""{encryptCommandName}"" or ""{decryptCommandName}""");
        }
    }
}
