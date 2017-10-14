// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsLocked.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines IsLocked class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.ProcessIntegrationItem
{
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Texts;

  public class IsLocked
  {
    public virtual void Process([NotNull] ProcessIntegrationItemArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Assert.IsNotNull(args.IntegrationItem, "args.IntegrationItem");

      if (args.IntegrationItem.Locking.IsLocked())
      {
        string message = string.Format(LogMessages.IntegrationItem0IsLockedAndCouldNotBeSynchronizedWithSharePointServer, args.IntegrationItem.Paths.FullPath);
        Log.Warn(message, this);

        args.AbortPipeline();
      }
    }
  }
}