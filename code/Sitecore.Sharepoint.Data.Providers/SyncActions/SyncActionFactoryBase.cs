// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncActionFactoryBase.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SyncActionFactoryBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.SyncActions
{
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;

  public abstract class SyncActionFactoryBase
  {
    public abstract SyncActionBase GetCreateIntegrationItemAction(SynchronizeTreeArgs args, BaseItem sharepointItem);

    public abstract SyncActionBase GetCreateSharepointItemAction(SynchronizeTreeArgs args, IntegrationItem item);

    public abstract SyncActionBase GetUpdateIntegrationItemAction(SynchronizeTreeArgs args, IntegrationItem item, BaseItem sharepointItem);

    public abstract SyncActionBase GetUpdateSharepointItemAction(SynchronizeTreeArgs args, BaseItem sharepointItem, IntegrationItem item);

    public abstract SyncActionBase GetDeleteIntegrationItemAction(SynchronizeTreeArgs args, IntegrationItem item);

    public abstract SyncActionBase GetDeleteSharepointItemAction(SynchronizeTreeArgs args, BaseItem sharepointItem);
  }
}