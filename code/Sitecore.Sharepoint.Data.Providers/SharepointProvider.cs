// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharepointProvider.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines SharepointProvider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Text;
  using System.Web.Services.Protocols;

  using Microsoft.Extensions.DependencyInjection;

  using Sitecore.Abstractions;
  using Sitecore.Collections;
  using Sitecore.Common;
  using Sitecore.Data;
  using Sitecore.Data.Events;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Data.Templates;
  using Sitecore.DependencyInjection;
  using Sitecore.Diagnostics;
  using Sitecore.Eventing;
  using Sitecore.Events;
  using Sitecore.SecurityModel;
  using Sitecore.Sharepoint.Common.Configuration;
  using Sitecore.Sharepoint.Common.Utils;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.Logging;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Sharepoint.Data.Providers.Pipelines;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.ProcessIntegrationItem.Common;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.Runners;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;
  using Sitecore.Sharepoint.ObjectModel;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;
  using Sitecore.Sharepoint.ObjectModel.Options;
  using Sitecore.Sharepoint.Pipelines.ProcessSharepointItem;
  using SharepointBaseItem = Sitecore.Sharepoint.ObjectModel.Entities.Items.BaseItem;
  using SharepointFieldIDs = Sitecore.Sharepoint.Common.FieldIDs;
  using SharepointTemplateIDs = Sitecore.Sharepoint.Common.TemplateIDs;

  /// <summary>
  /// Populates Sitecore tree by Sharepoint data.
  /// </summary>
  public class SharepointProvider
  {
    // private const int ErrorTimeout = 30;

    /// <summary>
    /// Default value for time expiration for cache.
    /// </summary>
    public const ulong DefaultTimeout = 3600;

    /// <summary>
    /// Instance of SharepointProvider class.(Singleton patern)
    /// </summary>
    private static SharepointProvider instance;

    /// <summary>
    /// The factory.
    /// </summary>
    private static BaseFactory factory;

    /// <summary>
    /// Gets Instance of SharepointProvider.
    /// </summary>
    [NotNull]
    public static SharepointProvider Instance
    {
      get
      {
        return instance ?? (instance = new SharepointProvider());
      }
    }

    /// <summary>
    /// Gets or sets the factory.
    /// </summary>
    internal static BaseFactory Factory
    {
      get
      {
        return factory ?? (factory = ServiceLocator.ServiceProvider.GetRequiredService<BaseFactory>());
      }

      set
      {
        factory = value;
      }
    }

    static SharepointProvider()
    {
      EventManager.Subscribe<ProcessTreeRemoteEvent>(OnProcessTreeRemoteEvent);
    }

    /// <summary>
    /// Called when [generic remote event].
    /// </summary>
    /// <typeparam name="TEvent">
    /// The type of the event.
    /// </typeparam>
    /// <param name="processTreeRemoteEvent">
    /// The @event.
    /// </param>
    private static void OnProcessTreeRemoteEvent(ProcessTreeRemoteEvent processTreeRemoteEvent)
    {
      var item = Factory.GetDatabase(processTreeRemoteEvent.DatabaseName).GetItem(processTreeRemoteEvent.Id);
      if (item == null)
      {
        return;
      }

      if (IsActiveIntegrationConfigItem(item) || IsActiveIntegrationFolder(item))
      {
        var options = ProcessIntegrationItemsOptions.DefaultAsyncOptions;
        options.IsEvent = true;
        ProcessTree(item, options);
      }
    }

    [CanBeNull]
    public static SharepointBaseItem CreateSharepointItem([NotNull] string itemName, [NotNull] Item destination, [NotNull] ID templateId)
    {
      Assert.ArgumentNotNull(itemName, "itemName");
      Assert.ArgumentNotNull(destination, "destination");
      Assert.ArgumentNotNull(templateId, "templateId");

      var synchContext = new SynchContext(destination);

      ProcessSharepointItemArgs pipelineArgs = IntegrationPipelinesRunner.CreateSharepointItem(itemName, templateId, synchContext);

      return pipelineArgs != null ? pipelineArgs.SharepointItem : null;
    }

    public static bool UpdateSharepointItem([NotNull] Item sourceIntegrationItem)
    {
      Assert.ArgumentNotNull(sourceIntegrationItem, "sourceIntegrationItem");

      CacheableIntegrationItemInfo cacheableIntegrationItemInfo = IntegrationCache.GetIntegrationItemInfo(sourceIntegrationItem.ID);
      if (cacheableIntegrationItemInfo == null)
      {
        Log.Warn("Can't find item in the Sharepoint cache and so can't update Sharepoint item. ", sourceIntegrationItem);
        return false;
      }

      var synchContext = new SynchContext(cacheableIntegrationItemInfo.ParentItemId, sourceIntegrationItem.Database);

      ProcessSharepointItemArgs pipelineArgs = IntegrationPipelinesRunner.UpdateSharepointItem(
                                                                                               cacheableIntegrationItemInfo.SharepointItemId,
                                                                                               sourceIntegrationItem,
                                                                                               synchContext);

      return pipelineArgs != null;
    }

    [CanBeNull]
    [Obsolete("It is never used and will be removed in future versions.")]
    public static Item CreateIntegrationItem([NotNull] string itemName, [NotNull] Item destination, [NotNull] ID templateId, [NotNull] ID newId)
    {
      Assert.ArgumentNotNull(itemName, "itemName");
      Assert.ArgumentNotNull(destination, "destination");
      Assert.ArgumentNotNull(templateId, "templateId");
      Assert.ArgumentNotNull(newId, "newId");
      
      if (!IntegrationDisabler.CurrentValue &&
          (IsActiveIntegrationConfigItem(destination) ||
          IsActiveIntegrationFolder(destination)))
      {
        using (new IntegrationDisabler())
        {
          return ItemManager.AddFromTemplate(itemName, templateId, destination, newId);
        }
      }

      return null;
    }

    /// <summary>
    /// Methos is deleting item in SharePoint.
    /// </summary>
    /// <param name="sharepointItemUniqueID">The sharepoint unique id.</param>
    /// <param name="integrationConfigDataSource">The item which contains integration config data for the target intergation item.</param>
    public static bool DeleteSharepointItem([NotNull] string sharepointItemUniqueID, [NotNull] Item integrationConfigDataSource)
    {
      Assert.ArgumentNotNull(sharepointItemUniqueID, "sharepointItemUniqueID");
      Assert.ArgumentNotNull(integrationConfigDataSource, "integrationConfigDataSource");

      var synchContext = new SynchContext(integrationConfigDataSource);

      ProcessSharepointItemArgs pipelineArgs = IntegrationPipelinesRunner.DeleteSharepointItem(sharepointItemUniqueID, synchContext);

      return pipelineArgs != null;
    }

    /// <summary>
    /// Method is processing item with use IBehaviour.
    /// </summary>
    /// <param name="item">The item for processing.</param>
    /// <param name="processIntegrationItemsOptions">The process integration items options.</param>
    public static void ProcessItem([NotNull] Item item, [NotNull] ProcessIntegrationItemsOptions processIntegrationItemsOptions)
    {
      Assert.ArgumentNotNull(item, "item");
      Assert.ArgumentNotNull(processIntegrationItemsOptions, "processIntegrationItemsOptions");

      if (item.Parent != null)
      {
        ProcessTree(item.Parent, processIntegrationItemsOptions);
      }
    }

    /// <summary>
    /// Method is processing integration config or integration folder item.
    /// </summary>
    /// <param name="integrationConfigDataSource">The item which contains integration config data.</param>
    /// <param name="processIntegrationItemsOptions">The process integration items options.</param>
    public static void ProcessTree([NotNull] Item integrationConfigDataSource, [NotNull] ProcessIntegrationItemsOptions processIntegrationItemsOptions)
    {
      Assert.ArgumentNotNull(integrationConfigDataSource, "integrationConfigDataSource");
      Assert.ArgumentNotNull(processIntegrationItemsOptions, "processIntegrationItemsOptions");

      CacheableIntegrationConfigData it = IntegrationCache.GetIntegrationConfigData(integrationConfigDataSource.ID);
      if (!IntegrationDisabler.CurrentValue && (processIntegrationItemsOptions.Force || it == null || it.IsExpired))
      {
        if (processIntegrationItemsOptions.AsyncIntegration)
        {
          string jobName = string.Format("SharePoint_Integration_{0}", integrationConfigDataSource.ID);
          var parameters = new object[]
          {
            processIntegrationItemsOptions, integrationConfigDataSource
          };

          JobUtil.StartJob(jobName, Instance, "ProcessTree", parameters);
        }
        else
        {
          Instance.ProcessTree(processIntegrationItemsOptions, integrationConfigDataSource);
        }
      }
    }

    /// <summary>
    /// Method is processing integration config or integration folder item.
    /// </summary>
    /// <param name="processIntegrationItemsOptions">The process integration items options.</param>
    /// <param name="integrationConfigDataSource">The item which contains integration config data.</param>
    public virtual void ProcessTree([NotNull] ProcessIntegrationItemsOptions processIntegrationItemsOptions, [NotNull] Item integrationConfigDataSource)
    {
      Assert.ArgumentNotNull(processIntegrationItemsOptions, "processIntegrationItemsOptions");
      Assert.ArgumentNotNull(integrationConfigDataSource, "integrationConfigDataSource");

      CacheableIntegrationConfigData it = IntegrationCache.GetIntegrationConfigData(integrationConfigDataSource.ID);
      if (!IntegrationDisabler.CurrentValue && (processIntegrationItemsOptions.Force || it == null || it.IsExpired))
      {
        using (new IntegrationDisabler())
        {
          SynchContext synchContext;
          try
          {
            synchContext = new SynchContext(integrationConfigDataSource);
          }
          catch (Exception ex)
          {
            Log.Error(string.Format("Synchronization context can not be created for \"{0}\" integration config data source item.", integrationConfigDataSource.Name), ex, this);
            return;
          }

          IntegrationPipelinesRunner.SynchronizeTree(processIntegrationItemsOptions, synchContext);
        }
      }
    }

    /// <summary>
    /// Method is processing integration config or integration folder item form SynchContext.
    /// </summary>
    /// <param name="processIntegrationItemsOptions">The process integration items options.</param>
    /// <param name="synchContext">The synchronization context.</param>
    [Obsolete("It is never used and will be removed in future versions.")]
    protected virtual void ProcessTree([NotNull] ProcessIntegrationItemsOptions processIntegrationItemsOptions, [NotNull] SynchContext synchContext)
    {
      Assert.ArgumentNotNull(processIntegrationItemsOptions, "processIntegrationItemsOptions");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      IntegrationCache.AddIntegrationConfigData(synchContext.ParentID, synchContext.IntegrationConfigData, (synchContext.IntegrationConfigData.ExpirationInterval > 0) ? synchContext.IntegrationConfigData.ExpirationInterval : DefaultTimeout);

      // IDList existenChildrenList = new IDList();
      // foreach (Item it in SynchContext.ParentItem.GetChildren())
      // {
      //   existenChildrenList.Add(it.ID);
      // }
      IDList existenChildrenList = synchContext.Database.DataManager.DataSource.GetChildIDs(synchContext.ParentID);

      var existenList = new List<ID>(existenChildrenList.Cast<ID>());
      foreach (SharepointBaseItem listItem in this.GetSubItems(this.GetList(synchContext.IntegrationConfigData), synchContext.IntegrationConfigData))
      {
        ID id = Utils.GetID(synchContext.ParentID.ToString(), listItem.UniqueID);

        bool itemExist = existenList.IndexOf(id) != -1;

        if (Switcher<bool, SynchDisabler>.CurrentValue == false)
        {
          if (itemExist)
          {
            if (IsActiveIntegrationDataItem(id, synchContext.Database))
            {
              IntegrationPipelinesRunner.UpdateIntegrationItem(id, listItem, synchContext, processIntegrationItemsOptions, EventSender.Sharepoint);
            }

            existenChildrenList.Remove(id);
          }
          else
          {
            IntegrationPipelinesRunner.CreateIntegrationItem(id, listItem, synchContext, processIntegrationItemsOptions, EventSender.Sharepoint);
          }
        }

        if (listItem is FolderItem)
        {
          if (IsActiveIntegrationDataItem(id, synchContext.Database))
          {
            var newSynchContext = new SynchContext(id, synchContext.Database);
            if (processIntegrationItemsOptions.Recursive)
            {
              ProcessTree(processIntegrationItemsOptions, newSynchContext);
            }
          }
        }

        IntegrationCache.AddIntegrationItemInfo(id, listItem, synchContext.ParentID, (synchContext.IntegrationConfigData.ExpirationInterval > 0) ? synchContext.IntegrationConfigData.ExpirationInterval : DefaultTimeout);
      }

      if (Switcher<bool, SynchDisabler>.CurrentValue == false)
      {
        foreach (ID id in existenChildrenList)
        {
          if (IsActiveIntegrationDataItem(id, synchContext.Database))
          {
            IntegrationPipelinesRunner.DeleteIntegrationItem(id, synchContext);
          }
        }
      }
    }

    /// <summary>
    /// Get items from SharePoint using information from SharePointSystemData.
    /// </summary>
    /// <param name="list">The SharePoint list.</param>
    /// <param name="integrationConfigData">The data for receiving items from SharePoint.</param>
    /// <returns>Items from SharePoint.</returns>
    [NotNull]
    protected ObjectModel.Entities.Collections.ItemCollection GetSubItems([NotNull] BaseList list, [NotNull] IntegrationConfigData integrationConfigData)
    {
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(integrationConfigData, "integrationConfigData");

      var itemsOptions = new ItemsRetrievingOptions
      {
        Folder = integrationConfigData.Folder,
        ViewName = integrationConfigData.View,
        ItemLimit = integrationConfigData.ItemLimit
      };

      return list.GetItems(itemsOptions);
    }

    /// <summary>
    /// Get List from SharePoint.
    /// </summary>
    /// <param name="integrationConfigData">The share point system data.</param>
    /// <returns>List from Sharepoint.</returns>
    [NotNull]
    [Obsolete("It is never used and will be removed in future versions.")]
    protected BaseList GetList([NotNull] IntegrationConfigData integrationConfigData)
    {
      Assert.ArgumentNotNull(integrationConfigData, "integrationConfigData");

      var context = SpContextProviderBase.Instance.CreateDataContext(integrationConfigData);

      return BaseList.GetList(integrationConfigData.Web, integrationConfigData.List, context);
    }

    public static bool IsIntegrationItem([NotNull] Item item)
    {
      Assert.ArgumentNotNull(item, "item");

      using (new IntegrationDisabler())
      {
        if (!Common.Utils.ItemUtil.IsContentItem(item))
        {
          return false;
        }

        Template template = TemplateManager.GetTemplate(item);
        if (template == null)
        {
          return false;
        }

        return template.DescendsFromOrEquals(Common.TemplateIDs.IntegrationBase);
      }
    }

    #region IsIntegrationDataItem

    /// <summary>
    /// Determines whether the specified item is integration item.
    /// </summary>
    /// <param name="itemId">The item Id.</param>
    /// <param name="database">The database.</param>
    /// <returns>
    ///  <c>true</c> if the specified item is integration item; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsActiveIntegrationDataItem([NotNull] ID itemId, [NotNull] Database database)
    {
      Assert.ArgumentNotNull(itemId, "itemId");
      Assert.ArgumentNotNull(database, "database");

      using (new IntegrationDisabler())
      {
        Item item = database.GetItem(itemId);
        return IsActiveIntegrationDataItem(item);
      }
    }

    /// <summary>
    /// Determines whether the specified item is integration item.
    /// </summary>
    /// <param name="item">The specified item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is integration item; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsActiveIntegrationDataItem([NotNull] Item item)
    {
      Assert.ArgumentNotNull(item, "item");

      using (new IntegrationDisabler())
      {
        return
          IsIntegrationDataItem(item) &&
          new IntegrationItem(item).IsActive;
      }
    }

    public static bool IsIntegrationDataItem([NotNull] Item item)
    {
      Assert.ArgumentNotNull(item, "item");

      using (new IntegrationDisabler())
      {
        return
          IsIntegrationItem(item) &&
          item.TemplateID != SharepointTemplateIDs.IntegrationConfig;
      }
    }

    #endregion IsIntegrationDataItem

    /// <summary>
    /// Determines whether the specified item is integration config item.
    /// </summary>
    /// <param name="item">The specified item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is integration config item; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsActiveIntegrationConfigItem([NotNull] Item item)
    {
      Assert.ArgumentNotNull(item, "item");

      using (new IntegrationDisabler())
      {
        return
          IsIntegrationConfigItem(item) &&
          new IntegrationItem(item).IsActive;
      }
    }

    public static bool IsIntegrationConfigItem([NotNull] Item item)
    {
      Assert.ArgumentNotNull(item, "item");

      using (new IntegrationDisabler())
      {
        return
          Common.Utils.ItemUtil.IsContentItem(item) &&
          item.TemplateID == SharepointTemplateIDs.IntegrationConfig;
      }
    }

    /// <summary>
    /// Determines whether the specified item is integration folder item.
    /// </summary>
    /// <param name="item">The specified item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is integration folder item; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsActiveIntegrationFolder([NotNull] Item item)
    {
      Assert.ArgumentNotNull(item, "item");

      using (new IntegrationDisabler())
      {
        return
          IsIntegrationFolder(item) &&
          new IntegrationItem(item).IsActive;
      }
    }

    public static bool IsIntegrationFolder([NotNull] Item item)
    {
      Assert.ArgumentNotNull(item, "item");

      using (new IntegrationDisabler())
      {
        return
          Common.Utils.ItemUtil.IsContentItem(item) &&
          item.TemplateID == SharepointTemplateIDs.IntegrationFolder;
      }
    }

    public static void CopyIntegrationFlagToChildren([NotNull] IntegrationItem integrationItem)
    {
      Assert.ArgumentNotNull(integrationItem, "integrationItem");

      using (new IntegrationDisabler())
      {
        if (!IsIntegrationFolder(integrationItem.InnerItem) && !IsIntegrationConfigItem(integrationItem.InnerItem))
        {
          return;
        }

        foreach (Item childItem in integrationItem.InnerItem.Children)
        {
          if (IsIntegrationDataItem(childItem) || IsIntegrationFolder(childItem))
          {
            // It is not necessary to update all descendants recursively, 
            // because it will be done by SaveItem method of ItemProvider class.
            var childIntegrationItem = new IntegrationItem(childItem)
            {
              IsActive = integrationItem.IsActive
            };
          }
        }
      }
    }
  }
}