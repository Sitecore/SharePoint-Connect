// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComparableUpdateIntegrationItemAction.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ComparableUpdateIntegrationItemAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.SyncActions.ComparableActions
{
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Sharepoint.Data.Providers.SyncActions;

  public class ComparableUpdateIntegrationItemAction : UpdateIntegrationItemAction
  {
    public ComparableUpdateIntegrationItemAction(IntegrationItem item, ObjectModel.Entities.Items.BaseItem sharepointItem, SynchContext synchContext, ProcessIntegrationItemsOptions options)
      : base(item, sharepointItem, synchContext, options)
    {
    }

    public bool Equals(UpdateIntegrationItemAction other)
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
      return this.Equals(other as UpdateIntegrationItemAction);
    }

    public override int GetHashCode()
    {
      return this.SharepointItem != null && this.SharepointItem.GUID != null ? this.SharepointItem.GUID.GetHashCode() : 0;
    }
  }
}
