// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharepointListsProvider.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SharepointListsProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.SynchronizeTree.Helpers
{
  using Sitecore.Sharepoint.ObjectModel;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;

  public class SharepointListsProvider : SharepointListsProviderBase
  {
    public override BaseList GetList(string webName, string list, SpContext context)
    {
      return BaseList.GetList(webName, list, context);
    }
  }
}