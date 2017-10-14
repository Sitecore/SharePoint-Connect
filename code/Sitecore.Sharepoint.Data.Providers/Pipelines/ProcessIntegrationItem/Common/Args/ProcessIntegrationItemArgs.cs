// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessIntegrationItemArgs.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ProcessIntegrationItemArgs class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.ProcessIntegrationItem
{
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Pipelines;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.ProcessIntegrationItem.Common;
  using SharepointBaseItem = Sitecore.Sharepoint.ObjectModel.Entities.Items.BaseItem;

  public class ProcessIntegrationItemArgs : PipelineArgs
  {
    public Item IntegrationItem { get; set; }

    public ID IntegrationItemID { get; set; }

    public ID IntegrationItemTemplateID { get; set; }

    public SharepointBaseItem SourceSharepointItem { get; set; }

    public SynchContext SynchContext { get; set; }

    public ProcessIntegrationItemsOptions Options { get; set; }

    public EventSender EventSender { get; set; }
  }
}