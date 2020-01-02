using System;

namespace TicketingAPI.Helpers
{
    public class EncryptionUtils
	{
        private readonly ECC eccProvider;
        private readonly AES aesProvider;
        private readonly RSA rsaProvider;

        public EncryptionUtils()
        {
            eccProvider = new ECC();
            aesProvider = new AES();
            rsaProvider = new RSA();
        }

        public Tuple<string, string>  GeneratePublicKeyPair()
        {
            return rsaProvider.GenerateKeyPair();
        }

        public byte[] GenerateSharedKey()
        {
            return eccProvider.DeriveKey();
        }

        public Tuple<byte[], byte[]> GenerateSecretKey()
        {
            return aesProvider.GenerateSecretKey();
        }

        /// <summary>
        ///     Method to encrypt using AES
        /// </summary>
        /// <param name="plain">Text to encrypt</param>
        /// <param name="secretKey">Secret key used to encrypt text</param>
        /// <returns>Encryption Result object</returns>
        public Tuple<byte[], byte[]> EncryptAES(string plain, string secretKey)
        {
            var result = aesProvider.Encrypt(plain, secretKey);
            if (result == null) throw new Exception("AES encryptie is mislukt");
            return result;
        }

        /// <summary>
        ///     Method to encrypt using AES
        /// </summary>
        /// <param name="plain">Text to encrypt</param>
        /// <param name="secretKey">Secret key used to encrypt text</param>
        /// <returns>Encryption Result object</returns>
        public Tuple<byte[], byte[]> EncryptAES(string plain, byte[] secretKey, byte[] iv)
        {
            var result = aesProvider.Encrypt(plain, secretKey, iv);
            if (result == null) throw new Exception("AES encryptie is mislukt");
            return result;
        }

        /// <summary>
        ///     Method to encrypt using RSA
        /// </summary>
        /// <param name="plain">Text to encrypt</param>
        /// <param name="publicKey">Public key used for encryption</param>
        /// <returns>Encrypted string</returns>
        public string EncryptRSA(string plain, string publicKey)
        {
            var result = "";
            result = rsaProvider.Encrypt(plain, publicKey);

            if (String.IsNullOrEmpty(result)) throw new Exception("RSA encryptie is mislukt");
            return result;
        }

        /// <summary>
        ///     Method to decrypt using AES
        /// </summary>
        /// <param name="cipher">Text to encrypt</param>
        /// <param name="password">Password used for encryption</param>
        /// <param name="iv">Initialization vector</param>
        /// <returns>Decrypted string</returns>
        public string DecryptAES(string cipher, string password, string iv)
        {
            var result = "";
            result = aesProvider.Decrypt(cipher, password, iv);

            if (String.IsNullOrEmpty(result)) throw new Exception("AES decryptie is mislukt");
            return result;
        }

        /// <summary>
        ///     Method to decrypt using AES
        /// </summary>
        /// <param name="cipher">Text to encrypt</param>
        /// <param name="password">Password used for encryption</param>
        /// <param name="iv">Initialization vector</param>
        /// <returns>Decrypted string</returns>
        public string DecryptAES(string cipher, byte[] key, byte[] iv)
        {
            var result = "";
            result = aesProvider.Decrypt(cipher, key, iv);

            if (String.IsNullOrEmpty(result)) throw new Exception("AES decryptie is mislukt");
            return result;
        }

        /// <summary>
        ///     Method to decrypt using RSA
        /// </summary>
        /// <param name="cipher">Text to encrypt</param>
        /// <param name="privateKey">Private key belonging to public key used to encrypt the cipher</param>
        /// <returns>Decrypted string</returns>
        public string DecryptRSA(string cipher, string privateKey)
        {
            var result = "";
            result = rsaProvider.Decrypt(cipher, privateKey);

            if (String.IsNullOrEmpty(result)) throw new Exception("RSA decryptie is mislukt");
            return result;
        }
    }
}