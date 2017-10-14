namespace Sitecore.Sharepoint.Data.Providers
{
  using System;
  using System.Net;
  using Sitecore.Collections;
  using Sitecore.Common;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.ObjectModel;
  using Sitecore.Sharepoint.ObjectModel.ListItems;

  static class PopulateSharepointRoot
  {
    private const int ErrorTimeout = 30;
    private const int DefaultTimeout = 20;

    private static SharepointCache Cache = new SharepointCache();

    public static TemplateID WssFolderTemplateID = new TemplateID(new ID("{F28EDE93-DB84-472E-B45B-595968A9C97C}"));

    public delegate void PopulateSharepointItemDelegate(SharePointSystemData info, ListItem listItem, Item child, Item rootItem);

    public static void PopulateSharepointRootItem(Item item)
    {
      PopulateSharepointRootItem(item, null, false);
    }

    public static void PopulateSharepointRootItem(Item item, PopulateSharepointItemDelegate populateSharepointItemDelegate)
    {
      PopulateSharepointRootItem(item, null, false);
    }

    public static void PopulateSharepointRootItem(Item item, bool force)
    {
      PopulateSharepointRootItem(item, null, force);
    }

    public static void PopulateSharepointRootItem(Item item, PopulateSharepointItemDelegate populateSharepointItemDelegate, bool force)
    {
      try
      {
        PopulateSharepointRootItem(item, item[ItemProvider.SharepointSystemDataField], populateSharepointItemDelegate, force);
      }
      catch (Exception ex)
      {
        Log.Error("Can't populate external data", ex, typeof(PopulateSharepointRoot));
        Cache.AddRootItem(item, null, ErrorTimeout);
      }
    }

    private static void PopulateSharepointRootItem(Item item, string wssServiceData, PopulateSharepointItemDelegate populateSharepointItemDelegate, bool force)
    {
      using (new PopulateSharepointRootDisabler())
      {
        ExpirableItemInformation expirationInfo = Cache.GetItem(item.ID);
        if (expirationInfo != null && !expirationInfo.IsExpired && !force)
        {
          //Sharepoint root item is not expired yet
          return;
        }
        SharePointSystemData systemData = SharePointSystemData.GetData(wssServiceData);
        if (!systemData.IsFolder)
        {
          return;
        }
        ICredentials credentials = GetCredentials(systemData);
        SpContext context = new SpDataContext(systemData.Server, credentials);
        List list = List.GetList(systemData.Web, systemData.List, context);

        //ItemsList listTest = list.GetItemsChanges(DateTime.Now);

        CreateItems(item, systemData, GetSubItems(list, systemData), item, populateSharepointItemDelegate, force);
        Cache.AddRootItem(item, systemData, DefaultTimeout);

      }
    }

    private static void CreateItems(Item parent, SharePointSystemData info, ItemsList itemsList, Item rootItem, PopulateSharepointItemDelegate populateSharepointItemDelegate, bool force)
    {
      string parentID = parent.ID.ToString();

      IDList existenChildrenList = parent.Database.DataManager.DataSource.GetChildIDs(parent.ID);

      foreach (ListItem listItem in itemsList)
      {
        ID id = Utils.GetID(parentID, listItem.UniqueID);

        Item child = rootItem.Database.GetItem(id, parent.Language, Sitecore.Data.Version.Latest);

        if (Switcher<bool, SharepointUpdateItemDisabler>.CurrentValue == false)
        {


          if (child == null)
          {
            TemplateID templateID = new TemplateID(new ID(info.TemplateID));
            if (listItem is FolderItem)
            {
              templateID = WssFolderTemplateID;
            }
            child = ItemManager.AddFromTemplate(listItem.Name, templateID, parent, id);
          }
          UpdateItem(info, listItem, child, rootItem, force);
          if (populateSharepointItemDelegate != null)
          {
            populateSharepointItemDelegate(info, listItem, child, rootItem);
          }
        }

        if (child != null)
        {
          for (int i = 0; i < existenChildrenList.Count; i++)
          {
            if (existenChildrenList[i] == child.ID)
            {
              existenChildrenList.Remove(existenChildrenList[i]);
              break;
            }
          }

          Cache.AddSharepointItem(child, listItem, parent, DefaultTimeout, rootItem);

          if (listItem is FolderItem)
          {

            SharePointSystemData childInfo = info.Clone();
            childInfo.ItemPath = StringUtil.RemovePrefix('/', StringUtil.EnsurePostfix('/', info.ItemPath) + listItem.Name + "/");
            childInfo.IsFolder = true;

            ICredentials credentials = GetCredentials(childInfo);
            SpContext context = new SpDataContext(childInfo.Server, credentials);
            List list = List.GetList(childInfo.Web, childInfo.List, context);

            CreateItems(child, childInfo, GetSubItems(list, childInfo), rootItem, populateSharepointItemDelegate, force);
          }
        }
      }

      foreach (ID id in existenChildrenList)
      {
        Item item = rootItem.Database.GetItem(id, parent.Language, Sitecore.Data.Version.Latest);
        if (item != null)
        {
          item.Delete();
        }
      }
    }

    private static void UpdateItem(SharePointSystemData info, ListItem listItem, Item child, Item rootItem, bool force)
    {
      SharepointItemInformation sharepointItemInformation = Cache.GetItem(child.ID) as SharepointItemInformation;
      if (force || sharepointItemInformation == null || sharepointItemInformation.Modified != Convert.ToDateTime(listItem.Properties["ows_Modified"]))
      {
        using (new EditContext(child))
        {
          DocumentItem documentItem = listItem as DocumentItem;

          Sitecore.Data.Fields.Field sharepointStagingFileField = rootItem.Fields["SharepointStagingFile"];

          if (sharepointStagingFileField == null || sharepointStagingFileField.Value != "1")
          {
            ReSetBlobField(child, documentItem);
          }

          foreach (SharePointSystemData.FieldMapping mapping in info.FieldMappings)
          {
            if (child.Fields[mapping.Target] != null && listItem.Properties.ContainsKey(mapping.Source))
            {
              child[mapping.Target] = listItem.Properties[mapping.Source];
            }
          }
        }
      }
    }

    public static void ReSetBlobField(Item child, DocumentItem documentItem)
    {
      if (documentItem != null && child.Fields["Blob"] != null)
      {
        child.Fields["Blob"].SetBlobStream(new System.IO.MemoryStream(documentItem.GetStream()));
      }
    }

    /// <summary>
    /// Gets the credentials.
    /// </summary>
    /// <returns></returns>
    [CanBeNull]
    internal static ICredentials GetCredentials(SharePointSystemData info)
    {
      string userName = info.User;
      string password = info.Password;
      if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
      {
        return new NetworkCredential(userName, password);
      }
      return null;
    }

    private static ItemsList GetSubItems(List list, SharePointSystemData data)
    {
      ItemsRetrievingOptions itemsOptions = new ItemsRetrievingOptions
      {
        Folder = data.ItemPath,
      };
      return list.GetItems(itemsOptions);
    }
  }
}
