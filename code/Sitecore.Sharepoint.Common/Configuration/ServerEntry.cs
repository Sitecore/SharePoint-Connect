// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerEntry.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ServerEntry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Configuration
{
  using System;
  using System.Net;
  using Sitecore.Diagnostics;

  public class ServerEntry
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerEntry"/> class.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <param name="context">The context.</param>
    /// <param name="isSharepointOnline">[Obsolete] is server sharepoint online.</param>
    /// <param name="connectionConfiguration">The connection configuration name that should be used for the server.</param>
    public ServerEntry([NotNull] string url, [CanBeNull] string userName, [CanBeNull] string password, [NotNull] string context, bool isSharepointOnline = false, string connectionConfiguration = null)
    {
      Assert.ArgumentNotNull(url, "url");
      Assert.ArgumentNotNull(context, "context");

      this.Url = url;
      this.Context = context;
      if (userName != null && password != null)
      {
        this.SetCredentials(userName, password);
      }

      this.IsSharepointOnline = isSharepointOnline;
      this.ConnectionConfiguration = connectionConfiguration;
    }

    public string Url { get; protected set; }

    public ICredentials Credentials { get; protected set; }

    public string Context { get; protected set; }

    [Obsolete("IsSharepointOnline property is obsolete.")]
    public bool IsSharepointOnline { get; protected set; }

    public string ConnectionConfiguration { get; protected set; }

    private void SetCredentials([NotNull] string userName, [NotNull] string password)
    {
      Assert.ArgumentNotNull(userName, "userName");
      Assert.ArgumentNotNull(password, "password");

      string domain = StringUtil.GetPrefix(userName, '\\');
      userName = StringUtil.GetPostfix(userName, '\\', userName);

      this.Credentials = new NetworkCredential(userName, password, domain);
    }
  }
}
