#r "System.Security.Cryptography.Csp"

using System.IO;
using System.Security.Cryptography;
using Foopipes.Core.Extensions;

var _aes = Aes.Create();

Task("aes.decryptstring").Binary(async (context, binary, ct)=>
{
    var key = Convert.FromBase64String(await context.GetExpandedConfigValue("key", true));
    var iv = Convert.FromBase64String(await context.GetExpandedConfigValue("iv", true));
    var decryptedData = DecryptStringFromBytes(binary.Data, key, iv);
    return JObject.FromObject(new { value=decryptedData });
});

Task("aes.decryptjson").Binary(async (context, binary, ct)=>
{
    var key = Convert.FromBase64String(await context.GetExpandedConfigValue("key", true));
    var iv = Convert.FromBase64String(await context.GetExpandedConfigValue("iv", true));
    var decryptedData = DecryptJsonFromBytes(binary.Data, key, iv);
    return decryptedData;
});

Task("aes.decryptbinary").Binary(async (context, binary, ct)=>
{
    var key = Convert.FromBase64String(await context.GetExpandedConfigValue("key", true));
    var iv = Convert.FromBase64String(await context.GetExpandedConfigValue("iv", true));
    var decryptedData = DecryptBinaryFromBytes(binary.Data, key, iv);
    return decryptedData;
});

string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
{
    using (var decryptor = _aes.CreateDecryptor(key, iv))
    {
        using (var msDecrypt = new MemoryStream(cipherText))
        {
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}

JObject[] DecryptJsonFromBytes(byte[] cipherText, byte[] key, byte[] iv)
{
    using (var decryptor = _aes.CreateDecryptor(key, iv))
    {
        using (var msDecrypt = new MemoryStream(cipherText))
        {
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                return csDecrypt.ReadJson();
            }
        }
    }
}

byte[] DecryptBinaryFromBytes(byte[] cipherText, byte[] key, byte[] iv)
{
    using (var decryptor = _aes.CreateDecryptor(key, iv))
    {
        using (var msDecrypt = new MemoryStream(cipherText))
        {
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                using (var resultStream = new MemoryStream())
                {
                    csDecrypt.CopyTo(resultStream);
                    return resultStream.ToArray();
                }
            }
        }
    }
}