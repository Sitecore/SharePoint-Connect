// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSitecoreItemList.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the GetSitecoreItemList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.SynchronizeTree
{
  using System.Linq;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Items;

  public class GetSitecoreItemList
  {
    public void Process(SynchronizeTreeArgs args)
    {
      Diagnostics.Assert.ArgumentNotNull(args, "args");
      Diagnostics.Assert.IsNotNull(args.Context, "Value can't be null: args.Context");

      args.IntegrationItems = args.Context.ParentItem.Children.Where(SharepointProvider.IsIntegrationItem).Select(x => new IntegrationItem(x)).ToList();
    }
  }
}