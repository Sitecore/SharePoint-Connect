// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WSTrustChannelFactoryWrapper.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the WSTrustChannelFactoryWrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Authentication.Workflows.Helpers
{
  using System.ServiceModel;
  using System.ServiceModel.Security;
  using Sitecore.Diagnostics;

  public class WSTrustChannelFactoryWrapper
  {
    [NotNull]
    public virtual ChannelFactory<IWSTrustChannelContract> CreateFactory([NotNull] TrustVersion trustVersion)
    {
      Assert.ArgumentNotNull(trustVersion, "trustVersion");
      
      return new WSTrustChannelFactory
      {
        TrustVersion = trustVersion
      };
    }

    public virtual void CloseFactory([NotNull] ChannelFactory<IWSTrustChannelContract> factory)
    {
      Assert.ArgumentNotNull(factory, "factory");
      
      factory.Close();
    }
  }
}
