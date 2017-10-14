// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateItem.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines UpdateItem class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.UpdateSharepointItem
{
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Pipelines.ProcessSharepointItem;

  public class UpdateItem
  {
    public virtual void Process([NotNull] ProcessSharepointItemArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Assert.IsNotNull(args.SharepointItem, "args.SharepointItem");
      Assert.IsNotNull(args.SourceIntegrationItem, "args.SourceIntegrationItem");
      Assert.IsNotNull(args.SynchContext, "args.SynchContext");

      SharepointItemProvider.UpdateItem(args.SourceIntegrationItem, args.SharepointItem, args.SynchContext);
    }
  }
}