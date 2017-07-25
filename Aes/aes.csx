#reference System.Security.Cryptography.Csp

using System.IO;
using System.Security.Cryptography;
using Foopipes.Core.Extensions;

Task("aes.decryptstring").BinaryAsync(async (context, binary, ct)=>
{
    var key = Convert.FromBase64String(await context.GetExpandedConfigValueAsync("key", true));
    var iv = Convert.FromBase64String(await context.GetExpandedConfigValueAsync("iv", true));
    var decryptedData = DecryptStringFromBytes(binary.Data, key, iv);
    return new ProcessJsonResult(JObject.FromObject(new { value=decryptedData }));
});

Task("aes.decryptjson").BinaryAsync(async (context, binary, ct)=>
{
    var key = Convert.FromBase64String(await context.GetExpandedConfigValueAsync("key", true));
    var iv = Convert.FromBase64String(await context.GetExpandedConfigValueAsync("iv", true));
    var decryptedData = DecryptJsonFromBytes(binary.Data, key, iv);
    return new ProcessJsonResult(decryptedData);
});

Task("aes.decryptbinary").BinaryAsync(async (context, binary, ct)=>
{
    var key = Convert.FromBase64String(await context.GetExpandedConfigValueAsync("key", true));
    var iv = Convert.FromBase64String(await context.GetExpandedConfigValueAsync("iv", true));
    var decryptedData = DecryptBinaryFromBytes(binary.Data, key, iv);
    return new ProcessBinaryResult(decryptedData);
});

string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
{
    using (var aes = Aes.Create())
    {
        aes.Key = key;
        aes.IV = iv;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

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
    using (var aes = Aes.Create())
    {
        aes.Key = key;
        aes.IV = iv;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

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
    using (var aes = Aes.Create())
    {
        aes.Key = key;
        aes.IV = iv;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

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