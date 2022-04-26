using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PublicPassword.Implementation
{
    public class CryptoAlgorithm
    {
        private static readonly Encoding _textEncoding = Encoding.UTF8;

        public static async Task<byte[]> Encrypt(string password, string text)
        {
            using var alg = CreateSymmetricAlgorithm<AesManaged>(password);

            using var encryptor = alg.CreateEncryptor();
            await using var to = new MemoryStream();
            await using var writer = new CryptoStream(
                to,
                encryptor,
                CryptoStreamMode.Write);

            var encodedText = _textEncoding.GetBytes(text);
            await writer.WriteAsync(encodedText);
            writer.FlushFinalBlock();

            var encryptedData = to.ToArray();
            return encryptedData;
        }

        public static async Task<string> Decrypt(string password, byte[] encryptedData)
        {
            using var alg = CreateSymmetricAlgorithm<AesManaged>(password);
            using var decryptor = alg.CreateDecryptor();

            await using var from = new MemoryStream(encryptedData);
            await using var cryptoStream = new CryptoStream(from, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cryptoStream, _textEncoding);
            try
            {
                var result = await reader.ReadToEndAsync();
                return result;
            }
            catch (CryptographicException ex)
            {
                throw new Exception("Some error happened during decryption of data. " +
                                    "One of reason is you are trying to decrypt using wrong password.", ex);
            }
        }

        private static SymmetricAlgorithm CreateSymmetricAlgorithm<T>(string password)
            where T : SymmetricAlgorithm, new()
        {
            var alg = new T();
            try
            {
                var passwordBytes = _textEncoding.GetBytes(password);
                var salt = SHA256.Create().ComputeHash(passwordBytes);
                var passwordDerivedBytes = new Rfc2898DeriveBytes(passwordBytes, salt, iterations: 1000,
                    HashAlgorithmName.SHA1);

                alg.Key = passwordDerivedBytes.GetBytes(alg.KeySize / 8);
                alg.IV = passwordDerivedBytes.GetBytes(alg.BlockSize / 8);
                alg.Mode = CipherMode.CBC;
                alg.Padding = PaddingMode.PKCS7;
            }
            catch
            {
                alg.Dispose();
                throw;
            }

            return alg;
        }
    }
}
