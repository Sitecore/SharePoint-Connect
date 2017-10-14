// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncActionSafeBase.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SyncActionSafeBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.SyncActions
{
  using System;
  using Sitecore.Diagnostics;

  public abstract class SyncActionSafeBase : SyncActionBase
  {
    protected readonly SynchContext SynchContext;

    protected SyncActionSafeBase(SynchContext synchContext)
    {
      this.SynchContext = synchContext;
    }

    public override void Execute()
    {
      try
      {
        this.IsSuccessful = this.ExecuteAction();
      }
      catch (Exception ex)
      {
        Log.Error("Error processing action", ex, this);
      }
    }

    public abstract bool ExecuteAction();

    public override bool IsSuccessful { get; protected set; }
  }
}