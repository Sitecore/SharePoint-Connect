// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistoryProviderBase.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the HistoryProviderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.SynchronizeTree.Helpers
{
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;

  public abstract class HistoryProviderBase
  {
    public abstract bool IsItemChanged(SynchronizeTreeArgs args, string sharepointGUID);

    public abstract bool IsItemDeleted(SynchronizeTreeArgs args, string sharepointGUID);
  }
}