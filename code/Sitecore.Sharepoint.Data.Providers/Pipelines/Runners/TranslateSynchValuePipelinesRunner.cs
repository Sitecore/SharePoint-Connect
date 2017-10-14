// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslateSynchValuePipelinesRunner.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines TranslateSynchValuePipelinesRunner class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.Pipelines.Runners
{
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Pipelines.TranslateIntegrationValue;
  using Sitecore.Sharepoint.Pipelines.TranslateSharepointValue;
  using SharepointBaseItem = Sitecore.Sharepoint.ObjectModel.Entities.Items.BaseItem;

  public static class TranslateSynchValuePipelinesRunner
  {
    public static bool TranslateSharepointValue([NotNull] TranslateSharepointValueArgs pipelineArgs)
    {
      Assert.ArgumentNotNull(pipelineArgs, "pipelineArgs");

      return PipelineRunner.Run(PipelineNames.TranslateSharepointValue, pipelineArgs);
    }

    [CanBeNull]
    public static TranslateSharepointValueArgs TranslateSharepointValue(
                                                                        [NotNull] SharepointBaseItem sourceSharepointItem, 
                                                                        [NotNull] string sourceFieldName, 
                                                                        [NotNull] Item targetIntegrationItem, 
                                                                        [NotNull] string targetFieldName)
    {
      Assert.ArgumentNotNull(sourceSharepointItem, "sourceSharepointItem");
      Assert.ArgumentNotNull(sourceFieldName, "sourceFieldName");
      Assert.ArgumentNotNull(targetIntegrationItem, "targetIntegrationItem");
      Assert.ArgumentNotNull(targetFieldName, "targetFieldName");

      var pipelineArgs = new TranslateSharepointValueArgs(sourceSharepointItem, sourceFieldName, targetIntegrationItem, targetFieldName);

      return TranslateSharepointValue(pipelineArgs) ? pipelineArgs : null;
    }

    public static bool TranslateIntegrationValue([NotNull] TranslateIntegrationValueArgs pipelineArgs)
    {
      Assert.ArgumentNotNull(pipelineArgs, "pipelineArgs");

      return PipelineRunner.Run(PipelineNames.TranslateIntegrationValue, pipelineArgs);
    }

    [CanBeNull]
    public static TranslateIntegrationValueArgs TranslateIntegrationValue(
                                                                          [NotNull] Item sourceIntegrationItem,
                                                                          [NotNull] string sourceFieldName,
                                                                          [NotNull] SharepointBaseItem targetSharepointItem,
                                                                          [NotNull] string targetFieldName)
    {
      Assert.ArgumentNotNull(sourceIntegrationItem, "sourceIntegrationItem");
      Assert.ArgumentNotNull(sourceFieldName, "sourceFieldName");
      Assert.ArgumentNotNull(targetSharepointItem, "targetSharepointItem");
      Assert.ArgumentNotNull(targetFieldName, "targetFieldName");

      var pipelineArgs = new TranslateIntegrationValueArgs(sourceIntegrationItem, sourceFieldName, targetSharepointItem, targetFieldName);

      return TranslateIntegrationValue(pipelineArgs) ? pipelineArgs : null;
    }
  }
}