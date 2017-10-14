// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheableIntegrationItemInfo.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the CacheableIntegrationItemInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.Cache
{
  using Sitecore.Data;
  using Sitecore.Reflection;

  /// <summary>
  /// Defines the properties and functionality of an object that is cacheablefor Item.
  /// </summary>
  public class CacheableIntegrationItemInfo : CacheableData
  {
    /// <summary>
    /// Gets the size of the data to use when caching.
    /// </summary>
    /// <returns>
    /// Size of the data
    /// </returns>
    public override long GetDataLength()
    {
      return base.GetDataLength() + TypeUtil.SizeOfID() /*+ TypeUtil.SizeOfID()*/ + TypeUtil.SizeOfString(SharepointItemId); //+ TypeUtil.SizeOfInt64();
    }

    /// <summary>
    /// Gets or sets Id of parent Item.
    /// </summary>
    public ID ParentItemId { get; set; }

    //public ID RootItemId { get; set; }

    /// <summary>
    /// Gets or sets Id of SharePoint item.
    /// </summary>
    public string SharepointItemId { get; set; }

    //public DateTime Modified { get; set; }
  }
}
