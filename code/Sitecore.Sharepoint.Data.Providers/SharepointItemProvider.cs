// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharepointItemProvider.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines SharepointItemProvider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers
{
  using System;
  using System.IO;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.Runners;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Sharepoint.Pipelines.ProcessIntegrationItem;
  using Sitecore.Sharepoint.Pipelines.TranslateIntegrationValue;
  using SharepointBaseItem = Sitecore.Sharepoint.ObjectModel.Entities.Items.BaseItem;

  public static class SharepointItemProvider
  {
    [CanBeNull]
    public static FolderItem CreateFolderItem([NotNull] string itemName, [NotNull] SynchContext synchContext)
    {
      Assert.ArgumentNotNull(itemName, "itemName");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      var folderItem = new FolderItem(itemName, synchContext.IntegrationConfigData.Folder, synchContext.IntegrationConfigData.List, new Uri(StringUtil.RemovePostfix('/', synchContext.IntegrationConfigData.Server) + StringUtil.EnsurePrefix('/', synchContext.IntegrationConfigData.Web)), synchContext.Context);
      folderItem.Update();

      return folderItem;
    }

    [CanBeNull]
    public static DocumentItem CreateDocumentItem([NotNull] string itemName, [NotNull] SynchContext synchContext, Stream stream = null)
    {
      Assert.ArgumentNotNull(itemName, "itemName");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      var documentItem = new DocumentItem(itemName, synchContext.IntegrationConfigData.Folder, synchContext.IntegrationConfigData.List, new Uri(StringUtil.RemovePostfix('/', synchContext.IntegrationConfigData.Server) + StringUtil.EnsurePrefix('/', synchContext.IntegrationConfigData.Web)), synchContext.Context);
      documentItem.SetStream(stream ?? Stream.Null);
      documentItem.Update();

      return documentItem;
    }

    public static void UpdateItem(
                                  [NotNull] Item sourceIntegrationItem,
                                  [NotNull] SharepointBaseItem targetSharepointItem,
                                  [NotNull] SynchContext synchContext)
    {
      Assert.ArgumentNotNull(sourceIntegrationItem, "sourceIntegrationItem");
      Assert.ArgumentNotNull(targetSharepointItem, "targetSharepointItem");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      CopyFieldValues(sourceIntegrationItem, targetSharepointItem, synchContext.IntegrationConfigData);

      var documentItem = targetSharepointItem as DocumentItem;
      if (documentItem != null)
      {
        CopyBlobValue(sourceIntegrationItem, documentItem);
      }

      targetSharepointItem.Update();
    }

    public static void CopyFieldValues(
                                       [NotNull] Item sourceIntegrationItem,
                                       [NotNull] SharepointBaseItem targetSharepointItem,
                                       [NotNull] IntegrationConfigData integrationConfigData)
    {
      Assert.ArgumentNotNull(sourceIntegrationItem, "sourceIntegrationItem");
      Assert.ArgumentNotNull(targetSharepointItem, "targetSharepointItem");
      Assert.ArgumentNotNull(integrationConfigData, "integrationConfigData");

      foreach (IntegrationConfigData.FieldMapping fieldMapping in integrationConfigData.FieldMappings)
      {
        if (sourceIntegrationItem.Fields[fieldMapping.Target] == null)
        {
          continue;
        }

        TranslateIntegrationValueArgs pipelineArgs = TranslateSynchValuePipelinesRunner.TranslateIntegrationValue(
                                                                                                                  sourceIntegrationItem,
                                                                                                                  fieldMapping.Target,
                                                                                                                  targetSharepointItem,
                                                                                                                  fieldMapping.Source);
        if (pipelineArgs != null && !(String.IsNullOrEmpty(targetSharepointItem[fieldMapping.Source]) && String.IsNullOrEmpty(pipelineArgs.TranslatedValue)))
        {
          targetSharepointItem[fieldMapping.Source] = pipelineArgs.TranslatedValue;
        }
      }
    }

    public static void CopyBlobValue([NotNull] Item sourceIntegrationItem, [NotNull] DocumentItem targetSharepointItem)
    {
      Assert.ArgumentNotNull(sourceIntegrationItem, "sourceIntegrationItem");
      Assert.ArgumentNotNull(targetSharepointItem, "targetSharepointItem");

      Field blobField = sourceIntegrationItem.Fields[FieldNames.Blob];
      if (blobField == null || !blobField.IsBlobField)
      {
        return;
      }

      targetSharepointItem.SetStream(blobField.GetBlobStream() ?? Stream.Null);
    }
  }
}
