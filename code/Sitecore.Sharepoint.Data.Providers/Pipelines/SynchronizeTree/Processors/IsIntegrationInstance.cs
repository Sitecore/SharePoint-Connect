// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsIntegrationInstance.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the IsMainInstance type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.SynchronizeTree
{
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Configuration;
  using Sitecore.Sharepoint.Common.Wrappers;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Azure;
  
  public class IsIntegrationInstance
  {
    private readonly SitecoreEventManager eventManager;
    private readonly AzureEnvironmentBase azureEnvironment;

    public IsIntegrationInstance(SitecoreEventManager eventManager)
    {
      Assert.ArgumentNotNull(eventManager, "eventManager");

      this.eventManager = eventManager;
    }

    public IsIntegrationInstance(SitecoreEventManager eventManager, AzureEnvironmentBase azureEnvironment)
      : this(eventManager)
    {
      this.azureEnvironment = azureEnvironment;
    }

    protected virtual bool IsIntegration
    {
      get
      {
        if (this.azureEnvironment != null && this.azureEnvironment.IsAzure)
        {
          return this.azureEnvironment.IsIntegrationInstance;
        }

        return Settings.IntegrationInstance == Configuration.Settings.InstanceName;
      }
    }

    public void Process(SynchronizeTreeArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      if (!this.IsIntegration)
      {
        args.AbortPipeline();
      }

      if (!args.Options.IsEvent)
      {
        this.eventManager.QueueEvent(new ProcessTreeRemoteEvent(args.Context.ParentID, args.Context.Database.Name));
      }
    }
  }
}
