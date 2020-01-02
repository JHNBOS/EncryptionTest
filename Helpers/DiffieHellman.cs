
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TicketingAPI.Helpers
{
    public class DiffieHellman
	{
        public byte[] SenderPublicKey { get; set; }
        public byte[] ReceiverPublicKey { get; set; }
        public byte[] EncryptedMessage { get; set; }

        public string Init()
        {
            using (ECDiffieHellmanCng receiver = new ECDiffieHellmanCng(521))
            {
                receiver.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                receiver.HashAlgorithm = CngAlgorithm.Sha256;

                // Get public key of receiver
                ReceiverPublicKey = receiver.PublicKey.ToByteArray();

                using (ECDiffieHellmanCng sender = new ECDiffieHellmanCng(521))
                {
                    sender.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                    sender.HashAlgorithm = CngAlgorithm.Sha256;

                    // Get public key of sender
                    SenderPublicKey = sender.PublicKey.ToByteArray();

                    var senderKey = sender.DeriveKeyMaterial(CngKey.Import(ReceiverPublicKey, CngKeyBlobFormat.EccPublicBlob));
                    var receiverKey = receiver.DeriveKeyMaterial(CngKey.Import(SenderPublicKey, CngKeyBlobFormat.EccPublicBlob));

                    if(senderKey.SequenceEqual(receiverKey))
                    {
                        var cipher = Encrypt(senderKey, @"{transaction:38547494,seat:271,timestamp:1577788104}");
                        var plainText = Decrypt(cipher.Item1, cipher.Item2, receiverKey);

                        return Convert.ToBase64String(cipher.Item1);
                    }                    
                }
            }
            return String.Empty;
        }

        private Tuple<byte[], byte[]> Encrypt(byte[] key, string message)
        {
            byte[] cipher;
            byte[] iv;

            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Key = key;

                aes.GenerateIV();
                iv = aes.IV;

                // Encrypt the message
                using (MemoryStream ciphertext = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] plaintextMessage = Encoding.UTF8.GetBytes(message);

                    cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                    cs.Close();

                    cipher = ciphertext.ToArray();
                }
            }
            return new Tuple<byte[], byte[]>(cipher, iv);
        }

        public string Decrypt(byte[] encryptedMessage, byte[] iv, byte[] key)
        {
            string message;
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Key = key;
                aes.IV = iv;

                // Decrypt the message
                using (MemoryStream plaintext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedMessage, 0, encryptedMessage.Length);
                        cs.Close();
                        message = Encoding.UTF8.GetString(plaintext.ToArray());
                    }
                }
            }
            return message;
        }

        //public Tuple<byte[], byte[]> GenerateKeyPair()
        //{

        //}
    }
}