// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharepointListsProviderBase.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SharepointListsProviderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.SynchronizeTree.Helpers
{
  using Sitecore.Sharepoint.ObjectModel;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;

  public abstract class SharepointListsProviderBase
  {
    public abstract BaseList GetList(string webName, string list, SpContext context);
  }
}