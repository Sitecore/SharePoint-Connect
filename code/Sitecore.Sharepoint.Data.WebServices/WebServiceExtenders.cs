// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebServiceExtenders.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the WebServiceExtenders class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.WebServices
{
  using System;
  using System.Web.Services.Protocols;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Common.Wrappers;
  using Sitecore.Sharepoint.Data.WebServices.SharepointLists;

  /// <summary>
  /// This is util class to make webservices usage easier.
  /// </summary>
  public static class WebServiceExtenders
  {
    private static readonly ConnectionConfigurationsFactory ConnectionConfigurationsFactory = new ConnectionConfigurationsFactory(new SitecoreFactory());

    /// <summary>
    /// Sets the url of the target Web service.
    /// </summary>
    /// <param name="service">The target Web service.</param>
    /// <param name="url">The specified url.</param>
    /// <param name="context"> The context of the SharePoint server.</param>
    public static void SetServer(this Lists service, Uri url, SpContextBase context = null)
    {
      SetUpService(service, url, "/_vti_bin/lists.asmx", context);
    }

    /// <summary>
    /// Sets the url of the target Web service.
    /// </summary>
    /// <param name="service">The target Web service.</param>
    /// <param name="url">The specified url.</param>
    /// <param name="context"> The context of the SharePoint server.</param>
    public static void SetServer(this Data.WebServices.SharepointViews.Views service, Uri url, SpContextBase context = null)
    {
      SetUpService(service, url, "/_vti_bin/views.asmx", context);
    }

    /// <summary>
    /// Sets the url of the target Web service.
    /// </summary>
    /// <param name="service">The target Web service.</param>
    /// <param name="url">The specified url.</param>
    /// <param name="context"> The context of the SharePoint server.</param>
    public static void SetServer(this Data.WebServices.SharepointSearch.QueryService service, Uri url, SpContextBase context = null)
    {
      SetUpService(service, url, "/_vti_bin/spsearch.asmx", context);
    }

    /// <summary>
    /// Sets the url of the target Web service.
    /// </summary>
    /// <param name="service">The target Web service.</param>
    /// <param name="url">The specified url.</param>
    /// <param name="context"> The context of the SharePoint server.</param>
    public static void SetServer(this Data.WebServices.SharepointMOSSSearch.QueryService service, Uri url, SpContextBase context = null)
    {
      SetUpService(service, url, "/_vti_bin/search.asmx", context);
    }

    /// <summary>
    /// Sets the url of the target Web service.
    /// </summary>
    /// <param name="service">The target Web service.</param>
    /// <param name="url">The specified url.</param>
    /// <param name="context"> The context of the SharePoint server.</param>
    public static void SetServer(this Data.WebServices.SharepointWebs.Webs service, Uri url, SpContextBase context = null)
    {
      SetUpService(service, url, "/_vti_bin/webs.asmx", context);
    }

    /// <summary>
    /// Sets the url of the target Web service.
    /// </summary>
    /// <param name="service">The target Web service.</param>
    /// <param name="url">The specified url.</param>
    /// <param name="context"> The context of the SharePoint server.</param>
    public static void SetServer(this Data.WebServices.SharepointCopy.Copy service, Uri url, SpContextBase context = null)
    {
      SetUpService(service, url, "/_vti_bin/copy.asmx", context);
    }

    /// <summary>
    /// Sets the url of the target Web service.
    /// </summary>
    /// <param name="service">
    ///   The service.
    /// </param>
    /// <param name="server">
    ///   The server.
    /// </param>
    /// <param name="name">
    ///   The name.
    /// </param>
    /// <param name="context"> The context of the SharePoint server.</param>
    private static void SetUpService(HttpWebClientProtocol service, Uri server, string name, SpContextBase context)
    {
      service.Url = StringUtil.RemovePostfix('/', server.ToString()) + name;
      var init = ConnectionConfigurationsFactory.CreateInstance(context);
      init.Initialize(service, context);
    }
  }
}