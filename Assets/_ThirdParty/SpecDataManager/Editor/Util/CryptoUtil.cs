/*
* Copyright (c) Sample.
*/

using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Sample.SpecData.Editor.Util
{
    internal static class CryptoUtil
    {
        public static byte[] EncryptAes128(string text, byte[] keyArray)
        {
            try
            {
                byte[] KeyArray = keyArray;
                byte[] EncryptArray = Encoding.UTF8.GetBytes(text);
                byte[] IKArray = keyArray.Reverse().ToArray();
                RijndaelManaged rijndaelManaged = new RijndaelManaged();
                rijndaelManaged.Mode = CipherMode.CBC;
                rijndaelManaged.Padding = PaddingMode.PKCS7;
                rijndaelManaged.Key = KeyArray;
                rijndaelManaged.IV = IKArray;
                ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
                byte[] bytes = cryptoTransform.TransformFinalBlock(EncryptArray, 0, EncryptArray.Length);
                return bytes;
            }
            catch
            {
                return null;
            }
        }

        public static byte[] DecryptAes128(byte[] encryptData, byte[] keyArray)
        {
            try
            {
                byte[] KeyArray = keyArray;
                byte[] IKArray = keyArray.Reverse().ToArray();
                RijndaelManaged rijndaelManaged = new RijndaelManaged();
                rijndaelManaged.Mode = CipherMode.CBC;
                rijndaelManaged.Padding = PaddingMode.PKCS7;
                rijndaelManaged.Key = KeyArray;
                rijndaelManaged.IV = IKArray;

                ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
                byte[] ResultArray = cryptoTransform.TransformFinalBlock(encryptData, 0, encryptData.Length);
                return ResultArray;
            }
            catch
            {
                return null;
            }
        }
    }
}
