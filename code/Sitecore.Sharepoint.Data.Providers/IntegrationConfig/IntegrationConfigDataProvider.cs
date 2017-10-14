// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationConfigDataProvider.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines IntegrationConfigDataProvider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.IntegrationConfig
{
  using System;
  using System.Linq;
  using Sitecore.Data;
  using Sitecore.Data.Events;
  using Sitecore.Data.Fields;
  using Sitecore.Data.IDTables;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Events;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Common.Security.Cryptography;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Data.Providers.Items;

  public class IntegrationConfigDataProvider
  {
    private const string EncryptionKeyIDTableEntryPrefix = "IntegrationConfigData";

    public static void SaveToItem([NotNull] IntegrationConfigData data, [NotNull] Item destinationIntegrationItem)
    {
      Assert.ArgumentNotNull(data, "data");
      Assert.ArgumentNotNull(destinationIntegrationItem, "destinationIntegrationItem");

      ValidateIntegrationItem(destinationIntegrationItem);

      string encryptionKey = null;
      if (data.Credentials != null)
      {
        encryptionKey = GetEncryptionKey(destinationIntegrationItem);
        if (string.IsNullOrEmpty(encryptionKey))
        {
          encryptionKey = Convert.ToBase64String(CryptoManager.GenerateKey());
          SaveEncryptionKey(encryptionKey, destinationIntegrationItem);
        }
      }

      using (new EditContext(destinationIntegrationItem))
      {
        destinationIntegrationItem[FieldIDs.IntegrationConfigData] = data.SerializeToString(encryptionKey);

        new CheckboxField(destinationIntegrationItem.Fields[FieldIDs.ScheduledBlobTransfer])
        {
          Checked = data.ScheduledBlobTransfer
        };

        new CheckboxField(destinationIntegrationItem.Fields[FieldIDs.BidirectionalLink])
        {
          Checked = data.BidirectionalLink
        };
      }
    }

    public void OnItemSaved(object sender, EventArgs args)
    {
      Assert.ArgumentNotNull(sender, "sender");
      var item = Event.ExtractParameter(args, 0) as Item;
      Assert.IsNotNull(item, "No item in parameters");

      using (new IntegrationDisabler())
      {
        if (SharepointProvider.IsActiveIntegrationConfigItem(item))
        {
          RemoveChildIntegrationFoldersFromCache(item);
        }
      }
    }

    public void OnItemSavedRemote(object sender, EventArgs args)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(args, "args");
      var savedRemoteEventArgs = args as ItemSavedRemoteEventArgs;
      if (savedRemoteEventArgs == null)
      {
        return;
      }

      using (new IntegrationDisabler())
      {
        if (savedRemoteEventArgs.Changes.FieldChanges.Contains(FieldIDs.IntegrationConfigData))
        {
          RemoveChildIntegrationFoldersFromCache(savedRemoteEventArgs.Item);
        }
      }
    }

    /// <exception cref="Exception">Throws <c>Exception</c> if encryption key for Integration Config Data of the source item does not exist.</exception>
    [CanBeNull]
    public static IntegrationConfigData GetFromItem([NotNull] Item sourceIntegrationItem)
    {
      Assert.ArgumentNotNull(sourceIntegrationItem, "sourceIntegrationItem");

      ValidateIntegrationItem(sourceIntegrationItem);

      string xmlData = sourceIntegrationItem.Fields[FieldIDs.IntegrationConfigData].Value;
      if (string.IsNullOrEmpty(xmlData))
      {
        return null;
      }

      string encryptionKey = GetEncryptionKey(sourceIntegrationItem);

      var data = new IntegrationConfigData(xmlData, encryptionKey)
      {
        ScheduledBlobTransfer = new CheckboxField(sourceIntegrationItem.Fields[FieldIDs.ScheduledBlobTransfer]).Checked,
        BidirectionalLink = new CheckboxField(sourceIntegrationItem.Fields[FieldIDs.BidirectionalLink]).Checked
      };

      return data;
    }

    private static void RemoveChildIntegrationFoldersFromCache(Item item)
    {
      IntegrationCache.RemoveIntegrationConfigData(item.ID);
      foreach (Item child in item.Children.Where(SharepointProvider.IsIntegrationFolder))
      {
        RemoveChildIntegrationFoldersFromCache(child);
      }
    }

    private static void SaveEncryptionKey([NotNull] string encryptionKey, [NotNull] Item targetIntegrationItem)
    {
      Assert.ArgumentNotNull(encryptionKey, "encryptionKey");
      Assert.ArgumentNotNull(targetIntegrationItem, "targetIntegrationItem");

      IDTable.Add(EncryptionKeyIDTableEntryPrefix, targetIntegrationItem.ID.ToShortID().ToString(), ID.NewID, ID.Null, encryptionKey);
    }

    [CanBeNull]
    private static string GetEncryptionKey([NotNull] Item targetIntegrationItem)
    {
      Assert.ArgumentNotNull(targetIntegrationItem, "targetIntegrationItem");

      IDTableEntry encryptionKeyEntry = IDTable.GetID(EncryptionKeyIDTableEntryPrefix, targetIntegrationItem.ID.ToShortID().ToString());
      if (encryptionKeyEntry == null)
      {
        return null;
      }

      return encryptionKeyEntry.CustomData;
    }

    /// <exception cref="Exception">Throws <c>Exception</c> if the target item does not contain IntegrationConfigData field.</exception>
    private static void ValidateIntegrationItem([NotNull] Item targetIntegrationItem)
    {
      Assert.ArgumentNotNull(targetIntegrationItem, "targetIntegrationItem");

      if (targetIntegrationItem.Template.GetField(FieldIDs.IntegrationConfigData) == null)
      {
        string message = string.Format("Item \"{0}\" does not contain IntegrationConfigData field.", targetIntegrationItem.Paths.FullPath);
        throw new Exception(message);
      }
    }
  }
}