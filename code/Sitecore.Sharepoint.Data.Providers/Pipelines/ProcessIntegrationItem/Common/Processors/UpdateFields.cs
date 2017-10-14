// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateFields.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines UpdateFields class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.ProcessIntegrationItem
{
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.Providers;

  public class UpdateFields
  {
    public virtual void Process([NotNull] ProcessIntegrationItemArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Assert.IsNotNull(args.IntegrationItem, "args.IntegrationItem");
      Assert.IsNotNull(args.SourceSharepointItem, "args.SourceSharepointItem");
      Assert.IsNotNull(args.SynchContext, "args.SynchContext");

      IntegrationItemProvider.UpdateFields(args.IntegrationItem, args.SourceSharepointItem, args.SynchContext);
    }
  }
}