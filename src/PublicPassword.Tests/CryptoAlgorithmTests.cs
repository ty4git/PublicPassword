using System;
using System.Linq;
using System.Threading.Tasks;
using PublicPassword.Impl;
using Xunit;

namespace PublicPassword.Tests
{
    public class CryptoAlgorithmTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task EncryptThanDecrypt_ResultsDecryptedTextIsTheSameAsSourceText(int sourceTextLength)
        {
            var random = new Random();
            var password = "password";
            var sourceText = RandomString(sourceTextLength);

            var encrypted = await CryptoAlgorithm.Encrypt(password, sourceText);
            var decryptedText = await CryptoAlgorithm.Decrypt(password, encrypted);

            Assert.True(sourceText == decryptedText);
        }

        public static string RandomString(int length)
        {
            var random = new Random();
            return new string(Enumerable.Range(0, length)
                .Select(_ => (char)random.Next(33, 126))
                .ToArray());
        }
    }
}
