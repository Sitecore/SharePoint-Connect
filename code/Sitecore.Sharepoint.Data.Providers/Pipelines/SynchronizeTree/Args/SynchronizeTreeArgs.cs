// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizeTreeArgs.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SynchronizeTreeArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.SynchronizeTree
{
  using System.Collections.Generic;
  using Sitecore.Pipelines;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Sharepoint.Data.Providers.SyncActions;

  public class SynchronizeTreeArgs : PipelineArgs
  {
    public List<IntegrationItem> IntegrationItems { get; set; }

    public List<ObjectModel.Entities.Items.BaseItem> SharepointItemList { get; set; }
    
    public SynchContext Context { get; set; }

    public List<SyncActionBase> ActionList { get; set; }
    
    public ProcessIntegrationItemsOptions Options { get; set; }
  }
}