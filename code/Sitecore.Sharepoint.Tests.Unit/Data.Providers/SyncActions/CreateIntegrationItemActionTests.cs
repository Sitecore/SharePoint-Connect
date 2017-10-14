// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateIntegrationItemActionTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the CreateIntegrationItemActionTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.SyncActions
{
  using FluentAssertions;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Sharepoint.Data.Providers.Pipelines;
  using Sitecore.Sharepoint.Data.Providers.SyncActions;
  using Sitecore.Sharepoint.Pipelines.ProcessIntegrationItem;
  using Xunit;

  public class CreateIntegrationItemActionTests
  {
    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_call_pipeline()
    {
      // Arrange
      using (var pipelines = new PipelinesHandler())
      {
        var synchContext = new SynchContext(new ItemMock().AsConfigurationItem());
        var sharepointItem = TestHelper.CreateSharepointItem();
        var action = new CreateIntegrationItemAction(sharepointItem, synchContext, ProcessIntegrationItemsOptions.DefaultOptions);

        // Act
        action.Execute();
        action.IsSuccessful.Should().BeTrue();

        // Assert
        pipelines.ShouldReceiveCall<ProcessIntegrationItemArgs>(
          PipelineNames.CreateIntegrationItem,
          x => x.SourceSharepointItem == sharepointItem && x.SynchContext == synchContext);
      }
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_be_not_successsfull_if_pipeline_throw_exception()
    {
      // Arrange
      using (new PipelinesHandler().ThrowInPipeline(PipelineNames.CreateIntegrationItem))
      {
        var synchContext = new SynchContext(new ItemMock().AsConfigurationItem());
        var action = new CreateIntegrationItemAction(TestHelper.CreateSharepointItem(), synchContext, ProcessIntegrationItemsOptions.DefaultOptions);

        // Act
        action.Execute();

        // Assert
        action.IsSuccessful.Should().BeFalse();
      }
    }
  }
}
