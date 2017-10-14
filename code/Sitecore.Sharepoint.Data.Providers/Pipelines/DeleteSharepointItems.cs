// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteSharepointItems.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Processor for pipeline uiDeleteItems
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.Pipelines
{
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.IDTables;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Tasks;
  using Sitecore.Text;
  using Sitecore.Web.UI.Sheer;

  /// <summary>
  /// Processor for pipeline uiDeleteItems
  /// </summary>
  public class DeleteSharepointItems
  {
    /// <summary>
    /// This is processor of pipeline uiDeleteItems.
    /// This processor prepeare data for deleting items from SharePoint.
    /// </summary>
    /// <param name="args">
    /// The args            .
    /// </param>

    const string CancelationKey = "SharepointUpdateCancelation";
    public void Prepare(ClientPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(args, "args");
      Database database = GetDatabase(args);
      ListString list = new ListString(args.Parameters["items"], '|');
      foreach (string id in list)
      {
        Item item = database.GetItem(id);
        if (SharepointProvider.IsActiveIntegrationConfigItem(item))
        {
          args.Parameters[CancelationKey] = "1";
        }

        if (SharepointProvider.IsActiveIntegrationDataItem(item))
        {
          if (item != null)
          {
            CacheableIntegrationItemInfo cacheableIntegrationItemInfo = IntegrationCache.GetIntegrationItemInfo(item.ID);
            if (cacheableIntegrationItemInfo != null)
            {
              args.Parameters[id] = cacheableIntegrationItemInfo.ParentItemId.ToString();
              args.Parameters[id + "uniqueId"] = cacheableIntegrationItemInfo.SharepointItemId;
            }
          }
        }
      }
    }

    /// <summary>
    /// This is processor of pipeline uiDeleteItems.
    /// This processor is deleting data in SharePoint.
    /// </summary>
    /// <param name="args">
    /// The args         .
    /// </param>
    public void Execute(ClientPipelineArgs args)
    {
      if (!string.IsNullOrEmpty(args.Parameters[CancelationKey]))
      {
        return;
      }

      using (new SynchDisabler())
      {
        Assert.ArgumentNotNull(args, "args");
        Assert.ArgumentNotNull(args, "args");
        Database database = GetDatabase(args);

        ListString list = new ListString(args.Parameters["items"], '|');
        using (new TaskContext("DeleteSharepointItems pipeline"))
        {
          foreach (string id in list)
          {
            if (args.Parameters[id] != null && ID.IsID(id))
            {
              ItemDefinition itemDfenition = database.DataManager.DataSource.GetItemDefinition(new ID(id));
              if (itemDfenition == null)
              {
                Log.Audit(this, "Delete SharePoint item: {0}", new[]
                {
                  args.Parameters[id + "uniqueId"]
                });
                if (ID.IsID(args.Parameters[id]))
                {
                  IDTable.Add("SPIF_DELETE", args.Parameters[id], ID.NewID, ID.Null, args.Parameters[id + "uniqueId"]);
                }
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Get databaser class from args.Parameters["database"]. 
    /// </summary>
    /// <param name="args">
    /// The args             .
    /// </param>
    /// <returns>
    /// Database from args.Parameters["database"]
    /// </returns>
    private static Database GetDatabase(ClientPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Database database = Factory.GetDatabase(args.Parameters["database"]);
      Assert.IsNotNull(database, typeof(Database), "Name: {0}", new object[] { args.Parameters["database"] });
      return Assert.ResultNotNull(database);
    }
  }
}
