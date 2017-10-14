// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslateIntegrationValueArgs.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the TranslateIntegrationValueArgs class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.TranslateIntegrationValue
{
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Pipelines;

  using SharepointBaseItem = Sitecore.Sharepoint.ObjectModel.Entities.Items.BaseItem;

  public class TranslateIntegrationValueArgs : PipelineArgs
  {
    public readonly Item SourceIntegrationItem;
    public readonly string SourceFieldName;
    public readonly SharepointBaseItem TargetSharepointItem;
    public readonly string TargetFieldName;

    public string TranslatedValue;

    public TranslateIntegrationValueArgs(
                                         [NotNull] Item sourceIntegrationItem, 
                                         [NotNull] string sourceFieldName, 
                                         [NotNull] SharepointBaseItem targetSharepointItem, 
                                         [NotNull] string targetFieldName)
    {
      Assert.ArgumentNotNull(sourceIntegrationItem, "sourceIntegrationItem");
      Assert.ArgumentNotNull(sourceFieldName, "sourceFieldName");
      Assert.ArgumentNotNull(targetSharepointItem, "targetSharepointItem");
      Assert.ArgumentNotNull(targetFieldName, "targetFieldName");

      this.SourceIntegrationItem = sourceIntegrationItem;
      this.SourceFieldName = sourceFieldName;
      this.TargetSharepointItem = targetSharepointItem;
      this.TargetFieldName = targetFieldName;
    }
  }
}