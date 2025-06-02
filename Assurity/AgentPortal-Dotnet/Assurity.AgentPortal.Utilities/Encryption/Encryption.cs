namespace Assurity.AgentPortal.Utilities.Encryption
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Provides methods for encrypting and decrypting data.
    /// This encryption implementation was added to ensure compatibility with a legacy application
    /// written in an older version of .NET. The existing package
    /// <see cref="PackageReference Include='Assurity.Common.Cryptography' Version='1.0.22209.1'"/> was not compatible with the legacy version,
    /// so this implementation was introduced to facilitate secure data exchange between newer and legacy systems.
    /// </summary>
    public class Encryption : IEncryption
    {
        /// <summary>
        /// Encrypts a given text using DES encryption.
        /// </summary>
        /// <param name="textToEncrypt">The plaintext string to be encrypted.</param>
        /// <returns>Encrypted string in base64 format.</returns>
        /// <exception cref="Exception">Thrown when encryption fails.</exception>
        public string Encrypt(string textToEncrypt)
        {
            try
            {
                string toReturn = string.Empty;
                string publickey = "pdrGxgau";
                string secretkey = "qRaPSAv1";
                byte[] secretkeyByte = { };
                secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    toReturn = Convert.ToBase64String(ms.ToArray());
                }

                return toReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Decrypts a given encrypted text back to its original form using DES decryption.
        /// </summary>
        /// <param name="textToDecrypt">The encrypted string to be decrypted.</param>
        /// <returns>Decrypted string (original plaintext).</returns>
        /// <exception cref="Exception">Thrown when decryption fails.</exception>
        public string Decrypt(string textToDecrypt)
        {
            try
            {
                string toReturn = string.Empty;
                string publickey = "pdrGxgau";
                string privatekey = "qRaPSAv1";

                byte[] privatekeyByte = { };
                privatekeyByte = System.Text.Encoding.UTF8.GetBytes(privatekey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    toReturn = encoding.GetString(ms.ToArray());
                }

                return toReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }
        }
    }
}
