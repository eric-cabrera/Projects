namespace Assurity.AgentPortal.Utilities.Encryption
{
    /// <summary>
    /// Provides methods for encrypting and decrypting data.
    /// </summary>
    public interface IEncryption
    {
        /// <summary>
        /// Encrypts a given plaintext string using a secure encryption method.
        /// </summary>
        /// <param name="textToEncrypt">The plaintext string to be encrypted.</param>
        /// <returns>Encrypted string in base64 format.</returns>
        string Encrypt(string textToEncrypt);

        /// <summary>
        /// Decrypts a given encrypted string back to its original plaintext form.
        /// </summary>
        /// <param name="textToDecrypt">The encrypted string to be decrypted.</param>
        /// <returns>The original plaintext string after decryption.</returns>
        string Decrypt(string textToDecrypt);
    }
}
