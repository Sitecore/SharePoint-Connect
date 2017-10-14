// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetChildren.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the GetChildren type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.GetChildren
{
  using Sitecore.Diagnostics;
  using Sitecore.Pipelines.ItemProvider.GetChildren;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Options;

  /// <summary>The get children.</summary>
  public class GetChildren
  {
    /// <summary>The process.</summary>
    /// <param name="args">The args.</param>
    public virtual void Process([NotNull] GetChildrenArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      if (!IntegrationDisabler.CurrentValue)
      {
        if (SharepointProvider.IsActiveIntegrationConfigItem(args.Item) || SharepointProvider.IsActiveIntegrationFolder(args.Item))
        {
          SharepointProvider.ProcessTree(args.Item, ProcessIntegrationItemsOptions.DefaultAsyncOptions);
        }
      }

      if (!args.Handled)
      {
        using (new IntegrationDisabler())
        {
          args.Result = args.FallbackProvider.GetChildren(args.Item, args.SecurityCheck);
          args.Handled = true;
        }
      }
    }
  }
}
