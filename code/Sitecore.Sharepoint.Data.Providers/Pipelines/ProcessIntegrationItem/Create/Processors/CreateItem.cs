// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateItem.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines CreateItem class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.CreateIntegrationItem
{
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Pipelines.ProcessIntegrationItem;

  public class CreateItem
  {
    public virtual void Process([NotNull] ProcessIntegrationItemArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Assert.IsNotNull(args.IntegrationItemID, "args.IntegrationItemID");
      Assert.IsNotNull(args.IntegrationItemTemplateID, "args.IntegrationItemTemplateID");
      Assert.IsNotNull(args.SourceSharepointItem, "args.SourceSharepointItem");
      Assert.IsNotNull(args.SynchContext, "args.SynchContext");

      args.IntegrationItem = IntegrationItemProvider.CreateItem(
                                                               args.IntegrationItemID, 
                                                               args.IntegrationItemTemplateID, 
                                                               args.SourceSharepointItem, 
                                                               args.SynchContext);
    }
  }
}