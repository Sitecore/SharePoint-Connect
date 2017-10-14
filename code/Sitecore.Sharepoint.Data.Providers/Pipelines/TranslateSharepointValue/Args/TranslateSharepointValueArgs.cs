// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslateSharepointValueArgs.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines TranslateSharepointValueArgs class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.TranslateSharepointValue
{
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Pipelines;

  using SharepointBaseItem = Sitecore.Sharepoint.ObjectModel.Entities.Items.BaseItem;

  public class TranslateSharepointValueArgs : PipelineArgs
  {
    public readonly SharepointBaseItem SourceSharepointItem;
    public readonly string SourceFieldName;
    public readonly Item TargetIntegrationItem;
    public readonly string TargetFieldName;

    public string TranslatedValue;

    public TranslateSharepointValueArgs(
                                        [NotNull] SharepointBaseItem sourceSharepointItem, 
                                        [NotNull] string sourceFieldName, 
                                        [NotNull] Item targetIntegrationItem, 
                                        [NotNull] string targetFieldName)
    {
      Assert.ArgumentNotNull(sourceSharepointItem, "sourceSharepointItem");
      Assert.ArgumentNotNull(sourceFieldName, "sourceFieldName");
      Assert.ArgumentNotNull(targetIntegrationItem, "targetIntegrationItem");
      Assert.ArgumentNotNull(targetFieldName, "targetFieldName");

      this.SourceSharepointItem = sourceSharepointItem;
      this.SourceFieldName = sourceFieldName;
      this.TargetIntegrationItem = targetIntegrationItem;
      this.TargetFieldName = targetFieldName;
    }
  }
}