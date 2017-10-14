namespace Sitecore.Sharepoint.Data.Providers.Cache
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;

  using Sitecore.Caching;
  using Sitecore.Data;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.ObjectModel;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;

  /// <summary>
  /// Provides methods for caching Sitecore item.
  /// </summary>
  public class IntegrationCache : CustomCache
  {
    /// <summary>
    /// Prefix for adding folder item into cache.
    /// </summary>
    private const string FolderPrefix = "f";

    /// <summary>
    /// Prefix for adding integration item info into cache.
    /// </summary>
    private const string ItemPrefix = "i";

    /// <summary>
    /// Field with cashe for store expiration information.
    /// </summary>
    private static readonly IntegrationCache Instance = new IntegrationCache();

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationCache"/> class.
    /// </summary>
    protected IntegrationCache()
      : base("SharepointCache", 1024000)
    {
    }

    /// <summary>
    /// Remove items from cache with specified folder.
    /// </summary>
    /// <param name="itemID">
    /// The item id.
    /// </param>
    public static void RemoveIntegrationConfigData(ID itemID)
    {
      var keyListItems = Instance.InnerCache.GetCacheKeys().Where(x => x.StartsWith(ItemPrefix)).ToList();
      RemoveIntegrationConfigData(itemID, keyListItems);
    }

    private static void RemoveIntegrationConfigData(ID itemID, List<string> keyListItems)
    {
      foreach (var key in keyListItems)
      {
        CacheableIntegrationItemInfo cacheableIntegrationItemInfo = Instance.InnerCache.GetValue(key) as CacheableIntegrationItemInfo;
        if (cacheableIntegrationItemInfo != null && cacheableIntegrationItemInfo.ParentItemId == itemID)
        {
          ID id = new ID(key.Substring(ItemPrefix.Length));

          CacheableIntegrationConfigData cacheableIntegrationConfigData = GetIntegrationConfigData(id);
          if (cacheableIntegrationConfigData != null)
          {
            RemoveIntegrationConfigData(id, keyListItems);
            Instance.InnerCache.Remove(FolderPrefix + id);
          }

          Instance.InnerCache.Remove(key);
        }
      }

      Instance.InnerCache.Remove(FolderPrefix + itemID);
      Instance.InnerCache.Remove(ItemPrefix + itemID);
    }

    /// <summary>
    /// Add specified integration configuration data to the Cache.
    /// </summary>
    /// <param name="itemId">
    /// The item id.
    /// </param>
    /// <param name="integrationConfigData">
    /// The info.
    /// </param>
    /// <param name="expiredInSeconds">
    /// The expired in seconds.
    /// </param>
    public static void AddIntegrationConfigData(ID itemId, IntegrationConfigData integrationConfigData, ulong expiredInSeconds)
    {
      Instance.DoAddIntegrationConfigData(itemId, integrationConfigData, expiredInSeconds);
    }

    /// <summary>
    /// Get Item from cache.
    /// </summary>
    /// <param name="integrationItemID">
    /// The item id.
    /// </param>
    /// <returns>
    /// Item from cache.
    /// </returns>
    public static CacheableIntegrationItemInfo GetIntegrationItemInfo(ID integrationItemID)
    {
      return Instance.DoGetIntegrationItemInfo(integrationItemID);
    }

    /// <summary>
    /// Get integration configuration data from the cache.
    /// </summary>
    /// <param name="itemID">
    /// The item id.
    /// </param>
    /// <returns>
    /// Integration configuration data from the cache.
    /// </returns>
    public static CacheableIntegrationConfigData GetIntegrationConfigData(ID itemID)
    {
      return Instance.DoGetIntegrationConfigData(itemID);
    }

    /// <summary>
    /// Add Item to the cache.
    /// </summary>
    /// <param name="integrationItemID">
    /// The item id.
    /// </param>
    /// <param name="sharepointItem">
    /// The sp item.
    /// </param>
    /// <param name="parentItemId">
    /// The parent item id.
    /// </param>
    /// <param name="expiredInSeconds">
    /// The expired in seconds.
    /// </param>
    public static void AddIntegrationItemInfo(ID integrationItemID, BaseItem sharepointItem, ID parentItemId, ulong expiredInSeconds)
    {
      Instance.DoAddIntegrationItemInfo(integrationItemID, sharepointItem, parentItemId, expiredInSeconds);
    }

    /// <summary>
    /// Add specified integration configuration data to the Cache.
    /// </summary>
    /// <param name="itemId">
    /// The item id.
    /// </param>
    /// <param name="integrationConfigData">
    /// The info.
    /// </param>
    /// <param name="expiredInSeconds">
    /// The expired in seconds.
    /// </param>
    protected void DoAddIntegrationConfigData(ID itemId, IntegrationConfigData integrationConfigData, ulong expiredInSeconds)
    {
      SetObject(FolderPrefix + itemId,
                new CacheableIntegrationConfigData() { ExpirationDate = DateTime.Now.AddSeconds(expiredInSeconds), Data = integrationConfigData });
    }

    /// <summary>
    /// Add Item to the cache.
    /// </summary>
    /// <param name="integrationItemID">
    /// The item id.
    /// </param>
    /// <param name="sharepointItem">
    /// The sp item.
    /// </param>
    /// <param name="parentItemId">
    /// The parent item id.
    /// </param>
    /// <param name="expiredInSeconds">
    /// The expired in seconds.
    /// </param>
    protected void DoAddIntegrationItemInfo(ID integrationItemID, BaseItem sharepointItem, ID parentItemId, ulong expiredInSeconds)
    {
      SetObject(ItemPrefix + integrationItemID, new CacheableIntegrationItemInfo() { ExpirationDate = DateTime.Now.AddSeconds(expiredInSeconds), SharepointItemId = sharepointItem.UniqueID, ParentItemId = parentItemId });
    }

    /// <summary>
    /// Get Item from cache.
    /// </summary>
    /// <param name="integrationItemID">
    /// The item id.
    /// </param>
    /// <returns>
    /// Item from cache.
    /// </returns>
    protected CacheableIntegrationItemInfo DoGetIntegrationItemInfo(ID integrationItemID)
    {
      CacheableIntegrationItemInfo info = GetObject(ItemPrefix + integrationItemID) as CacheableIntegrationItemInfo;
      return info;
    }

    /// <summary>
    /// Get integration configuration data from the cache.
    /// </summary>
    /// <param name="itemID">
    /// The item id.
    /// </param>
    /// <returns>
    /// Integration configuration data from the cache.
    /// </returns>
    protected CacheableIntegrationConfigData DoGetIntegrationConfigData(ID itemID)
    {
      CacheableIntegrationConfigData info = GetObject(FolderPrefix + itemID) as CacheableIntegrationConfigData;
      return info;
    }

  }
}
