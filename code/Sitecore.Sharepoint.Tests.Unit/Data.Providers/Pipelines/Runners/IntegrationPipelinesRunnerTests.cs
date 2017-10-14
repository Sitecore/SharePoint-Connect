// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationPipelinesRunnerTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the IntegrationPipelinesRunnerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.Pipelines.Runners
{
  using System;
  using System.Web.Services.Protocols;
  using System.Xml;
  using FluentAssertions;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Sharepoint.Data.Providers.Pipelines;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.Runners;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;

  using Xunit;

  public class IntegrationPipelinesRunnerTests
  {
    [Fact]
    public void should_run_sync_pipeline()
    {
      // Arrange
      using (var pipelines = new PipelinesHandler())
      {
        var synchContext = new SynchContext(new ItemMock().AsConfigurationItem());

        // Act
        IntegrationPipelinesRunner.SynchronizeTree(ProcessIntegrationItemsOptions.DefaultOptions, synchContext);

        // Assert
        pipelines.ShouldReceiveCall<SynchronizeTreeArgs>(PipelineNames.SynchronizeTree, x => x.Context == synchContext);
      }
    }

    [Fact]
    public void should_not_throw_if_sync_pipeline_throws()
    {
      // Arrange
      using (new PipelinesHandler().ThrowInPipeline(PipelineNames.SynchronizeTree, new Exception("Pipeline is not work")))
      {
        var synchContext = new SynchContext(new ItemMock().AsConfigurationItem());

        // Act
        Action action = () => IntegrationPipelinesRunner.SynchronizeTree(ProcessIntegrationItemsOptions.DefaultOptions, synchContext);

        // Assert
        action.ShouldNotThrow();
      }
    }

    [Fact]
    public void should_log_if_sync_pipeline_throws()
    {
      // Arrange
      var exc = new Exception("Pipeline is not work");
      using (new PipelinesHandler().ThrowInPipeline(PipelineNames.SynchronizeTree, exc))
      {
        var synchContext = new SynchContext(new ItemMock().AsConfigurationItem());

        // Act
        var logWatcher = new LogWatcher(() => IntegrationPipelinesRunner.SynchronizeTree(ProcessIntegrationItemsOptions.DefaultOptions, synchContext));

        // Assert
        logWatcher.Ensure()
          .LevelIs(LogNotificationLevel.Error)
          .MessageIs(string.Format("Sharepoint Provider can't process tree.{2}Integration config item ID: {0}, {1}", synchContext.ParentID, "Web: server List: list", Environment.NewLine))
          .ExceptionIs(exc);
      }
    }

    [Fact]
    public void should_correct_log_soap_exception()
    {
      // Arrange
      var document = new XmlDocument();
      document.LoadXml("<details>DetailsInfo</details>");
      var exc = new SoapException("Pipeline is not work", new XmlQualifiedName(), "actor", document.ChildNodes[0]);

      using (new PipelinesHandler().ThrowInPipeline(PipelineNames.SynchronizeTree, exc))
      {
        var synchContext = new SynchContext(new ItemMock().AsConfigurationItem());

        // Act
        var logWatcher = new LogWatcher(() => IntegrationPipelinesRunner.SynchronizeTree(ProcessIntegrationItemsOptions.DefaultOptions, synchContext));

        // Assert
        logWatcher.Ensure()
          .Message.Should()
          .EndWith(string.Format("{1}{0}", exc.Detail.InnerText, Environment.NewLine));
      }
    }
  }
}
