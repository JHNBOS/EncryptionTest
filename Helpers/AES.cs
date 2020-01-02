using System;
using System.IO;
using System.Security.Cryptography;

namespace TicketingAPI.Helpers
{
    public class AES
    {
        /// <summary>
        ///     Method to encrypt plain text using AES
        /// </summary>
        /// <param name="plain">Plain text</param>
        /// <param name="key">Secret key used to encrypt</param>
        /// <param name="keySize">Size of the secret key</param>
        /// <returns>Cipher and IV</returns>
        public Tuple<byte[], byte[]> Encrypt(string plain, byte[] key, byte[] iv, int keySize = 256)
        {
            byte[] cipher;

            using (Aes aes = Aes.Create())
            {
                // Set AES algorithm properties to match the one used in front-end
                aes.KeySize = keySize;
                aes.BlockSize = aes.KeySize / 2;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key;

                if (iv == null)
                {
                    aes.GenerateIV();
                } else
                {
                    aes.IV = iv;
                }

                ICryptoTransform aesEncryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plain);
                        }
                        cipher = memoryStream.ToArray();
                    }
                }
            }

            return new Tuple<byte[], byte[]>(cipher, iv);
        }

        /// <summary>
        ///     Method to encrypt plain text using AES
        /// </summary>
        /// <param name="plain">Plain text</param>
        /// <param name="key">Secret key used to encrypt</param>
        /// <param name="keySize">Size of the secret key</param>
        /// <returns>Cipher and IV</returns>
        public Tuple<byte[], byte[]> Encrypt(string plain, string key, int keySize = 256)
        {
            byte[] encrypted;
            byte[] hash;
            byte[] iv;

            using (Aes aes = Aes.Create())
            {
                // Set AES algorithm properties to match the one used in front-end
                aes.KeySize = keySize;
                aes.BlockSize = aes.KeySize / 2;

                Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, 16, 1000);
                hash = rfc2898DeriveBytes.GetBytes(32);
                iv = rfc2898DeriveBytes.Salt;

                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = hash;
                aes.IV = iv;

                ICryptoTransform aesEncryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plain);
                        }
                        encrypted = memoryStream.ToArray();
                    }
                }
            }

            return new Tuple<byte[], byte[]>(encrypted, iv);
        }

        /// <summary>
        ///     Method to decrypt ciphertext using AES
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="iv"></param>
        /// <param name="password"></param>
        /// <returns>Decrypted string</returns>
        public string Decrypt(string cipherText, string password, string iv)
        {
            string plaintext = null;
            using (Aes aes = Aes.Create())
            {
                // Set AES algorithm properties
                aes.Mode = CipherMode.CBC;
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.FeedbackSize = 128;
                aes.Padding = PaddingMode.PKCS7;

                // Set key and initialization vector
                aes.Key = Convert.FromBase64String(password);
                aes.IV = Convert.FromBase64String(iv);

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }

        /// <summary>
        ///     Method to decrypt ciphertext using AES
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="iv"></param>
        /// <param name="password"></param>
        /// <returns>Decrypted string</returns>
        public string Decrypt(string cipherText, byte[] key, byte[] iv)
        {
            string plaintext = null;
            using (Aes aes = Aes.Create())
            {
                // Set AES algorithm properties
                aes.Mode = CipherMode.CBC;
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.FeedbackSize = 128;
                aes.Padding = PaddingMode.PKCS7;

                // Set key and initialization vector
                aes.Key = key;
                aes.IV = iv;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }

        /// <summary>
        ///     Generate a random secret key
        /// <param name="keySize">Size of secret key</param>
        /// </summary>
        /// <returns>Secret and IV</returns>
        public Tuple<byte[], byte[]> GenerateSecretKey(int keySize = 256)
        {
            byte[] key;
            byte[] iv;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = keySize;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                aesAlg.GenerateIV();
                aesAlg.GenerateKey();

                key = aesAlg.Key;
                iv = aesAlg.IV;
            }

            return new Tuple<byte[], byte[]>(key, iv);
        }

    }
}
