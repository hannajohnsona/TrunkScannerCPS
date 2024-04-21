using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TrunkScannerCPS
{
    internal class SysKey
    {
        public string SysId { get; private set; }

        public SysKey(string sysId)
        {
            SysId = sysId;
        }

        public bool ValidateKeyFile(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine("Error: File does not exist.");
                return false;
            }

            string encryptedData = File.ReadAllText(filename);
            string decryptedData = DecryptString(encryptedData, GenerateKeyFromSysId());
            return decryptedData == SysId;
        }

        private string DecryptString(string cipherText, string key)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                encryptor.Padding = PaddingMode.PKCS7;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    return Encoding.Unicode.GetString(ms.ToArray());
                }
            }
        }

        private string GenerateKeyFromSysId()
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(this.SysId));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
