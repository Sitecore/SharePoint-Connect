// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationConfigDataProviderTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the IntegrationConfigDataProviderTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.IntegrationConfig
{
  using System;
  using FluentAssertions;
  using Sitecore.Data;
  using Sitecore.Data.Events;
  using Sitecore.Data.Items;
  using Sitecore.Events;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Xunit;

  public class IntegrationConfigDataProviderTests
  {
    private readonly IntegrationConfigDataProvider provider = new IntegrationConfigDataProvider();

    [Fact]
    public void OnItemSaved_should_remove_active_configuration_items_from_cache()
    {
      this.CheckConfigurationItem(this.provider.OnItemSaved, item => new SitecoreEventArgs("testEvent", new object[] { item }, new EventResult()));
    }

    [Fact]
    public void OnItemSaved_should_remove_child_configuration_folderss_from_cache()
    {
      // Arrange Act Assert
      this.CheckFoldersStructure(this.provider.OnItemSaved, item => new SitecoreEventArgs("testEvent", new object[]{ item }, new EventResult()));
    }

    [Fact]
    public void OnItemSaved_should_asset_null_item()
    {
      // Arrange Act Assert
      Action action = () => this.provider.OnItemSaved(this, new SitecoreEventArgs("testEvent", new object[] { null }, new EventResult()));

      // Assert
      action.ShouldThrow<InvalidOperationException>().WithMessage("No item in parameters");
    }

    [Fact]
    public void OnItemSaved_should_assert_sender()
    {
      // Act
      Action action = () => this.provider.OnItemSaved(null, new EventArgs());

      // Assert
      action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: sender");
    }

    [Fact]
    public void OnItemSavedRemote_should_remove_active_configuration_items_from_cache()
    {
      this.CheckConfigurationItem(
        this.provider.OnItemSavedRemote,
        item =>
        {
          var itemChanges = new ItemChanges(item);
          itemChanges.SetFieldValue(item.Fields[FieldIDs.IntegrationConfigData], "changed");
          return new ItemSavedRemoteEventArgs(item, itemChanges);
        });
    }

    [Fact]
    public void OnItemSavedRemote_should_remove_child_configuration_folderss_from_cache()
    {
      // Act
      this.CheckFoldersStructure(
        this.provider.OnItemSavedRemote,
        item =>
        {
          var itemChanges = new ItemChanges(item);
          itemChanges.SetFieldValue(item.Fields[FieldIDs.IntegrationConfigData], "changed");
          return new ItemSavedRemoteEventArgs(item, itemChanges);
        });
    }

    [Fact]
    public void OnItemSavedRemote_should_not_throw_if_args_is_not_ItemSavedRemoteEventArgs()
    {
      // Act
      Action action = () => this.provider.OnItemSavedRemote(this, new EventArgs());

      // Assert
      action.ShouldNotThrow();
    }

    [Fact]
    public void OnItemSavedRemote_should_assert_args()
    {
      // Act
      Action action = () => this.provider.OnItemSavedRemote(this, null);

      // Assert
      action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: args");
    }


    [Fact]
    public void OnItemSavedRemote_should_assert_sender()
    {
      // Act
      Action action = () => this.provider.OnItemSavedRemote(null, new EventArgs());

      // Assert
      action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: sender");
    }

    private void CheckConfigurationItem(Action<object, EventArgs> action, Func<Item, EventArgs> constructArgs)
    {
      // Arrange
      Item configurationItem = new ItemMock().AsConfigurationItem().WithField(FieldIDs.IntegrationConfigData, string.Empty);
      using (new ItemIsContentItem(configurationItem))
      {
        var eventArgs = constructArgs(configurationItem);

        // Act
        action(this, eventArgs);

        // Assert
        IntegrationCache.GetIntegrationConfigData(configurationItem.ID).Should().BeNull();
      }
    }

    private void CheckFoldersStructure(Action<object, EventArgs> action, Func<Item, EventArgs> constructArgs)
    {
      var subFolderId = ID.NewID;
      var subFolder = new ItemMock(subFolderId).WithTemplate(TemplateIDs.IntegrationFolder).PutConfigDataToCache();

      var folderId = ID.NewID;
      var folder = new ItemMock(folderId)
        .WithChild(subFolder)
        .WithTemplate(TemplateIDs.IntegrationFolder).PutConfigDataToCache();


      var configurationItem = new ItemMock().AsConfigurationItem().WithChild(folder).WithField(FieldIDs.IntegrationConfigData, string.Empty);
      using (new ItemIsContentItem(configurationItem))
      {
        var eventArgs = constructArgs(configurationItem);

        // Act
        action(this, eventArgs);

        // Assert
        IntegrationCache.GetIntegrationConfigData(folderId).Should().BeNull();
        IntegrationCache.GetIntegrationConfigData(subFolderId).Should().BeNull();
      }
    }
  }
}
