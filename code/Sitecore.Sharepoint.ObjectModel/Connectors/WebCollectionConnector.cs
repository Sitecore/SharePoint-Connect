// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebCollectionConnector.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines WebCollectionConnector class.
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

  public class WebCollectionConnector
  {
    protected readonly Webs WebsWebService;

    public WebCollectionConnector([NotNull] SpContext context, [NotNull] Uri webUrl)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");

      this.WebsWebService = new Webs();
      this.WebsWebService.SetServer(webUrl, context);
    }

    [NotNull]
    public EntityValues GetWebs()
    {
      XmlNode websNode = this.WebsWebService.GetWebCollection();

      var webs = new EntityValues();
      foreach (XmlNode webNode in websNode.ChildNodes)
      {
        var web = new EntityValues
        {
          Properties = XmlUtils.LoadProperties(webNode)
        };

        webs.Add("Webs", web);
      }

      return webs;
    }
  }
}
