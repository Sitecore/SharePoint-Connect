// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewConnector.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ViewConnector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Sitecore.Sharepoint.ObjectModel.Connectors
{
  using System;
  using System.Collections.Generic;
  using System.Net;
  using System.Xml;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.WebServices;
  using Sitecore.Sharepoint.Data.WebServices.SharepointViews;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Utils;

  /// <summary>
  /// Provide methods for work with Views.
  /// </summary>
  public class ViewConnector
  {
    protected readonly Views ViewsWebService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewConnector"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="webUrl">The web URL.</param>
    public ViewConnector([NotNull] SpContext context, [NotNull] Uri webUrl)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");

      this.ViewsWebService = new Views();
      this.ViewsWebService.SetServer(webUrl, context);
    }

    /// <summary>
    /// Return information about current view in the list.
    /// </summary>
    /// <param name="listName">Name of the list.</param>
    /// <param name="viewName">Name of the view.</param>
    /// <returns>
    /// Information about current view in the list
    /// </returns>
    [NotNull]
    public EntityValues GetView([NotNull] string listName, [NotNull] string viewName)
    {
      Assert.ArgumentNotNull(listName, "listName");
      Assert.ArgumentNotNull(viewName, "viewName");

      XmlNode viewNode = this.ViewsWebService.GetView(listName, viewName);

      var view = new EntityValues
      {
        Properties = XmlUtils.LoadProperties(viewNode)
      };

      var viewFields = new List<EntityValues>();
      foreach (XmlNode viewFieldNode in XmlUtils.Select(viewNode, "/f:ViewFields/f:FieldRef"))
      {
        var viewField = new EntityValues
        {
          Properties = XmlUtils.LoadProperties(viewFieldNode)
        };

        viewFields.Add(viewField);
      }

      view.AddRange("ViewFields", viewFields);

      return view;
    }
  }
}
