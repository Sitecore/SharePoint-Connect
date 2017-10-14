// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionConfigurationEntry.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ConnectionConfigurationEntry type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Configuration
{
  public class ConnectionConfigurationEntry
  {
    [NotNull]
    public string Name { get; set; }

    [NotNull]
    public string DisplayName { get; set; }
  }
}
