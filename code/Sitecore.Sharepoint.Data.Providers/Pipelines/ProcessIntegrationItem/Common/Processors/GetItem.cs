// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetItem.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines GetItem class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable CheckNamespace
namespace Sitecore.Sharepoint.Pipelines.ProcessIntegrationItem
// ReSharper restore CheckNamespace
{
  using Sitecore.Diagnostics;
  using Sitecore.StringExtensions;

  public class GetItem
  {
    public virtual void Process([NotNull] ProcessIntegrationItemArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Assert.IsNotNull(args.IntegrationItemID, "args.IntegrationItemID");
      Assert.IsNotNull(args.SynchContext, "args.SynchContext");

      args.IntegrationItem = args.SynchContext.Database.GetItem(args.IntegrationItemID);
      if (args.IntegrationItem == null)
      {
        args.AbortPipeline();
        Log.Warn("Can't get item '{0}' from database '{1}'".FormatWith(args.IntegrationItemID, args.SynchContext.Database), this);
      }
    }
  }
}