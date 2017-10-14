// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipelineRunner.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines PipelineRunner class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.Pipelines.Runners
{
  using System;

  using Microsoft.Extensions.DependencyInjection;

  using Sitecore.Abstractions;
  using Sitecore.DependencyInjection;
  using Sitecore.Diagnostics;
  using Sitecore.Pipelines;
  using Sitecore.Sharepoint.Common.Texts;

  /// <summary>
  /// Defines methods for running pipelines.
  /// </summary>
  public static class PipelineRunner
  {
    private static BaseCorePipelineManager pipelineManger;

    /// <summary>
    /// Runs the specified pipeline.
    /// </summary>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <param name="pipelineArgs">The pipeline arguments.</param>
    /// <returns><c>true</c> if the specified pipeline is completed successfully; otherwise, <c>false</c>.</returns>
    public static bool Run([NotNull] string pipelineName, [NotNull] PipelineArgs pipelineArgs)
    {
      Assert.ArgumentNotNull(pipelineName, "pipelineName");
      Assert.ArgumentNotNull(pipelineArgs, "pipelineArgs");

      try
      {
        AssertRun(pipelineName, pipelineArgs);
      }
      catch (Exception ex)
      {
        string logMessage = string.Format(LogMessages.Text0PipelineFailedToExecute, pipelineName);
        Log.Error(logMessage, ex, typeof(PipelineRunner));

        return false;
      }

      return true;
    }

    /// <summary>
    /// Runs the specified pipeline without any handling of exception.
    /// </summary>
    /// <param name="pipelineName">The pipeline name.</param>
    /// <param name="pipelineArgs">The pipeline args.</param>
    public static void AssertRun([NotNull] string pipelineName, [NotNull] PipelineArgs pipelineArgs)
    {
      Assert.ArgumentNotNullOrEmpty(pipelineName, "pipelineName");
      Assert.ArgumentNotNull(pipelineArgs, "pipelineArgs");

      PipelineManager.Run(pipelineName, pipelineArgs, true);
    }

    internal static BaseCorePipelineManager PipelineManager
    {
      private get
      {
        return pipelineManger
               ?? (pipelineManger = ServiceLocator.ServiceProvider.GetRequiredService<BaseCorePipelineManager>());
      }

      set
      {
        pipelineManger = value;
      }
    }
  }
}
