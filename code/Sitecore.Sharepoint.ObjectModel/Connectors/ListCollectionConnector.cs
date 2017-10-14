// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListCollectionConnector.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ListCollectionConnector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Connectors
{
  using System;
  using System.Net;
  using System.Xml;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.WebServices;
  using Sitecore.Sharepoint.Data.WebServices.SharepointLists;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Utils;

  public class ListCollectionConnector
  {
    protected readonly Lists ListsWebService;

    public ListCollectionConnector([NotNull] SpContext context, [NotNull] Uri webUrl)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");

      this.ListsWebService = new Lists();
      this.ListsWebService.SetServer(webUrl, context);
    }

    [NotNull]
    public EntityValues GetLists()
    {
      XmlNode listsNode = this.ListsWebService.GetListCollection();

      var lists = new EntityValues();
      foreach (XmlNode listNode in listsNode.ChildNodes)
      {
        var list = new EntityValues
        {
          Properties = XmlUtils.LoadProperties(listNode)
        };

        lists.Add("Lists", list);
      }

      return lists;
    }
  }
}
