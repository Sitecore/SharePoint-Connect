// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteItem.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines DeleteItem class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.DeleteIntegrationItem
{
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Pipelines.ProcessIntegrationItem;

  public class DeleteItem
  {
    public virtual void Process([NotNull] ProcessIntegrationItemArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Assert.IsNotNull(args.IntegrationItem, "args.IntegrationItem");

      args.IntegrationItem.Delete();
    }
  }
}