// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetChildrenTests.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the GetChildrenTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.Pipelines.GetChildren
{
  using System.Diagnostics.CodeAnalysis;

  using FluentAssertions;

  using NSubstitute;

  using Sitecore.Collections;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Pipelines.ItemProvider.GetChildren;
  using Sitecore.SecurityModel;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Pipelines.GetChildren;

  using Xunit;

  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
  public class GetChildrenTests
  {
    private readonly GetChildren processor = new GetChildren();
    private readonly ItemProviderBase itemProvider = Substitute.For<ItemProviderBase>();

    [Fact]
    public void ShouldStartSync()
    {
      // Arrange
      Item item = new ItemMock().AsConfigurationItem();

      using (new ItemIsContentItem(item))
      {
        IntegrationCache.RemoveIntegrationConfigData(item.ID);
        var args = new GetChildrenArgs(this.itemProvider, item, SecurityCheck.Disable, ChildListOptions.None);

        // Act
        this.processor.Process(args);

        // Assert
        item.ID.WasSyncStarted().Should().BeTrue();
      }
    }

    [Fact]
    public void ShouldStartSyncForFolder()
    {
      Item item = Substitute.For<Database>().CreateConfigurationFolder();
      using (new ItemIsContentItem(item))
      {
        // Arrange
        IntegrationCache.RemoveIntegrationConfigData(item.ID);
        var args = new GetChildrenArgs(this.itemProvider, item, SecurityCheck.Disable, ChildListOptions.None);

        // Act
        this.processor.Process(args);

        // Assert
        item.ID.WasSyncStarted().Should().BeTrue();
      }
    }

    [Fact]
    public void ShouldNotStartSyncIfIntegrationDisabled()
    {
      // Arrange
      Item item = new ItemMock();
      IntegrationCache.RemoveIntegrationConfigData(item.ID);
      var args = new GetChildrenArgs(this.itemProvider, item, SecurityCheck.Disable, ChildListOptions.None);

      // Act
      using (new IntegrationDisabler())
      {
        this.processor.Process(args);
      }

      // Assert
      item.ID.WasSyncStarted().Should().BeFalse();
    }

    [Fact]
    public void ShouldNotStartSyncForeRegularItem()
    {
      // Arrange
      Item item = new ItemMock();

      using (new ItemIsContentItem(item))
      {
        var args = new GetChildrenArgs(this.itemProvider, item, SecurityCheck.Disable, ChildListOptions.None);

        // Act
        this.processor.Process(args);

        // Assert
        item.ID.WasSyncStarted().Should().BeFalse();
      }
    }

    [Fact]
    public void ShouldNotGetChildrenIfHandled()
    {
      Item item = Substitute.For<Database>().CreateConfigurationFolder();
      using (new ItemIsContentItem(item))
      {
        // Arrange
        var args = new GetChildrenArgs(this.itemProvider, item, SecurityCheck.Disable, ChildListOptions.None)
                     {
                       Handled = true
                     };

        // Act
        this.processor.Process(args);

        // Assert
        this.itemProvider.DidNotReceiveWithAnyArgs().GetChildren(default(Item), default(SecurityCheck));
      }
    }

    [Fact]
    public void ShouldGetChildren()
    {
      Item item = Substitute.For<Database>().CreateConfigurationFolder();
      using (new ItemIsContentItem(item))
      {
        // Arrange
        var args = new GetChildrenArgs(this.itemProvider, item, SecurityCheck.Disable, ChildListOptions.None);
        var childList = new ChildList(item, new ItemList());
        this.itemProvider.GetChildren(item, SecurityCheck.Disable)
          .Returns(childList)
          .AndDoes(obj => IntegrationDisabler.CurrentValue.Should().BeTrue());

        // Act
        this.processor.Process(args);
        
        // Assert
        args.Handled.Should().BeTrue();
        args.Result.As<object>().Should().BeSameAs(childList);
        IntegrationDisabler.CurrentValue.Should().BeFalse();
      }
    }
  }
}
