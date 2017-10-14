// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureEnvironment.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the AzureEnvironment type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.Azure
{
  using System.Linq;
  using Microsoft.WindowsAzure.ServiceRuntime;

  public class AzureEnvironment : AzureEnvironmentBase
  {
    public override bool IsIntegrationInstance
    {
      get
      {
        var instance = RoleEnvironment.CurrentRoleInstance;
        return instance.Id == instance.Role.Instances.Min(x => x.Id);
      }
    }

    public override bool IsAzure
    {
      get
      {
        return RoleEnvironment.IsAvailable;
      }
    }
  }
}
