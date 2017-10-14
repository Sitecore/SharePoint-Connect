// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateSharepointItemActionTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the CreateSharepointItemActionTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.SyncActions
{
  using FluentAssertions;
  using Sitecore.Data.Items;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.Pipelines;
  using Sitecore.Sharepoint.Data.Providers.SyncActions;
  using Sitecore.Sharepoint.Pipelines.ProcessSharepointItem;
  using Xunit;

  public class CreateSharepointItemActionTests
  {
    [Fact]
    public void should_call_pipeline()
    {
      // Arrange
      using (var pipelines = new PipelinesHandler())
      {
        var synchContext = new SynchContext(new ItemMock().AsConfigurationItem());

        Item sourceItem = new ItemMock().AsIntegrationItem();
        var action = new CreateSharepointItemAction(new IntegrationItem(sourceItem), synchContext);

        // Act
        action.Execute();
        action.IsSuccessful.Should().BeTrue();

        // Assert
        pipelines.ShouldReceiveCall<ProcessSharepointItemArgs>(
          PipelineNames.CreateSharepointItem,
          x =>
          x.SourceIntegrationItem == sourceItem && x.SourceIntegrationItemTemplateID == sourceItem.TemplateID
          && x.SynchContext == synchContext);
      }
    }

    [Fact]
    public void should_be_not_successsfull_if_pipeline_throw_exception()
    {
      // Arrange
      using (new PipelinesHandler().ThrowInPipeline(PipelineNames.CreateSharepointItem))
      {
        var synchContext = new SynchContext(new ItemMock().AsConfigurationItem());
        Item sourceItem = new ItemMock().AsIntegrationItem();
        var action = new CreateSharepointItemAction(new IntegrationItem(sourceItem), synchContext);

        // Act
        action.Execute();
        
        // Assert
        action.IsSuccessful.Should().BeFalse();
      }
    }
  }
}
