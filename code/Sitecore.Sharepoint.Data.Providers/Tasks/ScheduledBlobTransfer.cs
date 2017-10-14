// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduledBlobTransfer.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ScheduledBlobTransfer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.Tasks
{
  using System;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Tasks;

  /// <summary>
  /// This class used to run synchronization between SharePoint and Sitecore by tasks.
  /// </summary>
  public class ScheduledBlobTransfer
  {
    /// <summary>
    /// Runs synchronization for integration configuration items with checked ScheduledBlobTransfer field.
    /// </summary>
    /// <param name="itemArray">
    /// The item array.
    /// </param>
    /// <param name="commandItem">
    /// The command item.
    /// </param>
    /// <param name="scheduledItem">
    /// The scheduled item.
    /// </param>
    public void Execute(Item[] itemArray, CommandItem commandItem, ScheduleItem scheduledItem)
    {
      try
      {
        Database database = commandItem.Database;
        string query = string.Format("fast://*[@{0} = '1' and @@templateid = '{1}']", FieldNames.ScheduledBlobTransfer, TemplateIDs.IntegrationConfig);
        Item[] items = database.SelectItems(query);

        foreach (var item in items)
        {
          var processIntegrationItemsOptions = new ProcessIntegrationItemsOptions
          {
            Force = true,
            ScheduledBlobTransfer = true,
            Recursive = true,
            AsyncIntegration = true
          };

          SharepointProvider.ProcessTree(item, processIntegrationItemsOptions);
        }
      }
      catch (Exception exception)
      {
        Log.Error("Tasks.ScheduledBlobTransfer.Execute() failed", exception);
      }
    }
  }
}
