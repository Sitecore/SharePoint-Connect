// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncActionFactoryTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SyncActionFactoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.SyncActions
{
  using System.Linq;
  using FluentAssertions;

  using Sitecore.Sharepoint.Data.Providers.SyncActions;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;
  using Xunit;

  public class SyncActionFactoryTests
  {
    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_return_right_create_integration_item_action()
    {
      // Arrange
      var syncActionFactory = new SyncActionFactory();

      var args = new SynchronizeTreeArgs();
      var sharepointItem = TestHelper.CreateSharepointItemsList(new TestHelper.Items { "1" }).First();

      // Act
      var action = syncActionFactory.GetCreateIntegrationItemAction(args, sharepointItem);

      // Assert
      action.Should().Match<CreateIntegrationItemAction>(x => x.SharepointItem == sharepointItem);
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_return_right_update_integration_item_action()
    {
      // Arrange
      var syncActionFactory = new SyncActionFactory();

      var args = new SynchronizeTreeArgs();
      var integrationItem = TestHelper.CreateSitecoreItemsList(new TestHelper.Items { "1" }).First();
      var sharepointItem = TestHelper.CreateSharepointItemsList(new TestHelper.Items { "1" }).First();

      // Act
      var action = syncActionFactory.GetUpdateIntegrationItemAction(args, integrationItem, sharepointItem);

      // Assert
      action.Should().Match<UpdateIntegrationItemAction>(x => x.SharepointItem == sharepointItem);
    }

    [Fact]
    public void should_return_right_delete_integration_item_action()
    {
      // Arrange
      var syncActionFactory = new SyncActionFactory();

      var args = new SynchronizeTreeArgs();
      var integrationItem = TestHelper.CreateSitecoreItemsList(new TestHelper.Items { "1" }).First();

      // Act
      var action = syncActionFactory.GetDeleteIntegrationItemAction(args, integrationItem);

      // Assert
      action.Should().Match<DeleteIntegrationItemAction>(x => x.Item == integrationItem);
    }

    [Fact]
    public void should_return_right_create_sharepoint_item_action()
    {
      // Arrange
      var syncActionFactory = new SyncActionFactory();

      var args = new SynchronizeTreeArgs();
      var integrationItem = TestHelper.CreateSitecoreItemsList(new TestHelper.Items { { "1", string.Empty } }).First();

      // Act
      var action = syncActionFactory.GetCreateSharepointItemAction(args, integrationItem);

      // Assert
      action.Should().Match<CreateSharepointItemAction>(x => x.Item == integrationItem);
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_return_right_update_sharepoint_item_action()
    {
      // Arrange
      var syncActionFactory = new SyncActionFactory();

      var args = new SynchronizeTreeArgs();
      var integrationItem = TestHelper.CreateSitecoreItemsList(new TestHelper.Items { "1" }).First();
      var sharepointItem = TestHelper.CreateSharepointItemsList(new TestHelper.Items { "1" }).First();

      // Act
      var action = syncActionFactory.GetUpdateSharepointItemAction(args, sharepointItem, integrationItem);

      // Assert
      action.Should().Match<UpdateSharepointItemAction>(x => x.Item == integrationItem);
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_return_right_delete_sharepoint_item_action()
    {
      // Arrange
      var syncActionFactory = new SyncActionFactory();

      var args = new SynchronizeTreeArgs();
      var sharepointItem = TestHelper.CreateSharepointItemsList(new TestHelper.Items { "1" }).First();

      // Act
      var action = syncActionFactory.GetDeleteSharepointItemAction(args, sharepointItem);

      // Assert
      action.Should().Match<DeleteSharepointItemAction>(x => x.SharepointItem == sharepointItem);
    }
  }
}
