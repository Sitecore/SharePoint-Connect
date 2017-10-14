// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateIntegrationItemActionTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the UpdateIntegrationItemActionTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.SyncActions
{
  using FluentAssertions;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Sharepoint.Data.Providers.Pipelines;
  using Sitecore.Sharepoint.Data.Providers.SyncActions;
  using Sitecore.Sharepoint.Pipelines.ProcessIntegrationItem;
  using Xunit;

  public class UpdateIntegrationItemActionTests
  {
    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_call_pipeline()
    {
      // Arrange
      using (var pipelines = new PipelinesHandler())
      {
        var updateItem = new ItemMock();
        var item = new ItemMock().AsConfigurationItem()
          .WithChild(updateItem);

        var synchContext = new SynchContext(item);
        var sharepointItem = TestHelper.CreateSharepointItem();
        var options = new ProcessIntegrationItemsOptions();
        var action = new UpdateIntegrationItemAction(new IntegrationItem(updateItem), sharepointItem, synchContext, options);

        // Act
        action.Execute();
        action.IsSuccessful.Should().BeTrue();

        // Assert
        pipelines.ShouldReceiveCall<ProcessIntegrationItemArgs>(
          PipelineNames.UpdateIntegrationItem,
          x => x.IntegrationItemID == updateItem.ID
            && x.SourceSharepointItem == sharepointItem
            && x.SynchContext == synchContext
            && x.Options == options);
      }
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_be_not_successsfull_if_pipeline_throw_exception()
    {
      // Arrange
      using (new PipelinesHandler().ThrowInPipeline(PipelineNames.UpdateIntegrationItem))
      {
        var synchContext = new SynchContext(new ItemMock().AsConfigurationItem());
        var action = new UpdateIntegrationItemAction(new IntegrationItem(new ItemMock().AsIntegrationItem()), TestHelper.CreateSharepointItem(), synchContext, new ProcessIntegrationItemsOptions());

        // Act
        action.Execute();

        // Assert
        action.IsSuccessful.Should().BeFalse();
      }
    }
  }
}
