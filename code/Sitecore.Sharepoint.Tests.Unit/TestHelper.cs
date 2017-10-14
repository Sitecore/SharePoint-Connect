// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestHelper.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the TestHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit
{
  using System;
  using System.Collections.Generic;

  using NSubstitute;

  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.ObjectModel;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;

  using BaseItem = Sitecore.Sharepoint.ObjectModel.Entities.Items.BaseItem;

  internal static class TestHelper
  {
    public static List<BaseItem> CreateSharepointItemsList(Items collection)
    {
      var items = new List<BaseItem>();
      if (collection != null)
      {
        var dataContext = Substitute.For<SpContext>();
        var list = Substitute.For<BaseList>(new EntityValues(), dataContext, new Uri("http://empty"));

        foreach (KeyValuePair<string, string> item in collection)
        {
          var sharepointItem = Substitute.For<BaseItem>(new EntityProperties(), list, dataContext);
          sharepointItem.Title.Returns(item.Key);
          sharepointItem.GUID.Returns(item.Value);

          items.Add(sharepointItem);
        }
      }

      return items;
    }

    public static List<IntegrationItem> CreateSitecoreItemsList(Items collection)
    {
      var items = new List<IntegrationItem>();
      if (collection == null)
      {
        return items;
      }

      foreach (KeyValuePair<string, string> item in collection)
      {
        var fakeItem = new ItemMock().AsIntegrationItem(item.Value, true).WithDataModifiedField();
        items.Add(new IntegrationItem(fakeItem));
      }

      return items;
    }

    public static BaseItem CreateSharepointItem()
    {
      var dataContext = Substitute.For<SpContext>();
      var list = Substitute.For<BaseList>(new EntityValues(), dataContext, new Uri("http://empty"));
      return Substitute.For<BaseItem>(new EntityProperties(), list, dataContext);
    }

    internal class Items : GenericKeyValurPairCollection<string, string>
    {
      public override void Add(string guid)
      {
        this.List.Add(new KeyValuePair<string, string>(guid, guid));
      }
    }
  }
}
