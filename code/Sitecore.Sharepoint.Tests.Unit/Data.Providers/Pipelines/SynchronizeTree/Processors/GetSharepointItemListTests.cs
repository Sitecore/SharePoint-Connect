// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSharepointItemListTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the GetSharepointItemListTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.Pipelines.SynchronizeTree.Processors
{
  using System;
  using System.Collections.Generic;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Data;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.ObjectModel;
  using Sitecore.Sharepoint.ObjectModel.Entities.Collections;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;
  using Sitecore.Sharepoint.ObjectModel.Options;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree.Helpers;
  using Xunit;

  public class GetSharepointItemListTests
  {
    private readonly GetSharepointItemList processor;
    private readonly SharepointListsProviderBase sharepointListProvider;
    private readonly SpContextProviderBase sharepointContextProvider;

    public GetSharepointItemListTests()
    {
      this.sharepointListProvider = Substitute.For<SharepointListsProviderBase>();
      this.sharepointContextProvider = Substitute.For<SpContextProviderBase>();
      this.processor = new GetSharepointItemList(this.sharepointListProvider, this.sharepointContextProvider);
    }

    [Fact]
    public void should_throw_if_arg_is_not_set()
    {
      // Arrange

      // Act
      Action action = () => this.processor.Process(null);

      // Assert
      action.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void should_throw_if_context_is_not_set()
    {
      // Arrange

      // Act
      Action action = () => this.processor.Process(new SynchronizeTreeArgs());

      // Assert
      action.ShouldThrow<InvalidOperationException>().WithMessage("Value can't be null: args.Context");
    }

    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void should_return_all_list_items()
    {
      // Arrange
      var dataContext = Substitute.For<SpContext>();
      var list = Substitute.For<BaseList>(new EntityValues(), dataContext, new Uri("http://empty"));
      var itemCollection = new MyItemCollection(dataContext, list, new ItemsRetrievingOptions());

      var listItems = new List<BaseItem>
      {
        new BaseItem(new EntityProperties(), list, dataContext),
        new BaseItem(new EntityProperties(), list, dataContext),
        new BaseItem(new EntityProperties(), list, dataContext)
      };

      itemCollection.Items = listItems;

      // We have to make these methods as virtual in the BaseList class:
      list.GetItems(Arg.Any<ItemsRetrievingOptions>()).Returns(itemCollection);

      var configData = new IntegrationConfigData("server", "list", "templateID");
      var args = new SynchronizeTreeArgs
      {
        Context = this.CreateSynchContext(configData)
      };

      var context = Substitute.For<SpContext>();
      this.sharepointContextProvider.CreateDataContext(configData).Returns(context);
      this.sharepointListProvider.GetList(configData.Web, configData.List, context).Returns(list);

      // Act
      this.processor.Process(args);

      // Assert
      args.SharepointItemList.Should().BeEquivalentTo(listItems);
    }

    [Fact]
    public void should_request_items_with_correct_options()
    {
      // Arrange
      var configData = new IntegrationConfigData("server", "list", "templateID")
      {
        ItemLimit = 100500,
        Folder = "SomeFolder",
        View = "SomeView"
      };

      var context = Substitute.For<SpContext>();
      var list = Substitute.For<BaseList>(new EntityValues(), context, new Uri("http://empty"));
      var myItemCollection = new MyItemCollection(context, list, new ItemsRetrievingOptions());
      list.GetItems(Arg.Any<ItemsRetrievingOptions>()).Returns(myItemCollection);

      this.sharepointListProvider.GetList(string.Empty, string.Empty, null).ReturnsForAnyArgs(list);

      var args = new SynchronizeTreeArgs
      {
        Context = this.CreateSynchContext(configData)
      };

      // Act
      this.processor.Process(args);

      // Assert
      list.Received().GetItems(Arg.Is<ItemsRetrievingOptions>(x => x.ItemLimit == configData.ItemLimit && x.Folder == configData.Folder && x.ViewName == configData.View));
    }

    private SynchContext CreateSynchContext(IntegrationConfigData integrationConfigData)
    {
      var id = ID.NewID;
      IntegrationCache.AddIntegrationConfigData(id, integrationConfigData, 0);
      var synchContext = new SynchContext(id, Substitute.For<Database>());
      return synchContext;
    }

    // Simplest way to substitute items returned by ItemCollection.
    private class MyItemCollection : ItemCollection
    {
      public MyItemCollection([NotNull] SpContext context, [NotNull] BaseList list, [NotNull] ItemsRetrievingOptions options)
        : base(context, list, options)
      {
        this.Items = new List<BaseItem>();
      }

      public List<BaseItem> Items { get; set; }

      protected override List<BaseItem> GetEntities()
      {
        return this.Items;
      }
    }
  }
}
