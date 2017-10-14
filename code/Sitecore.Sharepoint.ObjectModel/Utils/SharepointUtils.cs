// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharepointUtils.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SharepointUtils type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Sitecore.Sharepoint.ObjectModel.Utils
{
  using System;
  using System.Collections.Generic;
  using System.Net;
  using System.Web;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Configuration;

  /// <summary>
  /// This is util class to make classes usage easier.
  /// </summary>
  public static class SharepointUtils
  {
    private static Dictionary<string, HttpStatusCode> iconExistsCache;

    static SharepointUtils()
    {
      iconExistsCache = new Dictionary<string, HttpStatusCode>();
    }

    /// <summary>
    /// Gets current web path.
    /// </summary>
    public static string CurrentWebPath
    {
      get
      {
        string path = HttpUtility.UrlDecode(Context.Request.GetQueryString("spWebPath"));
          
        if (!string.IsNullOrEmpty(path))
        {
          return path;
        }

        return Context.Item["Web"];
      }
    }

    /// <summary>
    /// Gets current sharepoint server.
    /// </summary>
    public static string CurrentSharepointServer
    {
      get
      {
        string server = Sitecore.Context.Item["Server"];
        if (!string.IsNullOrEmpty(server))
        {
          return server;
        }
        server = HttpUtility.UrlDecode(Sitecore.Context.Request.GetQueryString("spWebServer"));
        if (String.IsNullOrEmpty(server))
        {
          server = Settings.DefaultSharepointServer;
        }
        return server;
      }
    }

    /// <summary>
    /// Write debug information into log.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    public static void LogDebugInfo(SpContext context, string message)
    {
      string userDomain = context.Credentials is NetworkCredential ? (context.Credentials as NetworkCredential).Domain : "";
      string userName = context.Credentials is NetworkCredential ? (context.Credentials as NetworkCredential).UserName : "";
      if (string.IsNullOrEmpty(userDomain) || String.IsNullOrEmpty(userName))//(context.Credentials == CredentialCache.DefaultNetworkCredentials)
      {
        userName = "env " + System.Security.Principal.WindowsIdentity.GetCurrent().Name;        
        userDomain = "";
      }
      Log.Info(string.Format("Sharepoint Debug:{0}\n Context:{1}", message, userDomain + "\\" + userName), context);
    }

    /// <summary>
    /// Write debug information into log.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="messageFormat">
    /// The message format.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    public static void LogDebugInfo(SpContext context, string messageFormat, params object[] args)
    {
      LogDebugInfo(context, String.Format(messageFormat, args));
    }

    /// <summary>
    /// Icons the exists.
    /// </summary>
    /// <param name="sharepointIconUrl">The sharepoint icon URL.</param>
    /// <returns>Whether the icon exists on sharepoint server.</returns>
    public static bool IconExists(string sharepointIconUrl)
    {
      if (!iconExistsCache.ContainsKey(sharepointIconUrl))
      {
        WebRequest iconReq = WebRequest.Create(sharepointIconUrl);
        try
        {
          HttpWebResponse iconRes = (HttpWebResponse)iconReq.GetResponse();
          iconExistsCache.Add(sharepointIconUrl, iconRes.StatusCode);
          iconRes.Close();
        }
        catch (WebException ex)
        {
          iconExistsCache.Add(sharepointIconUrl, ((HttpWebResponse)ex.Response).StatusCode);
        }
      }
      return (iconExistsCache[sharepointIconUrl] == HttpStatusCode.OK) ||
        (iconExistsCache[sharepointIconUrl] == HttpStatusCode.NotModified);
    }
  }
}