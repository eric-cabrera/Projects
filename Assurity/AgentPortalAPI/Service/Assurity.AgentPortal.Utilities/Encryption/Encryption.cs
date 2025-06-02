namespace Assurity.AgentPortal.Utilities.Encryption;

using Assurity.AgentPortal.Contracts.PolicyDelivery.Request;
using Assurity.Common.Cryptography;
using Org.BouncyCastle.Tls.Crypto;

public class Encryption : IEncryption
{
    public Encryption(
        IAesEncryptor aesEncryptor)
    {
        AesEncryptor = aesEncryptor;
    }

    private IAesEncryptor AesEncryptor { get; set; }

    public string DecryptGAC(string stringToDecrypt, string environment, string sharedSecret)
    {
        return AesEncryptor.DecryptGAC(stringToDecrypt, environment, sharedSecret);
    }

    public string EncryptGAC(string stringToEncrypt, string environment, string sharedSecret)
    {
        return AesEncryptor.EncryptGAC(stringToEncrypt, environment, sharedSecret);
    }

    public string EncryptStringAes(string value, string secret, string salt)
    {
        return AesEncryptor.EncryptStringAes(value, secret, salt);
    }
}
