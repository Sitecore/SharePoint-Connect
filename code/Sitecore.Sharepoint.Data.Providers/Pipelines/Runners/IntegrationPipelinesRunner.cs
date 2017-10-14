// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationPipelinesRunner.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines IntegrationPipelinesRunner class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.Pipelines.Runners
{
  using System;
  using System.Text;
  using System.Web.Services.Protocols;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Texts;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.Logging;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.ProcessIntegrationItem.Common;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Sharepoint.Pipelines.ProcessIntegrationItem;
  using Sitecore.Sharepoint.Pipelines.ProcessSharepointItem;
  using SharepointBaseItem = Sitecore.Sharepoint.ObjectModel.Entities.Items.BaseItem;

  public static class IntegrationPipelinesRunner
  {
    public static bool CreateIntegrationItem([NotNull] ProcessIntegrationItemArgs pipelineArgs)
    {
      Assert.ArgumentNotNull(pipelineArgs, "pipelineArgs");

      try
      {
        PipelineRunner.AssertRun(PipelineNames.CreateIntegrationItem, pipelineArgs);
        return true;
      }
      catch (Exception exception)
      {
        LogMessage(exception, LogMessages.Text0IntegrationItem1HasBeenFailed, "Creating", pipelineArgs.IntegrationItemID);
      }

      return false;
    }

    [CanBeNull]
    public static ProcessIntegrationItemArgs CreateIntegrationItem(
                                                                   [NotNull] ID integrationItemID,
                                                                   [NotNull] SharepointBaseItem sourceSharepointItem,
                                                                   [NotNull] SynchContext synchContext,
                                                                   [NotNull] ProcessIntegrationItemsOptions options,
                                                                   [NotNull] EventSender eventSender)
    {
      Assert.ArgumentNotNull(integrationItemID, "integrationItemID");
      Assert.ArgumentNotNull(sourceSharepointItem, "sourceSharepointItem");
      Assert.ArgumentNotNull(synchContext, "synchContext");
      Assert.ArgumentNotNull(options, "options");

      var pipelineArgs = new ProcessIntegrationItemArgs
      {
        IntegrationItemID = integrationItemID,
        SourceSharepointItem = sourceSharepointItem,
        SynchContext = synchContext,
        Options = options,
        EventSender = eventSender
      };

      return CreateIntegrationItem(pipelineArgs) ? pipelineArgs : null;
    }

    public static bool UpdateIntegrationItem([NotNull] ProcessIntegrationItemArgs pipelineArgs)
    {
      Assert.ArgumentNotNull(pipelineArgs, "pipelineArgs");

      try
      {
        PipelineRunner.AssertRun(PipelineNames.UpdateIntegrationItem, pipelineArgs);
        return true;
      }
      catch (Exception exception)
      {
        LogMessage(exception, LogMessages.Text0IntegrationItem1HasBeenFailed, "Updating", pipelineArgs.IntegrationItemID);
      }

      return false;
    }

    [CanBeNull]
    public static ProcessIntegrationItemArgs UpdateIntegrationItem(
                                                                   [NotNull] ID integrationItemID,
                                                                   [NotNull] SharepointBaseItem sourceSharepointItem,
                                                                   [NotNull] SynchContext synchContext,
                                                                   [NotNull] ProcessIntegrationItemsOptions options,
                                                                   [NotNull] EventSender eventSender)
    {
      Assert.ArgumentNotNull(integrationItemID, "integrationItemID");
      Assert.ArgumentNotNull(sourceSharepointItem, "sourceSharepointItem");
      Assert.ArgumentNotNull(synchContext, "synchContext");
      Assert.ArgumentNotNull(options, "options");

      var pipelineArgs = new ProcessIntegrationItemArgs
      {
        IntegrationItemID = integrationItemID,
        SourceSharepointItem = sourceSharepointItem,
        SynchContext = synchContext,
        Options = options,
        EventSender = eventSender
      };

      return UpdateIntegrationItem(pipelineArgs) ? pipelineArgs : null;
    }

    public static bool DeleteIntegrationItem([NotNull] ProcessIntegrationItemArgs pipelineArgs)
    {
      Assert.ArgumentNotNull(pipelineArgs, "pipelineArgs");

      try
      {
        PipelineRunner.AssertRun(PipelineNames.DeleteIntegrationItem, pipelineArgs);
        return true;
      }
      catch (Exception exception)
      {
        LogMessage(exception, LogMessages.Text0IntegrationItem1HasBeenFailed, "Deleting", pipelineArgs.IntegrationItemID);
      }

      return false;
    }

    [CanBeNull]
    public static ProcessIntegrationItemArgs DeleteIntegrationItem([NotNull] ID integrationItemID, [NotNull] SynchContext synchContext)
    {
      Assert.ArgumentNotNull(integrationItemID, "integrationItemID");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      var pipelineArgs = new ProcessIntegrationItemArgs
      {
        IntegrationItemID = integrationItemID,
        SynchContext = synchContext
      };

      return DeleteIntegrationItem(pipelineArgs) ? pipelineArgs : null;
    }

    public static bool CreateSharepointItem([NotNull] ProcessSharepointItemArgs pipelineArgs)
    {
      Assert.ArgumentNotNull(pipelineArgs, "pipelineArgs");

      try
      {
        PipelineRunner.AssertRun(PipelineNames.CreateSharepointItem, pipelineArgs);
        return true;
      }
      catch (Exception exception)
      {
        LogMessage(exception, LogMessages.Text0SharePointItem1HasBeenFailed, "Creating", LogMessageFormatter.FormatText0Item1(pipelineArgs.SynchContext.IntegrationConfigData, pipelineArgs.SourceIntegrationItemName));
      }

      return false;
    }

    [CanBeNull]
    public static ProcessSharepointItemArgs CreateSharepointItem(
                                                                 [NotNull] string sourceIntegrationItemName, 
                                                                 [NotNull] ID sourceIntegrationItemTemplateID, 
                                                                 [NotNull] SynchContext synchContext)
    {
      Assert.ArgumentNotNull(sourceIntegrationItemName, "sourceIntegrationItemName");
      Assert.ArgumentNotNull(sourceIntegrationItemTemplateID, "sourceIntegrationItemTemplateID");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      var pipelineArgs = new ProcessSharepointItemArgs
      {
        SourceIntegrationItemName = sourceIntegrationItemName,
        SourceIntegrationItemTemplateID = sourceIntegrationItemTemplateID,
        SynchContext = synchContext
      };

      return CreateSharepointItem(pipelineArgs) ? pipelineArgs : null;
    }

    [CanBeNull]
    public static ProcessSharepointItemArgs CreateSharepointItem([NotNull] IntegrationItem sourceIntegrationItem, [NotNull] SynchContext synchContext)
    {
      Assert.ArgumentNotNull(sourceIntegrationItem, "sourceIntegrationItem");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      var args = new ProcessSharepointItemArgs
      {
        SourceIntegrationItemName = sourceIntegrationItem.Name,
        SourceIntegrationItem = sourceIntegrationItem.InnerItem,
        SourceIntegrationItemTemplateID = sourceIntegrationItem.InnerItem.TemplateID,
        SynchContext = synchContext 
      };

      return CreateSharepointItem(args) ? args : null;
    }

    public static bool UpdateSharepointItem([NotNull] ProcessSharepointItemArgs pipelineArgs)
    {
      Assert.ArgumentNotNull(pipelineArgs, "pipelineArgs");

      try
      {
        PipelineRunner.AssertRun(PipelineNames.UpdateSharepointItem, pipelineArgs);
        return true;
      }
      catch (Exception exception)
      {
        LogMessage(exception, LogMessages.Text0SharePointItem1HasBeenFailed, "Updating", pipelineArgs.SharepointItemID);
      }

      return false;
    }

    [CanBeNull]
    public static ProcessSharepointItemArgs UpdateSharepointItem(
                                                                 [NotNull] string sharepointItemID, 
                                                                 [NotNull] Item sourceIntegrationItem, 
                                                                 [NotNull] SynchContext synchContext)
    {
      Assert.ArgumentNotNull(sharepointItemID, "sharepointItemID");
      Assert.ArgumentNotNull(sourceIntegrationItem, "sourceIntegrationItem");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      var pipelineArgs = new ProcessSharepointItemArgs
      {
        SharepointItemID = sharepointItemID,
        SourceIntegrationItem = sourceIntegrationItem,
        SynchContext = synchContext
      };

      return UpdateSharepointItem(pipelineArgs) ? pipelineArgs : null;
    }

    [CanBeNull]
    public static ProcessSharepointItemArgs UpdateSharepointItem(
                                                                 [NotNull] SharepointBaseItem sharepointItem,
                                                                 [NotNull] Item sourceIntegrationItem,
                                                                 [NotNull] SynchContext synchContext)
    {
      Assert.ArgumentNotNull(sharepointItem, "sharepointItem");
      Assert.ArgumentNotNull(sourceIntegrationItem, "sourceIntegrationItem");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      var pipelineArgs = new ProcessSharepointItemArgs
      {
        SharepointItemID = sharepointItem.UniqueID,
        SharepointItem = sharepointItem,
        SourceIntegrationItem = sourceIntegrationItem,
        SynchContext = synchContext
      };

      return UpdateSharepointItem(pipelineArgs) ? pipelineArgs : null;
    }

    public static bool DeleteSharepointItem([NotNull] ProcessSharepointItemArgs pipelineArgs)
    {
      Assert.ArgumentNotNull(pipelineArgs, "pipelineArgs");

      try
      {
        PipelineRunner.AssertRun(PipelineNames.DeleteSharepointItem, pipelineArgs);
        return true;
      }
      catch (Exception exception)
      {
        LogMessage(exception, LogMessages.Text0SharePointItem1HasBeenFailed, "Deleting", pipelineArgs.SharepointItemID);
      }

      return false;
    }

    [CanBeNull]
    public static ProcessSharepointItemArgs DeleteSharepointItem([NotNull] string sharepointItemID, [NotNull] SynchContext synchContext)
    {
      Assert.ArgumentNotNull(sharepointItemID, "sharepointItemID");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      var pipelineArgs = new ProcessSharepointItemArgs
      {
        SharepointItemID = sharepointItemID,
        SynchContext = synchContext
      };

      return DeleteSharepointItem(pipelineArgs) ? pipelineArgs : null;
    }

    [CanBeNull]
    public static ProcessSharepointItemArgs DeleteSharepointItem([NotNull] SharepointBaseItem sharepointItem, [NotNull] SynchContext synchContext)
    {
      Assert.ArgumentNotNull(sharepointItem, "sharepointItem");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      var pipelineArgs = new ProcessSharepointItemArgs
      {
        SharepointItemID = sharepointItem.UniqueID,
        SharepointItem = sharepointItem,
        SynchContext = synchContext
      };

      return DeleteSharepointItem(pipelineArgs) ? pipelineArgs : null;
    }

    public static void SynchronizeTree(ProcessIntegrationItemsOptions processIntegrationItemsOptions, SynchContext synchContext)
    {
      Assert.ArgumentNotNull(processIntegrationItemsOptions, "processIntegrationItemsOptions");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      var pipelineArgs = new SynchronizeTreeArgs
      {
        Context = synchContext,
        Options = processIntegrationItemsOptions
      };

      SynchronizeTree(pipelineArgs);
    }

    private static void SynchronizeTree(SynchronizeTreeArgs pipelineArgs)
    {
      try
      {
        PipelineRunner.AssertRun(PipelineNames.SynchronizeTree, pipelineArgs);
      }
      catch (Exception exception)
      {
        LogMessage(
          exception,
          "Sharepoint Provider can't process tree.{1}{0}",
          LogMessageFormatter.FormatIntegrationConfigItemID02(pipelineArgs.Context), 
          Environment.NewLine);
      }
    }

    private static void LogMessage(Exception exception, string messageFormat, params object[] messageArgs)
    {
      var errorMessage = new StringBuilder();

      if (messageArgs.Length > 0)
      {
        errorMessage.AppendFormat(messageFormat, messageArgs);
      }
      else
      {
        errorMessage.Append(messageFormat);
      }

      var soapException = exception as SoapException;
      if (soapException != null && soapException.Detail != null)
      {
        string detailMessage = soapException.Detail.InnerText;
        if (!string.IsNullOrEmpty(detailMessage))
        {
          errorMessage.AppendLine();
          errorMessage.Append(detailMessage);
        }
      }

      Log.Error(errorMessage.ToString(), exception, typeof(IntegrationPipelinesRunner));
    }
  }
}