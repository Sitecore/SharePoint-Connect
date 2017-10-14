// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsCheckedOut.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines IsCheckedOut class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.ProcessSharepointItem
{
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Texts;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;

  public class IsCheckedOut
  {
    public virtual void Process([NotNull] ProcessSharepointItemArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Assert.IsNotNull(args.SharepointItem, "args.SharepointItem");

      var documentItem = args.SharepointItem as DocumentItem;
      if (documentItem != null && documentItem.IsCheckedOut)
      {
        string message = string.Format(LogMessages.SharepointItem0IsCheckedOutAndCouldNotBeSynchronizedWithSitecore, args.SharepointItem.UniqueID);
        Log.Warn(message, this);

        args.AbortPipeline();
      }
    }
  }
}