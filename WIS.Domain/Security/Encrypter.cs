using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace WIS.Domain.Security
{
    public class Encrypter
    {
        public static string Encrypt(string plainText, out string salt, out int format)
        {
            var info = EncryptStringToBytes(plainText);
            var cipherTextBytes = info[0];
            var keyBytes = info[1];
            var ivBytes = info[2];
            var saltBytes = new byte[keyBytes.Length + ivBytes.Length];

            Buffer.BlockCopy(keyBytes, 0, saltBytes, 0, keyBytes.Length);
            Buffer.BlockCopy(ivBytes, 0, saltBytes, keyBytes.Length, ivBytes.Length);

            salt = Convert.ToBase64String(saltBytes);
            format = keyBytes.Length + (cipherTextBytes.Count(c => c >= 0 && c <= 9) % 7);

            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(string cipherText, string salt, int format)
        {
            var cipherTextBytes = Convert.FromBase64String(cipherText);
            var saltBytes = Convert.FromBase64String(salt);
            var keyBytes = new byte[format - (cipherTextBytes.Count(c => c >= 0 && c <= 9) % 7)];
            var ivBytes = new byte[saltBytes.Length - keyBytes.Length];

            Buffer.BlockCopy(saltBytes, 0, keyBytes, 0, keyBytes.Length);
            Buffer.BlockCopy(saltBytes, keyBytes.Length, ivBytes, 0, ivBytes.Length);

            return DecryptStringFromBytes(cipherTextBytes, keyBytes, ivBytes);
        }

        public static byte[][] EncryptStringToBytes(string plainText)
        {
            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                myRijndael.GenerateKey();
                myRijndael.GenerateIV();

                byte[] Key = myRijndael.Key;
                byte[] IV = myRijndael.IV;
                byte[] cipherText = EncryptStringToBytes(plainText, myRijndael.Key, myRijndael.IV);

                return new byte[][] { cipherText, Key, IV };
            }
        }

        public static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            byte[] encrypted;

            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        public static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
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
    }
}
