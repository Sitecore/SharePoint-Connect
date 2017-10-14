// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteActions.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ExecuteActions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.SynchronizeTree
{
  using Sitecore.Diagnostics;

  public class ExecuteActions
  {
    public void Process(SynchronizeTreeArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.IsNotNull(args.ActionList, "Value can't be null: args.ActionList");

      args.ActionList.ForEach(x => x.Execute());
    }
  }
}
