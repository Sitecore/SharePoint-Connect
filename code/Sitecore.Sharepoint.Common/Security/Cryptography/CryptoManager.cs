// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CryptoManager.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the CryptoManager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Security.Cryptography
{
  using System;
  using System.IO;
  using System.Security.Cryptography;
  using System.Text;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Configuration;

  public static class CryptoManager
  {
    [NotNull]
    private static byte[] DefaultInitializationVector
    {
      get
      {
        return Convert.FromBase64String(Settings.InitializationVector);
      }
    }

    [NotNull]
    public static string Encrypt([NotNull] string targetValue, [NotNull] string key)
    {
      Assert.ArgumentNotNull(targetValue, "targetValue");
      Assert.ArgumentNotNullOrEmpty(key, "key");

      return Convert.ToBase64String(Encrypt(
                                            Encoding.UTF8.GetBytes(targetValue),
                                            Convert.FromBase64String(key)));
    }

    [NotNull]
    public static byte[] Encrypt([NotNull] byte[] targetValue, [NotNull] byte[] key)
    {
      Assert.ArgumentNotNull(targetValue, "targetValue");
      Assert.ArgumentNotNull(key, "key");

      return Encrypt(targetValue, key, DefaultInitializationVector);
    }

    [NotNull]
    public static string Encrypt([NotNull] string targetValue, [NotNull] string key, [NotNull] string iv)
    {
      Assert.ArgumentNotNull(targetValue, "targetValue");
      Assert.ArgumentNotNullOrEmpty(key, "key");
      Assert.ArgumentNotNullOrEmpty(iv, "iv");

      return Convert.ToBase64String(Encrypt(
                                            Encoding.UTF8.GetBytes(targetValue),
                                            Convert.FromBase64String(key),
                                            Convert.FromBase64String(iv)));
    }

    [NotNull]
    public static byte[] Encrypt([NotNull] byte[] targetValue, [NotNull] byte[] key, [NotNull] byte[] iv)
    {
      Assert.ArgumentNotNull(targetValue, "targetValue");
      Assert.ArgumentNotNull(key, "key");
      Assert.ArgumentNotNull(iv, "iv");

      byte[] saltedTargetValue = SaltManager.GetSaltedValue(targetValue);

      using (var rijndaelManaged = new RijndaelManaged())
      {
        ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor(key, iv);

        using (var memoryStream = new MemoryStream())
        {
          using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
          {
            cryptoStream.Write(saltedTargetValue, 0, saltedTargetValue.Length);
          }

          return memoryStream.ToArray();
        }
      }
    }

    [NotNull]
    public static string Decrypt([NotNull] string targetValue, [NotNull] string key)
    {
      Assert.ArgumentNotNull(targetValue, "targetValue");
      Assert.ArgumentNotNullOrEmpty(key, "key");

      return Encoding.UTF8.GetString(Decrypt(
                                             Convert.FromBase64String(targetValue),
                                             Convert.FromBase64String(key)));
    }

    [NotNull]
    public static byte[] Decrypt([NotNull] byte[] targetValue, [NotNull] byte[] key)
    {
      Assert.ArgumentNotNull(targetValue, "targetValue");
      Assert.ArgumentNotNull(key, "key");

      return Decrypt(targetValue, key, DefaultInitializationVector);
    }

    [NotNull]
    public static string Decrypt([NotNull] string targetValue, [NotNull] string key, [NotNull] string iv)
    {
      Assert.ArgumentNotNull(targetValue, "targetValue");
      Assert.ArgumentNotNullOrEmpty(key, "key");
      Assert.ArgumentNotNullOrEmpty(iv, "iv");

      return Encoding.UTF8.GetString(Decrypt(
                                             Convert.FromBase64String(targetValue),
                                             Convert.FromBase64String(key),
                                             Convert.FromBase64String(iv)));
    }

    [NotNull]
    public static byte[] Decrypt([NotNull] byte[] targetValue, [NotNull] byte[] key, [NotNull] byte[] iv)
    {
      Assert.ArgumentNotNull(targetValue, "targetValue");
      Assert.ArgumentNotNull(key, "key");
      Assert.ArgumentNotNull(iv, "iv");

      using (var rijndaelManaged = new RijndaelManaged())
      {
        ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(key, iv);

        using (var memoryStream = new MemoryStream())
        {
          using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
          {
            cryptoStream.Write(targetValue, 0, targetValue.Length);
          }

          return SaltManager.GetOriginalValue(memoryStream.ToArray());
        }
      }
    }

    [NotNull]
    public static byte[] GenerateKey()
    {
      using (var rijndaelManaged = new RijndaelManaged())
      {
        return rijndaelManaged.Key;
      }
    }
  }
}