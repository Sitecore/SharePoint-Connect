// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateIntegrationItemAction.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the UpdateIntegrationItemAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.SyncActions
{
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.ProcessIntegrationItem.Common;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.Runners;

  public class UpdateIntegrationItemAction : SharepointItemActionBase
  {
    private readonly IntegrationItem item;
    private readonly ProcessIntegrationItemsOptions options;

    public UpdateIntegrationItemAction(IntegrationItem item, ObjectModel.Entities.Items.BaseItem sharepointItem, SynchContext synchContext, ProcessIntegrationItemsOptions options)
      : base(sharepointItem, synchContext)
    {
      this.item = item;
      this.options = options;
    }

    public override bool ExecuteAction()
    {
      var resultArgs = IntegrationPipelinesRunner.UpdateIntegrationItem(this.item.ID, this.SharepointItem, this.SynchContext, this.options, EventSender.Sharepoint);

      if (this.options.Recursive && SharepointProvider.IsActiveIntegrationFolder(this.item.InnerItem))
      {
        IntegrationPipelinesRunner.SynchronizeTree(this.options, new SynchContext(this.item.InnerItem));
      }

      return resultArgs != null;
    }
  }
}
