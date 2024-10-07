using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Sample.SpecData.Generator;
using UnityEngine;

public static partial class SpecDataResourceLoader
{
    public static string LoadSpecData()
    {
        var byteKey = _key.Clone() as byte[];
        for (byte i = 0; i < byteKey!.Length; i++)
        {
            byte b = byteKey[i];
            int v = b ^ i;
            byteKey[i] = (byte)v;
        }

        byte[] path =
        {
            83,
            112,
            101,
            99,
            68,
            97,
            116,
            97
        }; // SpecData
        var data = Resources.Load<TextAsset>(Encoding.UTF8.GetString(path));
        if (data == default)
        {
            Resources.UnloadAsset(data);
            return default;
        }

        var bytes = data.bytes;
        byte[] dataArray;
        try
        {
            byte[] keyArray1 = byteKey;
            byte[] ikArray = byteKey.Reverse().ToArray();
            RijndaelManaged rijndaelManaged1 = new RijndaelManaged();
            rijndaelManaged1.Mode = CipherMode.CBC;
            rijndaelManaged1.Padding = PaddingMode.PKCS7;
            rijndaelManaged1.Key = keyArray1;
            rijndaelManaged1.IV = ikArray;
            ICryptoTransform cryptoTransform1 = rijndaelManaged1.CreateDecryptor();
            byte[] resultArray = cryptoTransform1.TransformFinalBlock(bytes, 0, bytes.Length);
            dataArray = resultArray;
        }
        catch
        {
            dataArray = default;
        }

        Resources.UnloadAsset(data);
        return dataArray == default ? default : Encoding.UTF8.GetString(dataArray);
    }
}
