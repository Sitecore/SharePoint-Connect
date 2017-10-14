// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SitecoreExtensions.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the SitecoreExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit
{
  using System.Linq;

  using Sitecore.Data;
  using Sitecore.Jobs;

  /// <summary>The item extension.</summary>
  public static class SitecoreExtensions
  {
    /// <summary>The get sync job name.</summary>
    /// <param name="id">The item id.</param>
    /// <returns>The <see cref="string"/>.</returns>
    public static bool WasSyncStarted(this ID id)
    {
      var jobName = string.Format("SharePoint_Integration_{0}", id);
      return JobManager.GetJobs().Any(x => x.Name == jobName);
    }
  }
}