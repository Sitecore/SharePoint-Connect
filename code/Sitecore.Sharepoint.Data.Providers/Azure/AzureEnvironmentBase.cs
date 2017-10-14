// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureEnvironmentBase.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the AzureEnvironmentBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.Azure
{
  public abstract class AzureEnvironmentBase
  {
    public abstract bool IsIntegrationInstance { get; }

    public abstract bool IsAzure { get; }
  }
}