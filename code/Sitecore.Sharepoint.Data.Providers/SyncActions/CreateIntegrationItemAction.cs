// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateIntegrationItemAction.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the CreateIntegrationItemAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.SyncActions
{
  using Sitecore.Data;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.ProcessIntegrationItem.Common;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.Runners;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;

  public class CreateIntegrationItemAction : SharepointItemActionBase
  {
    private readonly ProcessIntegrationItemsOptions options;

    public CreateIntegrationItemAction(BaseItem sharepointItem, SynchContext synchContext, ProcessIntegrationItemsOptions options) : base(sharepointItem, synchContext)
    {
      this.options = options;
    }

    public override bool ExecuteAction()
    {
      var args = IntegrationPipelinesRunner.CreateIntegrationItem(ID.NewID, this.SharepointItem, this.SynchContext, ProcessIntegrationItemsOptions.DefaultOptions, EventSender.Sitecore);
      if (args == null)
      {
        return false;
      }

      if (this.options.Recursive && args.IntegrationItem != null && SharepointProvider.IsActiveIntegrationFolder(args.IntegrationItem))
      {
        IntegrationPipelinesRunner.SynchronizeTree(this.options, new SynchContext(args.IntegrationItem));
      }

      return true;
    }
  }
}