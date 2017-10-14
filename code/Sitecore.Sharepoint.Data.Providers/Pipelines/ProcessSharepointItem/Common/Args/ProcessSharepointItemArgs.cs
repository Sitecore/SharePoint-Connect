// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessSharepointItemArgs.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ProcessSharepointItemArgs class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.ProcessSharepointItem
{
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Pipelines;
  using Sitecore.Sharepoint.Data.Providers;
  using SharepointBaseItem = Sitecore.Sharepoint.ObjectModel.Entities.Items.BaseItem;

  public class ProcessSharepointItemArgs : PipelineArgs
  {
    public SharepointBaseItem SharepointItem { get; set; }

    public string SharepointItemID { get; set; }

    public Item SourceIntegrationItem { get; set; }

    public string SourceIntegrationItemName { get; set; }

    public ID SourceIntegrationItemTemplateID { get; set; }
    
    public SynchContext SynchContext { get; set; }
  }
}