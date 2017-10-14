// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionConfigurationsFactory.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ConnectionConfigurationsFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.WebServices
{
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Common.Wrappers;
  using Sitecore.Sharepoint.Data.WebServices.ConnectionConfigurations;

  internal class ConnectionConfigurationsFactory
  {
    private readonly SitecoreFactory sitecoreFactory;
    
    public ConnectionConfigurationsFactory([NotNull] SitecoreFactory sitecoreFactory)
    {
      Assert.ArgumentNotNull(sitecoreFactory, "sitecoreFactory");
      
      this.sitecoreFactory = sitecoreFactory;
    }

    [NotNull]
    public BaseConnectionConfiguration CreateInstance([NotNull] SpContextBase context)
    {
      Assert.ArgumentNotNull(context, "context");

      string configuration = context.ConnectionConfiguration;
      if (string.IsNullOrEmpty(configuration))
      {
        configuration = "Default";
      }

      var connectionConfiguration = string.Format("/sitecore/sharepoint/connectionConfigurations/{0}", configuration);
      return (BaseConnectionConfiguration)this.sitecoreFactory.CreateObject(connectionConfiguration, true);
    }
  }
}