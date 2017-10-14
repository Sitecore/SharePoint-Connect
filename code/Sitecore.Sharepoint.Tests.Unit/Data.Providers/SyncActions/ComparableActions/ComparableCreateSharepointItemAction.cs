// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComparableCreateSharepointItemAction.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ComparableCreateSharepointItemAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.SyncActions.ComparableActions
{
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.SyncActions;

  public class ComparableCreateSharepointItemAction : CreateSharepointItemAction
  {
    public ComparableCreateSharepointItemAction(IntegrationItem item, SynchContext synchContext)
      : base(item, synchContext)
    {
    }

    public bool Equals(ComparableCreateSharepointItemAction other)
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
      return this.Equals(other as ComparableCreateSharepointItemAction);
    }
  }
}