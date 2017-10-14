// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateGuidsPostStepAction.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the UpdateGuidsPostStepAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Installer
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Text;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Install.Framework;
  using Sitecore.SecurityModel;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.Data.Providers.Logging;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;
  using Sitecore.Sharepoint.ObjectModel.Options;
  using SharepointItem = ObjectModel.Entities.Items.BaseItem;

  public class UpdateGuidsPostStepAction : PostStepActions
  {
    private Func<Item, SharepointItem, BaseList, ID> idResolver;

    public override void Run(ITaskOutput output, NameValueCollection metaData)
    {
      base.Run(output, metaData);

      output.Alert(this.UpdateAll() ? "All integration items have been updated successfully." : "Some integration items haven't been updated. See log for details.");
    }

    public bool UpdateAll()
    {
      bool isSuccessful = true;
      using (new IntegrationDisabler())
      {
        string query = string.Format("fast://*[@@templateid = '{0}']", TemplateIDs.IntegrationConfig);
        foreach (Item item in Context.ContentDatabase.SelectItems(query))
        {
          isSuccessful = this.UpdateItemsGuid(item) && isSuccessful;
        }
      }

      return isSuccessful;
    }

    public bool UpdateItemsGuid(Item item)
    {
      IntegrationConfigData integrationData = null;
      try
      {
        integrationData = IntegrationConfigDataProvider.GetFromItem(item);
        
        Assert.IsNotNull(integrationData, "integrationData");

        var context = SpContextProviderBase.Instance.CreateDataContext(integrationData);
        var list = BaseList.GetList(integrationData.Web, integrationData.List, context);

        Assert.IsNotNull(list, "list");

        var itemCollection = list.GetItems(new ItemsRetrievingOptions
        {
          Folder = integrationData.Folder,
          ItemLimit = integrationData.ItemLimit,
          ViewName = integrationData.View
        });

        foreach (var listItem in itemCollection)
        {
          var connectedItem = this.GetConnectedItem(item, listItem, list);

          if (connectedItem != null)
          {
            using (new SecurityDisabler())
            {
              new IntegrationItem(connectedItem).GUID = listItem.GUID;
            }

            if (listItem is FolderItem)
            {
              this.UpdateItemsGuid(connectedItem);
            }
          }
        }

        return true;
      }
      catch (Exception exception)
      {
        var errorMessage = new StringBuilder("Updating guids have been failed.");
        errorMessage.AppendLine(string.Format("Integration config item: {0}", item.ID));
        
        if (integrationData != null)
        {
          errorMessage.AppendLine(LogMessageFormatter.FormatWeb01List23(integrationData));
        }

        Log.Error(errorMessage.ToString(), exception, this);

        return false;
      }
    }

    private static string UniqueIdV10(SharepointItem baseItem, BaseList list)
    {
      var values = new StringCollection
      {
        baseItem.WebUrl.ToString(),
        list.Name,
        baseItem.ID,
        OldFolder(baseItem, list)
      };
      return StringUtil.StringCollectionToString(values);
    }

    private static string OldFolder(SharepointItem baseItem, BaseList list)
    {
      string fileDirRef = StringUtil.RemovePrefix('/', StringUtil.RemovePostfix('/', baseItem["ows_FileDirRef"]));

      int webUrlLength = StringUtil.RemovePrefix('/', StringUtil.RemovePostfix('/', baseItem.WebUrl.PathAndQuery)).Length;
      int listStartIndex = fileDirRef.IndexOf(list.Name, webUrlLength + 1, StringComparison.Ordinal);
      int folderStartIndex = listStartIndex + list.Name.Length + 1;
      if (folderStartIndex >= fileDirRef.Length)
      {
        return string.Empty;
      }

      return fileDirRef.Substring(folderStartIndex);
    }

    private Item GetConnectedItem(Item parentItem, SharepointItem listItem, BaseList list)
    {
      if (this.idResolver != null)
      {
        return parentItem.Database.GetItem(this.idResolver(parentItem, listItem, list));
      }

      List<Func<Item, SharepointItem, BaseList, ID>> canditates = new List<Func<Item, SharepointItem, BaseList, ID>>
      {
        (item, baseItem, baseList) => Utils.GetID(item.ID.ToString(), baseItem.UniqueID),
        (item, baseItem, baseList) => Utils.GetID(item.ID.ToString(), UniqueIdV10(baseItem, baseList))
      };

      foreach (var canditate in canditates)
      {
        var item = parentItem.Database.GetItem(canditate(parentItem, listItem, list));
        {
          if (item != null)
          {
            this.idResolver = canditate;
            return item;
          }
        }
      }

      return null;
    }
  }
}
