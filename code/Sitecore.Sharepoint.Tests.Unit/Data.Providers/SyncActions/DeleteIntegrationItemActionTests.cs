// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteIntegrationItemActionTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the DeleteIntegrationItemActionTests type.
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
  using Sitecore.Sharepoint.Pipelines.ProcessIntegrationItem;
  using Xunit;

  public class DeleteIntegrationItemActionTests
  {
    [Fact]
    public void should_call_pipeline()
    {
      // Arrange
      using (var pipelines = new PipelinesHandler())
      {
        var deletedItem = new ItemMock();
        Item item = new ItemMock().AsConfigurationItem().WithChild(deletedItem);

        var synchContext = new SynchContext(item);
        
        var action = new DeleteIntegrationItemAction(new IntegrationItem(deletedItem), synchContext);

        // Act
        action.Execute();
        action.IsSuccessful.Should().BeTrue();

        // Assert
        pipelines.ShouldReceiveCall<ProcessIntegrationItemArgs>(
          PipelineNames.DeleteIntegrationItem,
          x => x.IntegrationItemID == deletedItem.ID && x.SynchContext == synchContext);
      }
    }

    [Fact]
    public void should_be_not_successsfull_if_pipeline_throw_exception()
    {
      // Arrange
      using (new PipelinesHandler().ThrowInPipeline(PipelineNames.DeleteIntegrationItem))
      {
        var synchContext = new SynchContext(new ItemMock().AsConfigurationItem());
        var action = new DeleteIntegrationItemAction(new IntegrationItem(new ItemMock().AsIntegrationItem()), synchContext);

        // Act
        action.Execute();

        // Assert
        action.IsSuccessful.Should().BeFalse();
      }
    }
  }
}
