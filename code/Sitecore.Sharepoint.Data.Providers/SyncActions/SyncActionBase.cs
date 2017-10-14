// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncActionBase.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SyncActionBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.SyncActions
{
  public abstract class SyncActionBase
  {
    public abstract string SharepotinGUID { get; }

    public abstract bool IsSuccessful { get; protected set; }

    public abstract void Execute();
  }
}