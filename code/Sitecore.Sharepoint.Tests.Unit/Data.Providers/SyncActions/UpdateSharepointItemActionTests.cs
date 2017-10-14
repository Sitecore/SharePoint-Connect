// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSharepointItemActionTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the UpdateSharepointItemActionTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.SyncActions
{
  using FluentAssertions;

  using Sitecore.Data.Items;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.Pipelines;
  using Sitecore.Sharepoint.Data.Providers.SyncActions;
  using Sitecore.Sharepoint.Pipelines.ProcessSharepointItem;
  using Xunit;

  public class UpdateSharepointItemActionTests
  {
    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_call_pipeline()
    {
      // Arrange
      using (var pipelines = new PipelinesHandler())
      {
        var updateItem = new ItemMock();
        Item item = new ItemMock().AsConfigurationItem().WithChild(updateItem);
        var synchContext = new SynchContext(item);
        var sharepointItem = TestHelper.CreateSharepointItem();
        
        var action = new UpdateSharepointItemAction(sharepointItem, new IntegrationItem(updateItem), synchContext);

        // Act
        action.Execute();
        action.IsSuccessful.Should().BeTrue();

        // Assert
        pipelines.ShouldReceiveCall<ProcessSharepointItemArgs>(
          PipelineNames.UpdateSharepointItem,
          x => x.SharepointItem == sharepointItem && x.SourceIntegrationItem == (Item)updateItem
               && x.SynchContext == synchContext);
      }
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_be_not_successsfull_if_pipeline_throw_exception()
    {
      // Arrange
      using (new PipelinesHandler().ThrowInPipeline(PipelineNames.UpdateSharepointItem))
      {
        var synchContext = new SynchContext(new ItemMock().AsConfigurationItem());
        var action = new UpdateSharepointItemAction(TestHelper.CreateSharepointItem(), new IntegrationItem(new ItemMock().AsIntegrationItem()), synchContext);

        // Act
        action.Execute();

        // Assert
        action.IsSuccessful.Should().BeFalse();
      }
    }
  }
}
