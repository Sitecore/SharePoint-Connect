// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebConnector.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines WebConnector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Connectors
{
  using System;
  using System.Net;
  using System.Xml;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.WebServices;
  using Sitecore.Sharepoint.Data.WebServices.SharepointWebs;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Utils;

  /// <summary>
  /// Provides methods for work with Webs.
  /// </summary>
  public class WebConnector
  {
    protected readonly Webs WebsWebService;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebConnector"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="webUrl">The web URL.</param>
    public WebConnector([NotNull] SpContext context, [NotNull] Uri webUrl)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");

      this.WebsWebService = new Webs();
      this.WebsWebService.SetServer(webUrl, context);
    }

    [NotNull]
    public EntityValues GetWeb()
    {
      XmlNode webNode = this.WebsWebService.GetWeb(".");

      var web = new EntityValues
      {
        Properties = XmlUtils.LoadProperties(webNode)
      };

      return web;
    }
  }
}
