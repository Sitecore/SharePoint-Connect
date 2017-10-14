// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComparableCreateIntegrationItemAction.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ComparableCreateIntegrationItemAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.SyncActions.ComparableActions
{
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Sharepoint.Data.Providers.SyncActions;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;

  public class ComparableCreateIntegrationItemAction : CreateIntegrationItemAction
  {
    public ComparableCreateIntegrationItemAction(BaseItem sharepointItem, SynchContext synchContext, ProcessIntegrationItemsOptions options)
      : base(sharepointItem, synchContext, options)
    {
    }

    public bool Equals(CreateIntegrationItemAction other)
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
      return this.Equals(other as CreateIntegrationItemAction);
    }

    public override int GetHashCode()
    {
      return this.SharepointItem != null && this.SharepointItem.GUID != null ? this.SharepointItem.GUID.GetHashCode() : 0;
    }
  }
}