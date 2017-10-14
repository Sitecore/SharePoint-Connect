// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncActionFactory.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SyncActionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.SyncActions
{
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;

  public class SyncActionFactory : SyncActionFactoryBase
  {
    public override SyncActionBase GetCreateIntegrationItemAction(SynchronizeTreeArgs args, BaseItem sharepointItem)
    {
      return new CreateIntegrationItemAction(sharepointItem, args.Context, args.Options);
    }

    public override SyncActionBase GetCreateSharepointItemAction(SynchronizeTreeArgs args, IntegrationItem item)
    {
      return new CreateSharepointItemAction(item, args.Context);
    }

    public override SyncActionBase GetUpdateIntegrationItemAction(SynchronizeTreeArgs args, IntegrationItem item, BaseItem sharepointItem)
    {
      return new UpdateIntegrationItemAction(item, sharepointItem, args.Context, args.Options);
    }

    public override SyncActionBase GetUpdateSharepointItemAction(SynchronizeTreeArgs args, BaseItem sharepointItem, IntegrationItem item)
    {
      return new UpdateSharepointItemAction(sharepointItem, item, args.Context);
    }

    public override SyncActionBase GetDeleteIntegrationItemAction(SynchronizeTreeArgs args, IntegrationItem item)
    {
      return new DeleteIntegrationItemAction(item, args.Context);
    }

    public override SyncActionBase GetDeleteSharepointItemAction(SynchronizeTreeArgs args, BaseItem sharepointItem)
    {
      return new DeleteSharepointItemAction(sharepointItem, args.Context);
    }
  }
}