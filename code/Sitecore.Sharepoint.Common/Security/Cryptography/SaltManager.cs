// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaltManager.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SaltManager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Security.Cryptography
{
  using System;
  using System.Security.Cryptography;
  using Sitecore.Diagnostics;

  public static class SaltManager
  {
    private const int MinAllowedSaltLength = 4;
    private const int MaxAllowedSaltLength = 255;

    [NotNull]
    public static byte[] GetSaltedValue([NotNull] byte[] originalValue)
    {
      Assert.ArgumentNotNull(originalValue, "originalValue");

      byte[] salt = GenerateSalt();

      var saltedValue = new byte[originalValue.Length + salt.Length];
      Array.Copy(salt, saltedValue, salt.Length);
      Array.Copy(originalValue, 0, saltedValue, salt.Length, originalValue.Length);

      return saltedValue;
    }

    [NotNull]
    public static byte[] GetOriginalValue([NotNull] byte[] saltedValue)
    {
      Assert.ArgumentNotNull(saltedValue, "saltedValue");

      int saltLength = RetrieveSaltLength(saltedValue);

      var originalValue = new byte[saltedValue.Length - saltLength];
      Array.Copy(saltedValue, saltLength, originalValue, 0, originalValue.Length);

      return originalValue;
    }

    [NotNull]
    private static byte[] GenerateSalt()
    {
      var salt = new byte[GenerateSaltLength()];
      new RNGCryptoServiceProvider().GetBytes(salt);

      return StoreSaltLength(salt);
    }

    private static int GenerateSaltLength()
    {
      var seedBytes = new byte[4];
      new RNGCryptoServiceProvider().GetBytes(seedBytes);

      int seed = BitConverter.ToInt32(seedBytes, 0);
      var random = new Random(seed);

      return random.Next(MinAllowedSaltLength, MaxAllowedSaltLength + 1);
    }

    [NotNull]
    private static byte[] StoreSaltLength([NotNull] byte[] salt)
    {
      Assert.ArgumentNotNull(salt, "salt");

      int saltLength = salt.Length;

      salt[0] = (byte)((salt[0] & 0xfc) | (saltLength & 0x03));
      salt[1] = (byte)((salt[1] & 0xf3) | (saltLength & 0x0c));
      salt[2] = (byte)((salt[2] & 0xcf) | (saltLength & 0x30));
      salt[3] = (byte)((salt[3] & 0x3f) | (saltLength & 0xc0));

      return salt;
    }

    private static int RetrieveSaltLength([NotNull] byte[] saltedValue)
    {
      Assert.ArgumentNotNull(saltedValue, "saltedValue");

      return (saltedValue[0] & 0x03) |
             (saltedValue[1] & 0x0c) |
             (saltedValue[2] & 0x30) |
             (saltedValue[3] & 0xc0);
    }
  }
}