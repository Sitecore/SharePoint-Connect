// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpContext.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Main entry point to connect to Sharepoint services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel
{
  using System;
  using System.Net;
  using Diagnostics;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Common.Configuration;
  using Sitecore.Sharepoint.ObjectModel.Utils;

  /// <summary>
  /// Main entry point to connect to Sharepoint services.
  /// </summary>
  public class SpContext : SpContextBase
  {
    public SpContext()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpContext"/> class.
    /// </summary>
    /// <param name="userName">Name of the user to access to Sharepoint server.</param>
    /// <param name="password">The user password.</param>
    [Obsolete("Not default constructors are deprecated.Please use SpContextProvider.Instance.CreateContext methods or default constructor and set appropriate properties.")]
    protected SpContext([NotNull] string userName, [NotNull] string password)
    {
      throw new InvalidOperationException("Please, use context provider to create context");
      Assert.ArgumentNotNull(userName, "userName");
      Assert.ArgumentNotNull(password, "password");

      this.Initialize(null, new NetworkCredential(userName, password));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpContext"/> class.
    /// Will connect to current sharepoint server. See  SharepointUtils.CurrentSharepointServer for info
    /// </summary>
    /// <param name="credentials">The credentials to be used to connect to Sharepoint server.</param>
    [Obsolete("Not default constructors are deprecated.Please use SpContextProvider.Instance.CreateContext methods or default constructor and set appropriate properties.")]
    protected SpContext([NotNull] ICredentials credentials)
    {
      throw new InvalidOperationException("Please, use context provider to create context");
      Assert.ArgumentNotNull(credentials, "credentials");

      this.Initialize(null, credentials);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpContext"/> class.
    /// Will connect to server via passed parameter using either user credentials from web.config or DefaultNetworkCredentials
    /// </summary>
    /// <param name="serverUrl">The server URL.</param>
    [Obsolete("Not default constructors are deprecated.Please use SpContextProvider.Instance.CreateContext methods or default constructor and set appropriate properties.")]
    protected SpContext([NotNull] string serverUrl)
    {
      throw new InvalidOperationException("Please, use context provider to create context");
      Assert.ArgumentNotNull(serverUrl, "serverUrl");

      this.Initialize(serverUrl, null);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpContext"/> class.
    /// </summary>
    /// <param name="url">The URL to the Sharepoint server.</param>
    /// <param name="credentials">The credentials to be used to connect.</param>
    [Obsolete("Not default constructors are deprecated.Please use SpContextProvider.Instance.CreateContext methods or default constructor and set appropriate properties.")]
    protected SpContext([NotNull] string url, ICredentials credentials)
    {
      throw new InvalidOperationException("Please, use context provider to create context");
      Assert.ArgumentNotNull(url, "url");

      this.Initialize(url, credentials);
    }

    /// <summary>
    /// Gets the URL to the Sharepoint server.
    /// </summary>
    public override string Url { get; set; }
  
    /// <summary>
    /// Gets Credentials to be used to connect to Sharepoint server
    /// </summary>
    public override ICredentials Credentials { get; set; }

    /// <summary>
    /// Gets the hash to be used in HashTable
    /// </summary>
    public override int Hash
    {
      get
      {
        return string.Format("{0}_{1}", this.Url, this.Credentials.GetHashCode()).GetHashCode();
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Claims-Based authentication should be used.
    /// </summary>
    [Obsolete("IsSharepointOnline property is obsolete.")]
    public override bool IsSharepointOnline { get; set; }

    public override string ConnectionConfiguration { get; set; }

    /// <summary>
    /// Initializes the SpConnector
    /// </summary>
    /// <param name="serverUrl">The URL to the server. If null or empty current Sharepoint server will be used. See SharepointUtils.CurrentSharepointServer for info</param>
    /// <param name="serverCredentials">The credentials to be used to connect. Is null or empty either credentials from web.config or DefaultNetworkCredentials will be used</param>
    [AllowNull("*")]
    [Obsolete("The method is obsolete and will be removed in future versions.")]
    protected void Initialize(string serverUrl, ICredentials serverCredentials)
    {
      if (string.IsNullOrEmpty(serverUrl))
      {
        serverUrl = SharepointUtils.CurrentSharepointServer;
      }

      this.Url = serverUrl;
      if (serverCredentials == null)
      {
        serverCredentials = this.GetPredefinedCredentials();
        if (serverCredentials == null)
        {
          serverCredentials = CredentialCache.DefaultNetworkCredentials;
          this.Credentials = serverCredentials;
          SharepointUtils.LogDebugInfo(this, "Using DefaultNetworkCredentials");
        }
      }

      this.Credentials = serverCredentials;
      this.InitConfiguration();
    }

    [Obsolete("The method is obsolete and will be removed in future versions.")]
    protected virtual void InitConfiguration()
    {
      string serverUrl;
      try
      {
        serverUrl = new Uri(this.Url).GetLeftPart(UriPartial.Authority);
      }
      catch (Exception e)
      {
        Log.Error(string.Format("Sharepoint Debug:Can't parse URL '{0}'.", this.Url), this);
        return;
      }

      ServerEntry predefinedServerEntry = this.GetPredefinedServerEntry(serverUrl);

      if (predefinedServerEntry != null)
      {
        this.ConnectionConfiguration = predefinedServerEntry.ConnectionConfiguration;
      }
    }

    /// <summary>
    /// Get predefined credentials
    /// </summary>
    /// <returns>Credentials.</returns>
    [CanBeNull]
    [Obsolete("The method is obsolete and will be removed in future versions.")]
    protected virtual ICredentials GetPredefinedCredentials()
    {
      string serverUrl;
      try
      {
        serverUrl = new Uri(this.Url).GetLeftPart(UriPartial.Authority);
      }
      catch (Exception e)
      {
        Log.Error(string.Format("Sharepoint Debug:Can't parse URL '{0}'.", this.Url), this);
        return null;
      }

      ServerEntry predefinedServerEntry = this.GetPredefinedServerEntry(serverUrl);

      if (predefinedServerEntry != null)
      {
        this.IsSharepointOnline = predefinedServerEntry.IsSharepointOnline;
        return predefinedServerEntry.Credentials;
      }

      return null;
    }

    [CanBeNull]
    [Obsolete("The method is obsolete and will be removed in future versions.")]
    protected virtual ServerEntry GetPredefinedServerEntry([NotNull] string serverUrl)
    {
      throw new NotImplementedException();
    }
  }
}
