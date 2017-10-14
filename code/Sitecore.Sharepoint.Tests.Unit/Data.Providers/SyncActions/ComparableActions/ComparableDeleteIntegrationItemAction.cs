// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComparableDeleteIntegrationItemAction.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ComparableDeleteIntegrationItemAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.SyncActions.ComparableActions
{
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.SyncActions;

  public class ComparableDeleteIntegrationItemAction : DeleteIntegrationItemAction
  {
    public ComparableDeleteIntegrationItemAction(IntegrationItem item, SynchContext synchContext)
      : base(item, synchContext)
    {
    }

    public bool Equals(ComparableDeleteIntegrationItemAction other)
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
      return this.Equals(other as ComparableDeleteIntegrationItemAction);
    }
  }
}
