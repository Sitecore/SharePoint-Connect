// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSharepointItemList.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the GetSharepointItemList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.SynchronizeTree
{
  using System.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.ObjectModel.Options;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree.Helpers;

  public class GetSharepointItemList
  {
    private readonly SharepointListsProviderBase provider;
    private SpContextProviderBase contextProvider;

    public GetSharepointItemList(SharepointListsProviderBase provider, SpContextProviderBase contextProvider)
    {
      this.provider = provider;
      this.contextProvider = contextProvider;
    }

    public void Process(SynchronizeTreeArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.IsNotNull(args.Context, "Value can't be null: args.Context");

      var context = this.contextProvider.CreateDataContext(args.Context.IntegrationConfigData);
      var list = this.provider.GetList(args.Context.IntegrationConfigData.Web, args.Context.IntegrationConfigData.List, context);

      var itemCollection = list.GetItems(new ItemsRetrievingOptions
      {
        Folder = args.Context.IntegrationConfigData.Folder,
        ItemLimit = args.Context.IntegrationConfigData.ItemLimit,
        ViewName = args.Context.IntegrationConfigData.View
      });

      args.SharepointItemList = itemCollection.ToList();
    }
  }
}