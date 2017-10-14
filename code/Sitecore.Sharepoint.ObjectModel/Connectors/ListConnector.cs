// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListConnector.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ListConnector class.
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
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;
  using Sitecore.Sharepoint.ObjectModel.Utils;
  using Sitecore.StringExtensions;

  /// <summary>
  /// Provides possibility to work with SharePoint lists.
  /// </summary>
  public class ListConnector
  {
    protected readonly Lists ListsWebService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListConnector"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="webUrl">The web URL.</param>
    /// <exception cref="Exception"><c>Throws exception if connector can't be created.</c>.</exception>
    public ListConnector([NotNull] SpContext context, [NotNull] Uri webUrl)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");

      try
      {
        this.ListsWebService = new Lists();
        this.ListsWebService.SetServer(webUrl, context);
      }
      catch (Exception ex)
      {
        string errorMessage = "List connector can't be created. Additional info:\n Server URL: {0}, webUrl: {1} ".FormatWith(context.Url, webUrl);
        Log.Error(errorMessage, ex, this);

        throw new Exception(errorMessage, ex);
      }
    }

    /// <summary>
    /// Return information about list.
    /// </summary>
    /// <param name="name">The name of the list.</param>
    /// <returns>List information.</returns>
    [NotNull]
    public EntityValues GetList([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, "name");

      XmlNode listNode = this.ListsWebService.GetList(name);

      return XmlUtils.GetEntityValues(listNode);
    }

    /// <summary>
    /// Deletes the specified SharePoint list from Web.
    /// </summary>
    /// <param name="list">The specified Sharepoint list.</param>
    public void DeleteList([NotNull] BaseList list)
    {
      Assert.ArgumentNotNull(list, "list");

      this.ListsWebService.DeleteList(list.ID);
    }

    /// <summary>
    /// Create new List in the Web.
    /// </summary>
    /// <param name="listName">The list name.</param>
    /// <param name="description">The description.</param>
    /// <param name="templateID">The template id.</param>
    /// <returns></returns>
    [NotNull]
    public EntityValues AddList([NotNull] string listName, [NotNull] string description, int templateID)
    {
      Assert.ArgumentNotNull(listName, "listName");
      Assert.ArgumentNotNull(description, "description");

      XmlNode listNode = this.ListsWebService.AddList(listName, description, templateID);

      return XmlUtils.GetEntityValues(listNode);
    }
  }
}