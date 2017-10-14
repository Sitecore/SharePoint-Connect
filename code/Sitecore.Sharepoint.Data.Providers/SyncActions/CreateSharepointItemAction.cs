// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateSharepointItemAction.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the CreateSharepointItemAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.SyncActions
{
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.Runners;

  public class CreateSharepointItemAction : IntegrationItemActionBase
  {
    public CreateSharepointItemAction(IntegrationItem item, SynchContext synchContext)
      : base(item, synchContext) 
    {
      Assert.IsTrue(this.Item.IsNew, "The item should be new");
    }

    public override bool ExecuteAction()
    {
      return IntegrationPipelinesRunner.CreateSharepointItem(this.Item, this.SynchContext) != null;
    }
  }
}