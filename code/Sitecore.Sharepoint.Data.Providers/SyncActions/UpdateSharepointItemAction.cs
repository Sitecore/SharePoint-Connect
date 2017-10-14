// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSharepointItemAction.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the UpdateSharepointItemAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.SyncActions
{
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.Runners;

  public class UpdateSharepointItemAction : IntegrationItemActionBase
  {
    private readonly ObjectModel.Entities.Items.BaseItem sharepointItem;

    public UpdateSharepointItemAction(ObjectModel.Entities.Items.BaseItem sharepointItem, IntegrationItem item, SynchContext synchContext)
      : base(item, synchContext)
    {
      this.sharepointItem = sharepointItem;
    }

    public override bool ExecuteAction()
    {
      return IntegrationPipelinesRunner.UpdateSharepointItem(this.sharepointItem, this.Item.InnerItem, this.SynchContext) != null;
    }
  }
}