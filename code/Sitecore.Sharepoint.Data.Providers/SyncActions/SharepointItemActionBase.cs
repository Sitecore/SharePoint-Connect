// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharepointActionBase.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SharepointActionBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.SyncActions
{
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;

  public abstract class SharepointItemActionBase : SyncActionSafeBase
  {
    protected SharepointItemActionBase(BaseItem sharepointItem, SynchContext synchContext) : base(synchContext)
    {
      this.SharepointItem = sharepointItem;
    }

    public override string SharepotinGUID
    {
      get
      {
        return this.SharepointItem.GUID;
      }
    }

    public BaseItem SharepointItem { get; protected set; }
  }
}