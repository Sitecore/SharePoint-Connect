// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MergeLists.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the MergeLists type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.SynchronizeTree
{
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.Providers.SyncActions;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree.Helpers;

  public class MergeLists
  {
    private readonly SyncActionFactoryBase actionFactory;
    private readonly HistoryProviderBase historyProvider;

    public MergeLists(SyncActionFactoryBase actionFactory, HistoryProviderBase historyProvider)
    {
      this.actionFactory = actionFactory;
      this.historyProvider = historyProvider;
    }

    public void Process(SynchronizeTreeArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.IsNotNull(args.IntegrationItems, "Value can't be null: args.IntegrationItems");
      Assert.IsNotNull(args.SharepointItemList, "Value can't be null: args.SharepointItemList");
      Assert.IsNotNull(args.Context, "Value can't be null: args.Context");

      var actions = new List<SyncActionBase>();

      var isBidirectional = args.Context.IntegrationConfigData.BidirectionalLink;
      var unprocessedBaseItems = args.SharepointItemList.ToDictionary(item => item.GUID);

      foreach (var integrationItem in args.IntegrationItems)
      {
        ObjectModel.Entities.Items.BaseItem sharepointItem;
        if (unprocessedBaseItems.TryGetValue(integrationItem.GUID, out sharepointItem))
        {
          if (this.historyProvider.IsItemDeleted(args, integrationItem.GUID))
          {
            continue;
          }

          unprocessedBaseItems.Remove(integrationItem.GUID);
          if (integrationItem.IsActive)
          {
            if (isBidirectional && this.historyProvider.IsItemChanged(args, integrationItem.GUID))
            {
              actions.Add(this.actionFactory.GetUpdateSharepointItemAction(args, sharepointItem, integrationItem));
            }
            else
            {
              actions.Add(this.actionFactory.GetUpdateIntegrationItemAction(args, integrationItem, sharepointItem));
            }
          }
        }
        else
        {
          if (isBidirectional && integrationItem.IsNew)
          {
            actions.Add(this.actionFactory.GetCreateSharepointItemAction(args, integrationItem));
          }
          else if (integrationItem.IsActive)
          {
            actions.Add(this.actionFactory.GetDeleteIntegrationItemAction(args, integrationItem));
          }
        }
      }

      foreach (var baseItem in unprocessedBaseItems.Values)
      {
        if (isBidirectional && this.historyProvider.IsItemDeleted(args, baseItem.GUID))
        {
          actions.Add(this.actionFactory.GetDeleteSharepointItemAction(args, baseItem));
        }
        else
        {
          actions.Add(this.actionFactory.GetCreateIntegrationItemAction(args, baseItem));
        }
      }

      args.ActionList = actions;
    }
  }
}