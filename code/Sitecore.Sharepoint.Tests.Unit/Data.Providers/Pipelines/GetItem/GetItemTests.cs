// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetItemTests.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the GetItemTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.Pipelines.GetItem
{
  using System.Diagnostics.CodeAnalysis;

  using FluentAssertions;

  using NSubstitute;

  using Ploeh.AutoFixture.Xunit2;

  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Globalization;
  using Sitecore.Pipelines.ItemProvider.GetItem;
  using Sitecore.SecurityModel;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Pipelines.GetItem;

  using Xunit;
  using Xunit.Extensions;

  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
  public class GetItemTests
  {
    private readonly GetItem processor = new GetItem();
    private readonly ItemProviderBase itemProvider = Substitute.For<ItemProviderBase>();

    [Fact]
    public void ShouldReturnItemById()
    {
      // Arrange
      Item item = Substitute.For<Database>().CreateConfigurationFolder();

      var args = this.CreateItemArgs(item);

      // Act
      this.processor.Process(args);

      // Assert
      args.Handled.Should().BeTrue();
      args.Result.Should().Be(item);
    }

    [Fact]
    public void ShouldReturnItemByPath()
    {
      // Arrange
      Item item = new ItemMock().AsConfigurationItem();
      var language = Substitute.For<Language>();

      this.itemProvider.GetItem("path", language, Version.Latest, item.Database, SecurityCheck.Disable).Returns(item);
      var args = new GetItemArgs(this.itemProvider, "path", language, Version.Latest, item.Database, SecurityCheck.Disable, false);

      // Act
      this.processor.Process(args);

      // Assert
      args.Handled.Should().BeTrue();
      args.Result.Should().Be(item);
    }

    [Fact]
    public void ShouldStartSyncForFolderParent()
    {
      // Arrange
      Item item = Substitute.For<Database>().CreateConfigurationFolder();

      IntegrationCache.RemoveIntegrationConfigData(item.ParentID);
      var args = this.CreateItemArgs(item);

      // Act
      this.processor.Process(args);

      // Assert
      item.ParentID.WasSyncStarted().Should().BeTrue();
    }

    [Fact]
    public void ShouldStartSyncForDataItemParent()
    {
      // Arrange
      Item item = new ItemMock().AsIntegrationItem().WithParent(new ItemMock());
      new IntegrationItem(item).IsActive = true;
      var args = this.CreateItemArgs(item);

      // Act
      this.processor.Process(args);

      // Assert
      item.ParentID.WasSyncStarted().Should().BeTrue();
    }

    [Fact]
    public void ShouldNotStartSyncIfIntegrationDisabled()
    {
      // Arrange
      Item item = Substitute.For<Database>().CreateConfigurationFolder();
      IntegrationCache.RemoveIntegrationConfigData(item.ID);
      var args = this.CreateItemArgs(item);

      // Act
      using (new IntegrationDisabler())
      {
        this.processor.Process(args);
      }

      // Assert
      item.ParentID.WasSyncStarted().Should().BeFalse();
    }

    [Fact]
    public void ShouldNotStartSyncIfItemIsNull()
    {
      // Arrange
      Item item = Substitute.For<Database>().CreateConfigurationFolder();
      IntegrationCache.RemoveIntegrationConfigData(item.ID);
      var args = this.CreateItemArgs(item);

      // Act
      this.processor.Process(args);

      // Assert
      item.ParentID.WasSyncStarted().Should().BeFalse();
    }

    [Theory, AutoData]
    public void ShouldNotStartSyncForRegularItem(ID templateId)
    {
      // Arrange
      Item item = new ItemMock().WithTemplate(templateId).WithParent(new ItemMock().WithTemplate(templateId));
      var args = this.CreateItemArgs(item);
      args.Result = item;
      args.Handled = true;

      // Act
      this.processor.Process(args);

      // Assert
      item.ParentID.WasSyncStarted().Should().BeFalse();
    }

    [Fact]
    public void ShouldNotGetItemIfHandled()
    {
      // Arrange
      var item = Substitute.For<Database>().CreateConfigurationFolder();
      var args = this.CreateItemArgs(item);
      args.Handled = true;

      // Act
      this.processor.Process(args);

      // Assert
      this.itemProvider.DidNotReceiveWithAnyArgs()
        .GetItem(default(ID), default(Language), default(Version), default(Database), default(SecurityCheck));
    }

    private GetItemArgs CreateItemArgs(Item item)
    {
      var language = Substitute.For<Language>();

      this.itemProvider.GetItem(item.ID, language, Version.Latest, item.Database, SecurityCheck.Disable).Returns(item);

      var args = new GetItemArgs(
        this.itemProvider,
        item.ID,
        language,
        Version.Latest,
        item.Database,
        SecurityCheck.Disable,
        false);

      return args;
    }
  }
}
