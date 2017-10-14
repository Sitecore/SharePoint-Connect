// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteIntegrationItemAction.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the DeleteIntegrationItemAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.SyncActions
{
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.Runners;

  public class DeleteIntegrationItemAction : IntegrationItemActionBase
  {
    public DeleteIntegrationItemAction(IntegrationItem item, SynchContext synchContext)
      : base(item, synchContext)
    {
    }

    public override bool ExecuteAction()
    {
      return IntegrationPipelinesRunner.DeleteIntegrationItem(this.Item.ID, this.SynchContext) != null;
    }
  }
}
