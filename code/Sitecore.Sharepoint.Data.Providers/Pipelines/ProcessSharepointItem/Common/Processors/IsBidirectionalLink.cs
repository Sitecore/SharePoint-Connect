// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsBidirectionalLink.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the IsBidirectionalLink class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.ProcessSharepointItem
{
  using Sitecore.Diagnostics;

  public class IsBidirectionalLink
  {
    public virtual void Process([NotNull] ProcessSharepointItemArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Assert.IsNotNull(args.SynchContext, "args.SynchContext");

      if (!args.SynchContext.IntegrationConfigData.BidirectionalLink)
      {
        args.AbortPipeline();
      }
    }
  }
}