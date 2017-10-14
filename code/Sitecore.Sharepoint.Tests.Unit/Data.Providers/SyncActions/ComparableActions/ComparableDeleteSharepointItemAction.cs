// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComparableDeleteSharepointItemAction.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ComparableDeleteSharepointItemAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.SyncActions.ComparableActions
{
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.SyncActions;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;

  public class ComparableDeleteSharepointItemAction : DeleteSharepointItemAction
  {
    public ComparableDeleteSharepointItemAction(BaseItem sharepointItem, SynchContext synchContext)
      : base(sharepointItem, synchContext)
    {
    }

    public bool Equals(ComparableDeleteSharepointItemAction other)
    {
      if (other == null)
      {
        return false;
      }

      if (other.SharepointItem == null)
      {
        return this.SharepointItem == null;
      }

      return other.SharepointItem.GUID == this.SharepointItem.GUID;
    }

    public override bool Equals(object other)
    {
      return this.Equals(other as ComparableDeleteSharepointItemAction);
    }
  }
}
