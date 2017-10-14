// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateExpirationInterval.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the UpdateExpirationInterval type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.SynchronizeTree
{
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Cache;

  public class UpdateExpirationInterval
  {
    public void Process(SynchronizeTreeArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      IntegrationCache.AddIntegrationConfigData(args.Context.ParentID, args.Context.IntegrationConfigData, (args.Context.IntegrationConfigData.ExpirationInterval > 0) ? args.Context.IntegrationConfigData.ExpirationInterval : SharepointProvider.DefaultTimeout);
    }
  }
}
