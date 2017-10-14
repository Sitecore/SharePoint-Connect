// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpContextProviderBase.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the SpContextProviderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace Sitecore.Sharepoint.Data.Providers
{
  using System;
  using System.Net;

  using Microsoft.Extensions.DependencyInjection;

  using Sitecore.Abstractions;
  using Sitecore.DependencyInjection;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.ObjectModel;

  /// <summary>The provider of SharePoint contexts.</summary>
  public abstract class SpContextProviderBase
  {
    private static BaseFactory factory;

    internal static BaseFactory Factory
    {
      get
      {
        return factory ?? (factory = ServiceLocator.ServiceProvider.GetRequiredService<BaseFactory>());
      }

      set
      {
        factory = value;
      }
    }

    /// <summary>Gets the instance.</summary>
    public static SpContextProviderBase Instance
    {
      get
      {
        return (SpContextProviderBase)Factory.CreateObject("SharepointContextProvider", true);
      }
    }

    /// <summary>Creates data context.</summary>
    /// <param name="integrationConfigData">The integration config data.</param>
    /// <returns>The <see cref="SpContext"/>.</returns>
    public abstract SpContext CreateDataContext(IntegrationConfigData integrationConfigData);

    /// <summary>Creates data context.</summary>
    /// <param name="sharepointServer">The SharePoint server.</param>
    /// <param name="credentials">The credentials.</param>
    /// <param name="connectionConfiguration">The connection configuration.</param>
    /// <returns>The <see cref="SpContext"/>.</returns>
    [Obsolete("Use CreateDataContext(sharepointServer, web, credentials, connectionConfiguration) instead")]
    public abstract SpContext CreateDataContext(string sharepointServer, ICredentials credentials = null, string connectionConfiguration = null);

    /// <summary>Creates data context.</summary>
    /// <param name="sharepointServer">The SharePoint server.</param>
    /// <param name="web">The web.</param>
    /// <param name="credentials">The credentials.</param>
    /// <param name="connectionConfiguration">The connection configuration.</param>
    /// <returns>The <see cref="SpContext"/>.</returns>
    public virtual SpContext CreateDataContext(string sharepointServer, string web, ICredentials credentials = null, string connectionConfiguration = null)
    {
      throw new NotImplementedException();
    }

    /// <summary>Create UI context.</summary>
    /// <param name="sharepointServer">The SharePoint server.</param>
    /// <param name="credentials">The credentials.</param>
    /// <param name="connectionConfiguration">The connection configuration.</param>
    /// <returns>The <see cref="SpContext"/>.</returns>
    [Obsolete("Use CreateUIContext(sharepointServer, web, credentials, connectionConfiguration) instead")]
    public abstract SpContext CreateUIContext(string sharepointServer, ICredentials credentials = null, string connectionConfiguration = null);

    /// <summary>Creates UI context.</summary>
    /// <param name="sharepointServer">The SharePoint server.</param>
    /// <param name="web">The web.</param>
    /// <param name="credentials">The credentials.</param>
    /// <param name="connectionConfiguration">The connection configuration.</param>
    /// <returns>The <see cref="SpContext"/>.</returns>
    public virtual SpContext CreateUIContext(string sharepointServer, string web, ICredentials credentials = null, string connectionConfiguration = null)
    {
      throw new NotImplementedException();
    }
  }
}