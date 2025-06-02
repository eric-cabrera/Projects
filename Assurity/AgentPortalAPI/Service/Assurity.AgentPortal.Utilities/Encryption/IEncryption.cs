namespace Assurity.AgentPortal.Utilities.Encryption
{
    public interface IEncryption
    {
        string EncryptStringAes(string value, string secret, string salt);

        string EncryptGAC(string stringToEncrypt, string environment, string sharedSecret);

        string DecryptGAC(string stringToDecrypt, string environment, string sharedSecret);
    }
}
