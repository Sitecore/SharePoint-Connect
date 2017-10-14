// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemProvider.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ItemProvider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers
{
  using Sitecore.Collections;
  using Sitecore.Common;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
#if Intranet
  using Sitecore.Configuration;
  using Sitecore.Intranet.DraftMode;
  using Sitecore.Intranet.DraftMode.Abstractions;
#endif
  using Sitecore.SecurityModel;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.Options;

  /// <summary>
  /// Defines the provider for performing basic CRUD operations against the database that stores items.
  /// Provides possibility to integrate SharePoint data into Sitecore.
  /// </summary>
#if Intranet
  public class ItemProvider : DraftModeItemProvider
#else 
  public class ItemProvider : Sitecore.Data.Managers.ItemProvider
#endif
  {
#if Intranet

    public ItemProvider(IDraftMode draftMode, BaseSettings settings): base(draftMode, settings)
    {
    }

#endif
    /// <summary>
    /// Gets the children of an item. 
    /// If the parent item is integration configuration or folder then synchronize it with appropriate SharePoint source previously.
    /// </summary>
    /// <param name="item">The parent item.</param>
    /// <param name="securityCheck">The security check.</param>
    /// <returns>The child items.</returns>
    [NotNull]
    public override ChildList GetChildren([NotNull] Item item, SecurityCheck securityCheck)
    {
      Assert.ArgumentNotNull(item, "item");

      if (!IntegrationDisabler.CurrentValue)
      {
        if (SharepointProvider.IsActiveIntegrationConfigItem(item) || SharepointProvider.IsActiveIntegrationFolder(item))
        {
          SharepointProvider.ProcessTree(item, ProcessIntegrationItemsOptions.DefaultAsyncOptions);
        }
      }

      using (new IntegrationDisabler())
      {
        return base.GetChildren(item, securityCheck);
      }
    }

    /// <summary>
    /// Gets an item from a database.
    /// If the item is integration folder or item then synchronize it with SharePoint source.
    /// </summary>
    /// <param name="itemId">The item id.</param>
    /// <param name="language">The language of the item to get.</param>
    /// <param name="version">The version of the item to get.</param>
    /// <param name="database">The database.</param>
    /// <returns>
    /// The item. If no item is found, <c>null</c> is returned.
    /// </returns>
    [CanBeNull]
    protected override Item GetItem([NotNull] ID itemId, [NotNull] Language language, [NotNull] Version version, [NotNull] Database database)
    {
      Assert.ArgumentNotNull(itemId, "itemId");
      Assert.ArgumentNotNull(language, "language");
      Assert.ArgumentNotNull(version, "version");
      Assert.ArgumentNotNull(database, "database");

      Item item = base.GetItem(itemId, language, version, database);

      if (IntegrationDisabler.CurrentValue || item == null)
      {
        return item;
      }

      if (SharepointProvider.IsActiveIntegrationFolder(item) || SharepointProvider.IsActiveIntegrationDataItem(item))
      {
        SharepointProvider.ProcessItem(item, ProcessIntegrationItemsOptions.DefaultAsyncOptions);
      }

      return item;
    }
  }
}
