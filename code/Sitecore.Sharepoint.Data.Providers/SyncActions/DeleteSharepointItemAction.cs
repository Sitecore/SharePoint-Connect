// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteSharepointItemAction.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the DeleteSharepointItemAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.SyncActions
{
  using Sitecore.Sharepoint.Data.Providers.Pipelines.Runners;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;

  public class DeleteSharepointItemAction : SharepointItemActionBase
  {
    public DeleteSharepointItemAction(BaseItem sharepointItem, SynchContext synchContext)
      : base(sharepointItem, synchContext)
    {
    }

    public override bool ExecuteAction()
    {
      return IntegrationPipelinesRunner.DeleteSharepointItem(this.SharepointItem, this.SynchContext) != null;
    }
  }
}
