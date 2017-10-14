// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsActiveTree.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the IsActiveTree type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.SynchronizeTree
{
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Items;

  public class IsActiveTree
  {
    public void Process(SynchronizeTreeArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.IsNotNull(args.Context, "Value can't be null: args.Context");

      // TODO: Find way to cover disabler with unit tests.
      using (new IntegrationDisabler())
      {
        var parent = args.Context.ParentItem;
        while (parent != null && (parent.TemplateID == TemplateIDs.IntegrationConfig || parent.TemplateID == TemplateIDs.IntegrationFolder))
        {
          if (!new IntegrationItem(parent).IsActive)
          {
            args.AbortPipeline();
            break;
          }

          parent = parent.Parent;
        }
      }
    }
  }
}