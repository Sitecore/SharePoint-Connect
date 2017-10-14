// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistoryProviderIDTable.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the IDTableProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.SynchronizeTree.Helpers
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using Microsoft.Extensions.DependencyInjection;

  using Sitecore.Abstractions;
  using Sitecore.Data;
  using Sitecore.Data.IDTables;
  using Sitecore.Data.Items;
  using Sitecore.DependencyInjection;
  using Sitecore.Diagnostics;
  using Sitecore.Events;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;
  using Sitecore.Text;
  using Sitecore.Web.UI.Sheer;

  public class HistoryProviderIDTable : HistoryProviderBase
  {
    private readonly BaseFactory factory;

    public const string PrefixSave = "SPIF_SAVE";

    public const string PrefixDelete = "SPIF_DELETE";

    private const string IdTableEntries = "IDTableEntries";
    private const string CancelationKey = "SharepointUpdateCancelation";
    private const string CancelationValue = "1";

    public HistoryProviderIDTable():this(ServiceLocator.ServiceProvider.GetRequiredService<BaseFactory>())
    {
    }

    internal HistoryProviderIDTable(BaseFactory factory)
    {
      this.factory = factory;
    }

    public override bool IsItemChanged(SynchronizeTreeArgs args, string sharepointGUID)
    {
      return this.IsEntryPresent(PrefixSave, args, sharepointGUID);
    }

    public override bool IsItemDeleted(SynchronizeTreeArgs args, string sharepointGUID)
    {
      return this.IsEntryPresent(PrefixDelete, args, sharepointGUID);
    }

    public void Init(SynchronizeTreeArgs args)
    {
      args.CustomData[IdTableEntries] = new Dictionary<string, List<IDTableEntry>>
      {
        { PrefixSave, this.GetEntries(PrefixSave, args.Context.ParentID) },
        { PrefixDelete, this.GetEntries(PrefixDelete, args.Context.ParentID) }
      };
    }

    public void Refresh(SynchronizeTreeArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.IsNotNull(args.ActionList, "Value can't be null: args.ActionList");

      var dictionary = (Dictionary<string, List<IDTableEntry>>)args.CustomData[IdTableEntries];
      var tableEntries = dictionary.SelectMany(x => x.Value).Where(x => x.ParentID == args.Context.ParentID).ToList();
      foreach (var entry in tableEntries)
      {
        var action = args.ActionList.Find(x => x.SharepotinGUID == entry.CustomData);
        if (action == null || action.IsSuccessful)
        {
          IDTable.RemoveKey(entry.Prefix, entry.Key);
        }
      }
    }

    public void PrepareToDelete(ClientPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var itemIDs = new ListString(args.Parameters["items"]);
      var database = this.factory.GetDatabase(args.Parameters["database"]);

      foreach (var itemID in itemIDs)
      {
        var item = database.GetItem(itemID);
        if (SharepointProvider.IsActiveIntegrationConfigItem(item))
        {
          args.Parameters[CancelationKey] = CancelationValue;
          return;
        }

        if (SharepointProvider.IsActiveIntegrationDataItem(item))
        {
          var integrationItem = new IntegrationItem(item);
          if (!integrationItem.IsNew)
          {
            args.CustomData.Add(itemID, integrationItem.GUID);
            args.CustomData.Add(itemID + "parentID", item.ParentID);
          }
        }
      }
    }

    public void ProcessDelete(ClientPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      if (args.Parameters[CancelationKey] == CancelationValue)
      {
        return;
      }

      var itemIDs = new ListString(args.Parameters["items"]);
      foreach (var itemID in itemIDs)
      {
        if (!string.IsNullOrWhiteSpace((string)args.CustomData[itemID]) && args.CustomData[itemID + "parentID"] != null)
        {
          this.AddToTable(PrefixDelete, (string)args.CustomData[itemID], (ID)args.CustomData[itemID + "parentID"]);
        }
      }
    }

    public void OnItemSaved(object sender, EventArgs args)
    {
      var item = Event.ExtractParameter(args, 0) as Item;
      Assert.IsNotNull(item, "No item in parameters");
      if (IntegrationDisabler.CurrentValue)
      {
        return;
      }

      if (SharepointProvider.IsActiveIntegrationDataItem(item))
      {
        var integrationItem = new IntegrationItem(item);
        if (!integrationItem.IsNew)
        {
          using (new IntegrationDisabler())
          {
            this.AddToTable(PrefixSave, integrationItem.GUID, item.ParentID);
          }
        }
      }
    }

    protected virtual void AddToTable(string prefix, string sharepointGUID, ID parentID)
    {
      IDTable.Add(prefix, ID.NewID.ToString(), ID.NewID, parentID, sharepointGUID);
    }

    private bool IsEntryPresent(string prefix, SynchronizeTreeArgs args, string sharepointGUID)
    {
      var dictionary = (Dictionary<string, List<IDTableEntry>>)args.CustomData[IdTableEntries];
      return dictionary[prefix].Any(x => x.CustomData == sharepointGUID && x.ParentID == args.Context.ParentID);
    }

    private List<IDTableEntry> GetEntries(string prefix, ID parentId)
    {
      return IDTable.GetKeys(prefix).Where(x => x.ParentID == parentId).ToList();
    }
  }
}
