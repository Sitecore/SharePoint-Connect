// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheableIntegrationConfigData.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the CacheableIntegrationConfigData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Sitecore.Sharepoint.Data.Providers.Cache
{
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;

  /// <summary>
  /// Defines the properties and functionality of an object that is cacheable for IntegrationConfig and IntegrationFolder items.
  /// </summary>
  public class CacheableIntegrationConfigData : CacheableData
  {
    /// <summary>
    /// Gets or sets sharepoint information.
    /// </summary>
    public IntegrationConfigData Data { get; set; }

    /// <summary>
    /// Gets the size of the data to use when caching.
    /// </summary>
    /// <returns>
    /// Size of the data
    /// </returns>
    public override long GetDataLength()
    {
      return base.GetDataLength() + this.Data.GetDataLength();
    }
  }
}
