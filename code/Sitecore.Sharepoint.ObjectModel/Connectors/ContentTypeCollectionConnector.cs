// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentTypeCollectionConnector.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ContentTypeCollectionConnector class.
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

  public class ContentTypeCollectionConnector
  {
    protected readonly Lists ListsWebService;

    public ContentTypeCollectionConnector([NotNull] SpContext context, [NotNull] Uri webUrl)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");

      this.ListsWebService = new Lists();
      this.ListsWebService.SetServer(webUrl, context);

    }

    [NotNull]
    public EntityValues GetContentTypes([NotNull] string listName)
    {
      Assert.ArgumentNotNull(listName, "listName");

      XmlNode contentTypesNode = this.ListsWebService.GetListContentTypes(listName, "0x01");

      var xmlNamespaceManager = new XmlNamespaceManager(contentTypesNode.OwnerDocument.NameTable);
      xmlNamespaceManager.AddNamespace("a", "http://schemas.microsoft.com/sharepoint/soap/");

      var contentTypes = new EntityValues();
      foreach (XmlNode contentTypeNode in contentTypesNode.SelectNodes("//a:ContentType", xmlNamespaceManager))
      {
        var contentType = new EntityValues
        {
          Properties = XmlUtils.LoadProperties(contentTypeNode)
        };

        contentTypes.Add("ContentTypes", contentType);
      }

      return contentTypes;
    }
  }
}
