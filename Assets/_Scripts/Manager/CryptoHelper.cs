using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
public class CryptoHelper : MonoBehaviour
{

    private static readonly string cryptoCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private static readonly string key = "fghdesjkhguerhgjndfkngKLJDFHJhdsf"; // Replace with your key

    public static string EncryptString(string data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        string base64String = Convert.ToBase64String(bytes);
        return EncryptBase64String(base64String);
    }

    private static string EncryptBase64String(string plainText)
    {
        char[] buffer = plainText.ToCharArray();
        for (int i = 0; i < buffer.Length; i++)
        {
            char c = buffer[i];
            if (cryptoCharacters.Contains(c))
            {
                int shift = key[i % key.Length];
                int value = (cryptoCharacters.IndexOf(c) + cryptoCharacters.IndexOf((char)shift));
                if (value >= cryptoCharacters.Length)
                {
                    value -= cryptoCharacters.Length;
                }
                buffer[i] = cryptoCharacters[value];
            }
        }
        return new string(buffer);
    }

    public static string DecryptString(string encryptedData)
    {
        string base64Decoded = DecryptBase64String(encryptedData);
        byte[] bytes = Convert.FromBase64String(base64Decoded);
        return Encoding.UTF8.GetString(bytes);
    }

    private static string DecryptBase64String(string encryptedText)
    {
        char[] buffer = encryptedText.ToCharArray();
        for (int i = 0; i < buffer.Length; i++)
        {
            char c = buffer[i];
            if (cryptoCharacters.Contains(c))
            {
                int shift = key[i % key.Length];
                int value = (cryptoCharacters.IndexOf(c) - cryptoCharacters.IndexOf((char)shift));
                if (value < 0)
                {
                    value += cryptoCharacters.Length;
                }
                buffer[i] = cryptoCharacters[value];
            }
        }
        return new string(buffer);
    }

    public static void Main()
    {
        string original = "HelloWorld123";
        string encrypted = EncryptString(original);
        string decrypted = DecryptString(encrypted);

        Console.WriteLine($"Original: {original}");
        Console.WriteLine($"Encrypted: {encrypted}");
        Console.WriteLine($"Decrypted: {decrypted}");
    }
}
