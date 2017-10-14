// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MergeListsTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the MergeListsTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.Pipelines.SynchronizeTree.Processors
{
  using System;
  using System.Collections.Generic;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Sharepoint.Data.Providers.SyncActions;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree.Helpers;
  using Sitecore.Sharepoint.Tests.Unit.Data.Providers.SyncActions.ComparableActions;
  using Xunit;
  using SpBaseItem = Sitecore.Sharepoint.ObjectModel.Entities.Items.BaseItem;

  public class MergeListsTests
  {
    private readonly MergeLists processor;

    private readonly HistoryProviderBase historyProvider;

    public MergeListsTests()
    {
      this.historyProvider = Substitute.For<HistoryProviderBase>();
      this.processor = new MergeLists(new ComparableSyncActionFactory(), this.historyProvider);
    }

    [Fact]
    public void should_throw_if_args_is_not_set()
    {
      // Arrange

      // Act
      Action action = () => this.processor.Process(null);

      // Assert
      action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: args");
    }

    [Fact]
    public void should_throw_if_integration_items_list_is_not_set()
    {
      // Arrange

      // Act
      Action action = () => this.processor.Process(new SynchronizeTreeArgs());

      // Assert
      action.ShouldThrow<InvalidOperationException>().WithMessage("Value can't be null: args.IntegrationItems");
    }

    [Fact]
    public void should_throw_if_sharepoint_items_list_is_not_set()
    {
      // Arrange

      // Act
      Action action = () => this.processor.Process(new SynchronizeTreeArgs { IntegrationItems = new List<IntegrationItem>() });

      // Assert
      action.ShouldThrow<InvalidOperationException>().WithMessage("Value can't be null: args.SharepointItemList");
    }

    [Fact]
    public void should_throw_if_context_is_not_set()
    {
      // Arrange

      // Act
      Action action = () => this.processor.Process(new SynchronizeTreeArgs { IntegrationItems = new List<IntegrationItem>(), SharepointItemList = new List<SpBaseItem>() });

      // Assert
      action.ShouldThrow<InvalidOperationException>().WithMessage("Value can't be null: args.Context");
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_not_detect_changes_is_sitecore_if_bidirectional_is_of()
    {
      // Arrange
      var args = this.CreateArgs(new TestHelper.Items { "1" }, new TestHelper.Items { "1" }, false);
      this.historyProvider.IsItemChanged(args, "1").Returns(true);

      // Act
      this.processor.Process(args);

      // Assert
      args.ActionList.Should().BeEquivalentTo(new List<SyncActionBase> { new ComparableUpdateIntegrationItemAction(args.IntegrationItems[0], args.SharepointItemList[0], args.Context, new ProcessIntegrationItemsOptions()) });
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_not_detect_deleting_is_sitecore_if_bidirectional_is_of()
    {
      // Arrange
      var args = this.CreateArgs(null, new TestHelper.Items { "1" }, false);
      this.historyProvider.IsItemDeleted(args, "1").Returns(true);

      // Act
      this.processor.Process(args);

      // Assert
      args.ActionList.Should().BeEquivalentTo(new List<SyncActionBase> { new ComparableCreateIntegrationItemAction(args.SharepointItemList[0], args.Context, ProcessIntegrationItemsOptions.DefaultOptions) });
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_detect_new_sharepoint_items()
    {
      // Arrange
      var args = this.CreateArgs(new TestHelper.Items { "1", "3" }, new TestHelper.Items { "0", "1", "2", "3", "4" });

      var syncActions = new List<SyncActionBase>
      {
        new ComparableCreateIntegrationItemAction(args.SharepointItemList[0], null, ProcessIntegrationItemsOptions.DefaultOptions),
        new ComparableCreateIntegrationItemAction(args.SharepointItemList[2], null, ProcessIntegrationItemsOptions.DefaultOptions),
        new ComparableCreateIntegrationItemAction(args.SharepointItemList[4], null, ProcessIntegrationItemsOptions.DefaultOptions),
      };

      // Act
      this.processor.Process(args);

      // Assert
      args.ActionList.Should().Contain(syncActions);
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_detect_new_sitecore_items()
    {
      // Arrange
      var args = this.CreateArgs(new TestHelper.Items { { "1", string.Empty }, "3" }, new TestHelper.Items { "3" });

      // Act
      this.processor.Process(args);

      // Assert
      args.ActionList.Should().Contain(new ComparableCreateSharepointItemAction(args.IntegrationItems[0], null));
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_detect_updated_items_in_sitecore()
    {
      // Arrange
      var args = this.CreateArgs(new TestHelper.Items { "1", "2", "3" }, new TestHelper.Items { "1", "2", "3" });

      this.historyProvider.IsItemChanged(args, args.SharepointItemList[0].GUID).Returns(true);

      // Act
      this.processor.Process(args);

      // Assert
      args.ActionList.Should().Contain(new ComparableUpdateSharepointItemAction(null, args.IntegrationItems[0], null));
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_detect_deleted_items_in_sitecore()
    {
      // Arrange
      var args = this.CreateArgs(new TestHelper.Items { "1", "2" }, new TestHelper.Items { "1", "2", "3" });

      this.historyProvider.IsItemDeleted(args, args.SharepointItemList[2].GUID).Returns(true);

      // Act
      this.processor.Process(args);

      // Assert
      args.ActionList.Should().Contain(new ComparableDeleteSharepointItemAction(args.SharepointItemList[2], null));
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_detect_deleted_items_in_sharepoint()
    {
      // Arrange
      var args = this.CreateArgs(new TestHelper.Items { "1", "2", "3" }, new TestHelper.Items { "1", "3" });

      // Act
      this.processor.Process(args);

      // Assert
      args.ActionList.Should().Contain(new ComparableDeleteIntegrationItemAction(args.IntegrationItems[1], null));
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_generate_right_action_list()
    {
      // Arrange
      var args = this.CreateArgs(new TestHelper.Items { { "1", string.Empty }, "2", "3", "5" }, new TestHelper.Items { "3", "4", "5", "6" });

      this.historyProvider.IsItemChanged(args, args.SharepointItemList[0].GUID).Returns(true);
      this.historyProvider.IsItemDeleted(args, args.SharepointItemList[1].GUID).Returns(true);

      var syncActions = new List<SyncActionBase>
      {
        new ComparableCreateSharepointItemAction(args.IntegrationItems[0], null),
        new ComparableDeleteIntegrationItemAction(args.IntegrationItems[1], null),
        new ComparableUpdateSharepointItemAction(args.SharepointItemList[0], args.IntegrationItems[2], null),
        new ComparableDeleteSharepointItemAction(args.SharepointItemList[1], null),
        new ComparableUpdateIntegrationItemAction(args.IntegrationItems[3], args.SharepointItemList[2], null, ProcessIntegrationItemsOptions.DefaultAsyncOptions),
        new ComparableCreateIntegrationItemAction(args.SharepointItemList[3], null, ProcessIntegrationItemsOptions.DefaultOptions),
      };

      // Act
      this.processor.Process(args);

      // Assert
      args.ActionList.Should().BeEquivalentTo(syncActions);
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_not_detect_not_active_interation_items()
    {
      // Arrange
      // args with not active integration items: first of them is deleted in sharepoint and second is edited in sitecore.
      var args = this.CreateArgs(new TestHelper.Items { "1", "2" }, new TestHelper.Items { "2" });
      this.historyProvider.IsItemChanged(Arg.Any<SynchronizeTreeArgs>(), "2").Returns(true);

      args.IntegrationItems.ForEach(x => x.IsActive = false);

      // Act
      this.processor.Process(args);

      // Assert
      args.ActionList.Should().BeEmpty();
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_not_generate_upgrade_action_if_item_is_deleted()
    {
      // Arrange
      var args = this.CreateArgs(new TestHelper.Items { "1" }, new TestHelper.Items { "1" });

      this.historyProvider.IsItemDeleted(args, args.SharepointItemList[0].GUID).Returns(true);

      // Act
      this.processor.Process(args);

      // Assert
      args.ActionList.Should().Contain(new ComparableDeleteSharepointItemAction(args.SharepointItemList[0], null));
    }

    private SynchronizeTreeArgs CreateArgs(
      TestHelper.Items sitecoreItems,
      TestHelper.Items sharepointItems,
      bool isBidirectional = true)
    {
      var configData = new IntegrationConfigData("server", "list", "templateID") { BidirectionalLink = isBidirectional };

      var args = new SynchronizeTreeArgs
                   {
                     IntegrationItems = TestHelper.CreateSitecoreItemsList(sitecoreItems),
                     SharepointItemList = TestHelper.CreateSharepointItemsList(sharepointItems),
                     Context = new SynchContext(new ItemMock().AsConfigurationItem(configData))
                   };

      return args;
    }
  }
}
