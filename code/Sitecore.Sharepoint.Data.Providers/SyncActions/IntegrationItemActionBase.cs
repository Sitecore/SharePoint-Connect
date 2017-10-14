// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationItemActionBase.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the IntegrationItemActionBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.SyncActions
{
  using Sitecore.Sharepoint.Data.Providers.Items;

  public abstract class IntegrationItemActionBase : SyncActionSafeBase
  {
    protected IntegrationItemActionBase(IntegrationItem item, SynchContext synchContext)
      : base(synchContext)
    {
      this.Item = item;
    }

    public IntegrationItem Item { get; protected set; }

    public override string SharepotinGUID
    {
      get
      {
        return this.Item.GUID;
      }
    }
  }
}