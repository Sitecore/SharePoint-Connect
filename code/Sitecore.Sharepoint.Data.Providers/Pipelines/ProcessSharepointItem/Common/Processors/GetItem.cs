// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetItem.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines GetItem class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.ProcessSharepointItem
{
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;

  public class GetItem
  {
    public virtual void Process([NotNull] ProcessSharepointItemArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Assert.IsNotNullOrEmpty(args.SharepointItemID, "args.SharepointItemID");
      Assert.IsNotNull(args.SynchContext, "args.SynchContext");

      args.SharepointItem = args.SharepointItem ?? BaseItem.GetItem(args.SharepointItemID, args.SynchContext.Context, args.SynchContext.IntegrationConfigData.View);
    }
  }
}