// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSitecoreItemListTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the GetSitecoreItemsTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.Pipelines.SynchronizeTree.Processors
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using Sitecore.Data;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;
  using Xunit;

  public class GetSitecoreItemListTests
  {
    [Fact]
    public void should_throw_if_args_is_null()
    {
      // Arrange
      var processor = new GetSitecoreItemList();

      // Act
      Action action = () => processor.Process(null);

      // Assert
      action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: args");
    }

    [Fact]
    public void should_throw_if_config_item_is_not_specified()
    {
      // Arrange
      var processor = new GetSitecoreItemList();
      var args = new SynchronizeTreeArgs();

      // Act
      Action action = () => processor.Process(args);

      // Assert
      action.ShouldThrow<InvalidOperationException>().WithMessage("Value can't be null: args.Context");
    }

    [Fact]
    public void should_should_fill_args_with_integration_items_only()
    {
      // Arrange
      var processor = new GetSitecoreItemList();

      var args = new SynchronizeTreeArgs { Context = CreateSyncContext() };

      // Act
      processor.Process(args);

      // Assert
      args.IntegrationItems.Should().NotBeNull();
      args.IntegrationItems.Select(x => x.Name)
        .Should()
        .Contain(new List<string> { "activeIntegrationItem", "notActiveIntegrationItem" });
    }

    [Fact]
    public void should_should_not_fill_args_with_not_integration_items_only()
    {
      // Arrange
      var processor = new GetSitecoreItemList();

      var args = new SynchronizeTreeArgs { Context = CreateSyncContext() };

      // Act
      processor.Process(args);

      // Assert
      args.IntegrationItems.Should().NotBeNull();
      args.IntegrationItems.Select(x => x.Name).Should().NotContain("notIntegrationItem");
    }

    private static ItemMock PrepareDbWithItemTree()
    {
      return new ItemMock
        {
          new ItemMock { { "some", Sharepoint.Common.FieldIDs.IsIntegrationItem, "1" } }
          .WithName("activeIntegrationItem").WithTemplate(Sharepoint.Common.TemplateIDs.IntegrationBase),

          new ItemMock { { "some", Sharepoint.Common.FieldIDs.IsIntegrationItem, "0" } }
          .WithName("notActiveIntegrationItem").WithTemplate(Sharepoint.Common.TemplateIDs.IntegrationBase),

          new ItemMock().WithTemplate(ID.NewID)
        };
    }

    private static SynchContext CreateSyncContext()
    {
      var item = PrepareDbWithItemTree();
      IntegrationCache.AddIntegrationConfigData(item.ID, new IntegrationConfigData("server", "list", "templateID"), 0);
      return new SynchContext(item);
    }
  }
}
