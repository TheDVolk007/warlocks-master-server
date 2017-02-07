namespace Dal.Common
{
    public interface IEncryptor
    {
        string DecryptStringFromBytes(byte[] cipherText);
        byte[] EncryptStringToBytes(string plainText);
        string SHA256(string source);
    }
}