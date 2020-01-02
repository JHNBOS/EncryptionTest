using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace TicketingAPI.Helpers
{
    public class RSA
    {
        /// <summary>
        ///     Encrypt provided text by using the given public key
        /// </summary>
        /// <param name="plain">Text to encrypt</param>
        /// <param name="publicKey">Public key to encrypt with</param>
        /// <param name="bytes">Bytes of the text to encrypt</param>
        /// <returns>Encrypted string</returns>
        public string Encrypt(string plain, string publicKey, byte[] bytes = null)
        {
            var rsaCryptoServiceProvider = new RSACryptoServiceProvider();
            var xmlSerializer = new XmlSerializer(typeof(RSAParameters), new XmlRootAttribute("RSAKeyValue"));

            // Get bytes from plain text
            if (!String.IsNullOrEmpty(plain))
            {
                bytes = Encoding.UTF8.GetBytes(plain);
            }
            else
            {
                throw new Exception("Invoer mag niet leeg of null zijn");
            }

            // Convert public key to xml
            var xml = PemToXml(publicKey);

            // Get public key parameters
            var publicKeyParameters = (RSAParameters)xmlSerializer.Deserialize(new StringReader(xml));
            rsaCryptoServiceProvider.ImportParameters(publicKeyParameters);

            // encrypt using PKCS#1.5 padding
            var encryptedBytes = rsaCryptoServiceProvider.Encrypt(bytes, false);
            var encryptedText = Convert.ToBase64String(encryptedBytes);

            return encryptedText;
        }

        /// <summary>
        ///     Decrypt provided encrypted text by using the given private key
        /// </summary>
        /// <param name="cipher">Cipher text to decrypt</param>
        /// <param name="privateKey">Private key to decrypt with</param>
        /// <returns>Encrypted string</returns>
        public string Decrypt(string cipher, string privateKey)
        {
            var rsaCryptoServiceProvider = new RSACryptoServiceProvider();
            var xmlSerializer = new XmlSerializer(typeof(RSAParameters), new XmlRootAttribute("RSAKeyValue"));

            // Get bytes from cipher
            var decodedBase64 = Convert.FromBase64String(cipher);
            if (decodedBase64.Length == 0)
            {
                throw new Exception("Invoer mag niet leeg of null zijn");
            }

            // Convert private key to xml
            var xml = PemToXml(privateKey);

            // Get private key parameters
            var privateKeyParameters = (RSAParameters)xmlSerializer.Deserialize(new StringReader(xml));
            rsaCryptoServiceProvider.ImportParameters(privateKeyParameters);

            // Decrypt using PKCS#1.5 padding
            var decodedBytes = rsaCryptoServiceProvider.Decrypt(decodedBase64, false);
            var decodedText = Encoding.UTF8.GetString(decodedBytes);

            return decodedText;
        }

        /// <summary>
        ///     Generate random public and private key
        /// </summary>
        /// <returns>Tuple with public and private key</returns>
        public Tuple<string, string> GenerateKeyPair()
        {
            RsaKeyPairGenerator keyGen = new RsaKeyPairGenerator();
            keyGen.Init(new KeyGenerationParameters(new SecureRandom(), 2048));

            var keys = keyGen.GenerateKeyPair();
            return new Tuple<string, string>(FormatPem(keys, false), FormatPem(keys, true));
        }

        #region Private Methods

        private static string FormatPem(AsymmetricCipherKeyPair keys, bool isPrivate)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            var pemWriter = new PemWriter(streamWriter);

            pemWriter.WriteObject(isPrivate == true ? keys.Private : keys.Public);
            streamWriter.Flush();

            string output = Encoding.ASCII.GetString(memoryStream.GetBuffer()).Trim();
            string footer = isPrivate == false ? "-----END PUBLIC KEY-----" : "-----END RSA PRIVATE KEY-----";
            int footerIndex = output.IndexOf(footer);

            memoryStream.Close();
            streamWriter.Close();

            if (isPrivate)
            {
                output = output.Substring(0, footerIndex + 29);
            }

            return output;
        }

        public static string PemToXml(string pem)
        {
            if (pem.StartsWith("-----BEGIN RSA PRIVATE KEY-----")
            || pem.StartsWith("-----BEGIN PRIVATE KEY-----"))
            {
                return GetXmlRsaKey(pem, obj =>
                {
                    if ((obj as RsaPrivateCrtKeyParameters) != null)
                        return DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters)obj);
                    var keyPair = (AsymmetricCipherKeyPair)obj;
                    return DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters)keyPair.Private);
                }, rsa => rsa.ToXmlString(true));
            }

            if (pem.StartsWith("-----BEGIN PUBLIC KEY-----"))
            {
                return GetXmlRsaKey(pem, obj =>
                {
                    var publicKey = (RsaKeyParameters)obj;
                    return DotNetUtilities.ToRSA(publicKey);
                }, rsa => rsa.ToXmlString(false));
            }

            return String.Empty;
        }

        private static string GetXmlRsaKey(string pem, Func<object, System.Security.Cryptography.RSA> getRsa, Func<System.Security.Cryptography.RSA, string> getKey)
        {
            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms))
            using (var sr = new StreamReader(ms))
            {
                sw.Write(pem);
                sw.Flush();
                ms.Position = 0;
                var pr = new PemReader(sr);
                object keyPair = pr.ReadObject();
                using (System.Security.Cryptography.RSA rsa = getRsa(keyPair))
                {
                    var xml = getKey(rsa);
                    return xml;
                }
            }
        }
        #endregion
    }
}
