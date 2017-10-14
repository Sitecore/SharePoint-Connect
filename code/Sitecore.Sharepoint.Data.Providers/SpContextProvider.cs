// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpContextProvider.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the ContextProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers
{
  using System;
  using System.Net;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Configuration;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.ObjectModel;
  using Sitecore.Sharepoint.ObjectModel.Utils;

  /// <summary>The SharePoint Context provider.</summary>
  public class SpContextProvider : SpContextProviderBase
  {
    /// <summary>The default credentials.</summary>
    private ICredentials defaultCredentials;

    /// <summary>Gets or sets the default credentials.</summary>
    public virtual ICredentials DefaultCredentials
    {
      get
      {
        return this.defaultCredentials ?? (this.defaultCredentials = CredentialCache.DefaultNetworkCredentials);
      }

      set
      {
        this.defaultCredentials = value;
      }
    }

    /// <summary>Create data context.</summary>
    /// <param name="integrationConfigData">The integration config data.</param>
    /// <returns>The <see cref="SpContext"/>.</returns>
    public override SpContext CreateDataContext(IntegrationConfigData integrationConfigData)
    {
      return this.CreateDataContext(
        integrationConfigData.Server,
        integrationConfigData.Web,
        integrationConfigData.Credentials,
        integrationConfigData.ConnectionConfiguration);
    }

    /// <summary>Creates data context.</summary>
    /// <param name="sharepointServer">The SharePoint server.</param>
    /// <param name="credentials">The credentials.</param>
    /// <param name="connectionConfiguration">The connection configuration.</param>
    /// <returns>The <see cref="SpContext"/>.</returns>
    [Obsolete("Use CreateDataContext(sharepointServer, web, credentials, connectionConfiguration) instead")]
    public override SpContext CreateDataContext(string sharepointServer, ICredentials credentials = null, string connectionConfiguration = null)
    {
      return this.CreateContext(sharepointServer, string.Empty, "Provider", credentials, connectionConfiguration);
    }

    /// <summary>Create data context.</summary>
    /// <param name="sharepointServer">The SharePoint server.</param>
    /// <param name="web">The web.</param>
    /// <param name="credentials">The credentials.</param>
    /// <param name="connectionConfiguration">The connection configuration.</param>
    /// <returns>The <see cref="SpContext"/>.</returns>
    public override SpContext CreateDataContext(string sharepointServer, string web, ICredentials credentials = null, string connectionConfiguration = null)
    {
      return this.CreateContext(sharepointServer, web, "Provider", credentials, connectionConfiguration);
    }

    /// <summary>Creates UI context.</summary>
    /// <param name="sharepointServer">The SharePoint server.</param>
    /// <param name="credentials">The credentials.</param>
    /// <param name="connectionConfiguration">The connection configuration.</param>
    /// <returns>The <see cref="SpContext"/>.</returns>
    [Obsolete("Use CreateUIContext(sharepointServer, web, credentials, connectionConfiguration) instead")]
    public override SpContext CreateUIContext(string sharepointServer, ICredentials credentials = null, string connectionConfiguration = null)
    {
      return this.CreateContext(sharepointServer, string.Empty, "Webcontrol", credentials, connectionConfiguration);
    }

    /// <summary>Create UI context.</summary>
    /// <param name="sharepointServer">The SharePoint server.</param>
    /// <param name="web">The web.</param>
    /// <param name="credentials">The credentials.</param>
    /// <param name="connectionConfiguration">The connection configuration.</param>
    /// <returns>The <see cref="SpContext"/>.</returns>
    public override SpContext CreateUIContext(string sharepointServer, string web, ICredentials credentials = null, string connectionConfiguration = null)
    {
      return this.CreateContext(sharepointServer, web, "Webcontrol", credentials, connectionConfiguration);
    }

    /// <summary>Creates context.</summary>
    /// <param name="sharepointServer">The SharePoint server.</param>
    /// <param name="web">The web.</param>
    /// <param name="predefinedContext">The predefined context.</param>
    /// <param name="credentials">The credentials.</param>
    /// <param name="connectionConfiguration">The connection configuration.</param>
    /// <returns>The <see cref="SpContext"/>.</returns>
    protected virtual SpContext CreateContext(string sharepointServer, string web, string predefinedContext, ICredentials credentials = null, string connectionConfiguration = null)
    {
      var context = new SpContext
      {
        Url = !string.IsNullOrEmpty(sharepointServer) ? sharepointServer : SharepointUtils.CurrentSharepointServer,
        Credentials = credentials,
        ConnectionConfiguration = connectionConfiguration
      };

      if (!string.IsNullOrEmpty(context.ConnectionConfiguration) && context.Credentials != null)
      {
        return context;
      }

      ServerEntry serverEntry = this.GetServerEntry(StringUtil.EnsurePostfix('/', sharepointServer) + StringUtil.RemovePrefix('/', web), predefinedContext);
      if (serverEntry != null)
      {
        if (string.IsNullOrEmpty(context.ConnectionConfiguration))
        {
          context.ConnectionConfiguration = serverEntry.ConnectionConfiguration;
        }

        if (context.Credentials == null)
        {
          context.Credentials = serverEntry.Credentials;
        }
      }

      if (context.Credentials == null)
      {
        context.Credentials = this.DefaultCredentials;
        SharepointUtils.LogDebugInfo(context, "Using DefaultNetworkCredentials");
      }

      return context;
    }

    /// <summary>Gets server entry from the predefined credentials.</summary>
    /// <param name="sharepointServer">The SharePoint server.</param>
    /// <param name="predefinedContext">The predefined context.</param>
    /// <returns>The <see cref="ServerEntry"/>.</returns>
    private ServerEntry GetServerEntry(string sharepointServer, string predefinedContext)
    {
      string serverUrl;
      try
      {
        serverUrl = new Uri(sharepointServer).ToString();
      }
      catch (Exception e)
      {
        Log.Error(string.Format("Sharepoint Debug:Can't parse URL '{0}'.", sharepointServer), e, this);
        return null;
      }

      return Settings.PredefinedServerEntries.GetFirstEntry(serverUrl, predefinedContext);
    }
  }
}