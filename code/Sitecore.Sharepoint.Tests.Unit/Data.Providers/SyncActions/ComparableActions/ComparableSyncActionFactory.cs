// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComparableSyncActionFactory.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ComparableSyncActionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.SyncActions.ComparableActions
{
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.SyncActions;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;

  public class ComparableSyncActionFactory : SyncActionFactoryBase
  {
    public override SyncActionBase GetCreateIntegrationItemAction(SynchronizeTreeArgs args, BaseItem sharepointItem)
    {
      return new ComparableCreateIntegrationItemAction(sharepointItem, args.Context, args.Options);
    }

    public override SyncActionBase GetCreateSharepointItemAction(SynchronizeTreeArgs args, IntegrationItem item)
    {
      return new ComparableCreateSharepointItemAction(item, args.Context);
    }

    public override SyncActionBase GetUpdateIntegrationItemAction(SynchronizeTreeArgs args, IntegrationItem item, BaseItem sharepointItem)
    {
      return new ComparableUpdateIntegrationItemAction(item, sharepointItem, args.Context, args.Options);
    }

    public override SyncActionBase GetUpdateSharepointItemAction(SynchronizeTreeArgs args, BaseItem sharepointItem, IntegrationItem item)
    {
      return new ComparableUpdateSharepointItemAction(sharepointItem, item, args.Context);
    }

    public override SyncActionBase GetDeleteIntegrationItemAction(SynchronizeTreeArgs args, IntegrationItem item)
    {
      return new ComparableDeleteIntegrationItemAction(item, args.Context);
    }

    public override SyncActionBase GetDeleteSharepointItemAction(SynchronizeTreeArgs args, BaseItem sharepointItem)
    {
      return new ComparableDeleteSharepointItemAction(sharepointItem, args.Context);
    }
  }
}