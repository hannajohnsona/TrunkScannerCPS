using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace TrunkScannerCPS
{
    public static class CryptoUtils
    {
        public static string EncryptString(string text, string password)
        {
            using (Aes aes = Aes.Create())
            {
                byte[] iv = aes.IV;
                aes.Key = Encoding.UTF8.GetBytes(password.PadRight(32, '\0'));

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, iv);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(text);
                        }
                    }
                    byte[] encryptedData = memoryStream.ToArray();
                    byte[] combinedData = new byte[iv.Length + encryptedData.Length];
                    Array.Copy(iv, 0, combinedData, 0, iv.Length);
                    Array.Copy(encryptedData, 0, combinedData, iv.Length, encryptedData.Length);

                    return Convert.ToBase64String(combinedData);
                }
            }
        }

        public static string DecryptString(string cipherText, string password)
        {
            byte[] combinedData = Convert.FromBase64String(cipherText);
            byte[] iv = new byte[16];
            byte[] encryptedData = new byte[combinedData.Length - iv.Length];
            Array.Copy(combinedData, 0, iv, 0, iv.Length);
            Array.Copy(combinedData, iv.Length, encryptedData, 0, encryptedData.Length);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(password.PadRight(32, '\0'));
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(encryptedData))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
