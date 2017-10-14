// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentTypeConnector.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ContentTypeConnector type.
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

  /// <summary>
  /// Provides methods for work with fields.
  /// </summary>
  public class ContentTypeConnector
  {
    protected Lists ListsWebService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentTypeConnector"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="webUrl">The web URL.</param>
    public ContentTypeConnector([NotNull] SpContext context, [NotNull] Uri webUrl)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");

      this.ListsWebService = new Lists();
      this.ListsWebService.SetServer(webUrl, context);
    }

    [NotNull]
    public EntityValues GetFields([NotNull] string listName, [NotNull] string contentTypeID)
    {
      Assert.ArgumentNotNull(listName, "listName");
      Assert.ArgumentNotNull(contentTypeID, "contentTypeID");

      XmlNode contentTypeNode = this.ListsWebService.GetListContentType(listName, contentTypeID);

      return XmlUtils.GetEntityValues(contentTypeNode);
    }
  }
}
