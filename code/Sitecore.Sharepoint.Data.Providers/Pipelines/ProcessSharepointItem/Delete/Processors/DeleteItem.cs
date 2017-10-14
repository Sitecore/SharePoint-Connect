// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteItem.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines DeleteItem class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.DeleteSharepointItem
{
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Pipelines.ProcessSharepointItem;

  public class DeleteItem
  {
    public virtual void Process([NotNull] ProcessSharepointItemArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Assert.IsNotNull(args.SharepointItem, "args.SharepointItem");

      args.SharepointItem.Delete();
    }
  }
}