// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheableData.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the CacheableData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.Cache
{
  using System;
  using Sitecore.Caching;
  using Sitecore.Reflection;

  /// <summary>
  ///  Defines the basic properties and functionality of an object that is cacheable.
  /// </summary>
  public class CacheableData : ICacheable
  {
    /// <summary>
    /// Gets or sets expiration date.
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Gets a value indicating that item is expired.
    /// </summary>
    public bool IsExpired
    {
      get { return DateTime.Now > ExpirationDate; }
    }

    /// <summary>
    /// Gets the size of the data to use when caching.
    /// </summary>
    /// <returns>
    /// Size of the data
    /// </returns>
    public virtual long GetDataLength()
    {
      return TypeUtil.SizeOfInt64();
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is cacheable.
    /// </summary>
    public bool Cacheable
    {
      get { return true; }
      set { }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is immutable.
    /// </summary>
    public bool Immutable
    {
      get { return true; }
    }

    public event DataLengthChangedDelegate DataLengthChanged;
  }
}
