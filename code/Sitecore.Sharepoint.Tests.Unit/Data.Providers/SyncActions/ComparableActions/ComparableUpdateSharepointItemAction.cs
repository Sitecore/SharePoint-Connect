// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComparableUpdateSharepointItemAction.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ComparableUpdateSharepointItemAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.SyncActions.ComparableActions
{
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.SyncActions;

  public class ComparableUpdateSharepointItemAction : UpdateSharepointItemAction
  {
    public ComparableUpdateSharepointItemAction(ObjectModel.Entities.Items.BaseItem sharepointItem, IntegrationItem item, SynchContext synchContext)
      : base(sharepointItem, item, synchContext)
    {
    }

    public bool Equals(ComparableUpdateSharepointItemAction other)
    {
      if (other == null)
      {
        return false;
      }

      if (other.Item == null)
      {
        return this.Item == null;
      }

      return other.Item.ID == this.Item.ID;
    }

    public override bool Equals(object other)
    {
      var comparableUpdateSharepointItemAction = other as ComparableUpdateSharepointItemAction;

      return comparableUpdateSharepointItemAction != null && this.Equals(comparableUpdateSharepointItemAction);
    }
  }
}