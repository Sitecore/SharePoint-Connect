// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateItem.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ResetIntegrationItem class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.UpdateSharepointItem
{
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.ProcessIntegrationItem.Common;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.Runners;
  using Sitecore.Sharepoint.Pipelines.ProcessSharepointItem;

  class ResetIntegrationItem
  {
    public virtual void Process([NotNull] ProcessSharepointItemArgs args)
    {
      using (new IntegrationDisabler())
      {
        IntegrationPipelinesRunner.UpdateIntegrationItem(args.SourceIntegrationItem.ID, args.SharepointItem, args.SynchContext, ProcessIntegrationItemsOptions.DefaultOptions, EventSender.Sitecore);
      }
    }
  }
}
