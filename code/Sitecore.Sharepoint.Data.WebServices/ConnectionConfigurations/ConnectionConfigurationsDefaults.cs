// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionConfigurationsDefaults.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ConnectionConfigurationsDefaults type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.WebServices.ConnectionConfigurations
{
  using Sitecore.Sharepoint.Common.Caches;

  internal class ConnectionConfigurationsDefaults
  {
    private static ConnectionConfigurationsDefaults instance;

    public static ConnectionConfigurationsDefaults Instance
    {
      get
      {
        return instance ?? (instance = new ConnectionConfigurationsDefaults());
      }

      set
      {
        instance = value;
      }
    }

    public virtual ICookieContainerCache GetContainerCache()
    {
      return new CookieContainerCache();
    }
  }
}