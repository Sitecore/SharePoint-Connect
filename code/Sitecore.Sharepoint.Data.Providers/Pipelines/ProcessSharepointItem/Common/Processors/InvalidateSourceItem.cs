// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidateSourceItem.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the InvalidateSourceItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.ProcessSharepointItem
{
  using System.Linq;
  using Sitecore.Caching;
  using Sitecore.Caching.Generics;
  using Sitecore.Data;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Texts;
  
  public class InvalidateSourceItem
  {
    public virtual void Process([NotNull] ProcessSharepointItemArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.IsNotNull(args.SourceIntegrationItem, "args.SourceIntegrationItem");

      ItemCache itemCache = CacheManager.GetItemCache(args.SourceIntegrationItem.Database);
      if (itemCache != null)
      {
        itemCache.RemoveItem(args.SourceIntegrationItem.ID);
      }

      DataCache dataCache = CacheManager.GetDataCache(args.SourceIntegrationItem.Database);
      if (dataCache != null)
      {
        dataCache.RemoveItemInformation(args.SourceIntegrationItem.ID);
      }

      var prefetchCache = this.GetPrefetchCache(args.SourceIntegrationItem.Database);
      if (prefetchCache != null)
      {
        prefetchCache.Remove(args.SourceIntegrationItem.ID);
      }

      args.SourceIntegrationItem = args.SourceIntegrationItem.Database.GetItem(args.SourceIntegrationItem.ID);
      if (args.SourceIntegrationItem == null)
      {
        string message = string.Format(LogMessages.SitecoreItem0IsDeletedAndCouldNotBeSynchronizedWithSharepoint, args.SharepointItem.UniqueID);
        Log.Warn(message, this);

        args.AbortPipeline();
      }
    }

    private ICache<ID> GetPrefetchCache(Database database)
    {
      var firstOrDefault = CacheManager.GetAllCaches().FirstOrDefault(cache => cache.Name.Contains(string.Format("Prefetch data({0})", database.Name)));

      return firstOrDefault != null ? CacheManager.FindCacheByName<ID>(firstOrDefault.Name) : null;
    }
  }
}
