// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetItem.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the GetItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.GetItem
{
  using Sitecore.Diagnostics;
  using Sitecore.Pipelines.ItemProvider.GetItem;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Options;

  /// <summary>The get item.</summary>
  public class GetItem
  {
    /// <summary>The process.</summary>
    /// <param name="args">The args.</param>
    public virtual void Process([NotNull] GetItemArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      if (!args.Handled)
      {
        args.Result = args.ItemPath != null ? args.FallbackProvider.GetItem(args.ItemPath, args.Language, args.Version, args.Database, args.SecurityCheck) : args.FallbackProvider.GetItem(args.ItemId, args.Language, args.Version, args.Database, args.SecurityCheck);
        args.Handled = true;
      }

      if (IntegrationDisabler.CurrentValue || args.Result == null)
      {
        return;
      }

      if (SharepointProvider.IsActiveIntegrationFolder(args.Result) || SharepointProvider.IsActiveIntegrationDataItem(args.Result))
      {
        SharepointProvider.ProcessItem(args.Result, ProcessIntegrationItemsOptions.DefaultAsyncOptions);
      }
    }
  }
}
