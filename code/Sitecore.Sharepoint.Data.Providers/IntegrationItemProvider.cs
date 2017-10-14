// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationItemProvider.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines IntegrationItemProvider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers
{
  using System;
  using Sitecore.Data;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Diagnostics;
  using Sitecore.Resources.Media;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.Data.Providers.Pipelines.Runners;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Sharepoint.Pipelines.TranslateSharepointValue;
  using SharepointBaseItem = Sitecore.Sharepoint.ObjectModel.Entities.Items.BaseItem;
  using SharepointFieldIDs = Sitecore.Sharepoint.Common.FieldIDs;

  /// <summary>
  /// Defines atomic operations for populating sitecore tree by Sharepoint data.
  /// </summary>
  public static class IntegrationItemProvider
  {
    /// <summary>
    /// Add new item to CMS from SharePoint
    /// </summary>
    /// <param name="integrationItemID">The item id.</param>
    /// <param name="sourceSharepointItem">The source.</param>
    /// <param name="integrationItemTemplateID">Template ID to be used to create a new item</param>
    /// <param name="synchContext">The synchronization context.</param>
    /// <returns>Adding item.</returns>
    [CanBeNull]
    public static Item CreateItem(
                                  [NotNull] ID integrationItemID,
                                  [NotNull] ID integrationItemTemplateID,
                                  [NotNull] SharepointBaseItem sourceSharepointItem,
                                  [NotNull] SynchContext synchContext)
    {
      Assert.ArgumentNotNull(integrationItemID, "integrationItemID");
      Assert.ArgumentNotNull(integrationItemTemplateID, "integrationItemTemplateID");
      Assert.ArgumentNotNull(sourceSharepointItem, "sourceSharepointItem");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      var validName = MediaPathManager.ProposeValidMediaPath(sourceSharepointItem.Title);

      Item integrationItem = ItemManager.AddFromTemplate(validName, integrationItemTemplateID, synchContext.ParentItem, integrationItemID);

      var folderItem = sourceSharepointItem as FolderItem;
      if (folderItem != null && integrationItem != null)
      {
        UpdateIntegrationConfigData(integrationItem, folderItem, synchContext);
      }

      return integrationItem;
    }

    /// <summary>
    /// Update fields of integration item in CMS from SharePoint.
    /// </summary>
    /// <param name="targetIntegrationItem">The item.</param>
    /// <param name="sourceSharepointItem">The source.</param>
    /// <param name="synchContext">The synchronization context.</param>
    public static void UpdateFields([NotNull] Item targetIntegrationItem, [NotNull] SharepointBaseItem sourceSharepointItem, [NotNull] SynchContext synchContext)
    {
      Assert.ArgumentNotNull(targetIntegrationItem, "targetIntegrationItem");
      Assert.ArgumentNotNull(sourceSharepointItem, "sourceSharepointItem");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      var folderItem = sourceSharepointItem as FolderItem;
      if (folderItem != null)
      {
        UpdateIntegrationConfigData(targetIntegrationItem, folderItem, synchContext);
      }

      using (new EditContext(targetIntegrationItem))
      {
        if (string.IsNullOrEmpty(targetIntegrationItem.Fields[FieldNames.GUID].Value))
        {
          if (targetIntegrationItem.Template.GetField(SharepointFieldIDs.IsIntegrationItem) != null)
          {
            new CheckboxField(targetIntegrationItem.Fields[SharepointFieldIDs.IsIntegrationItem]).Checked = true;
          }
        }

        targetIntegrationItem.Fields[FieldNames.GUID].Value = sourceSharepointItem.GUID;

        foreach (IntegrationConfigData.FieldMapping mapping in synchContext.IntegrationConfigData.FieldMappings)
        {
          if (targetIntegrationItem.Fields[mapping.Target] != null)
          {
            TranslateSharepointValueArgs pipelineArgs = TranslateSynchValuePipelinesRunner.TranslateSharepointValue(sourceSharepointItem, mapping.Source, targetIntegrationItem, mapping.Target);
            if (pipelineArgs != null)
            {
              targetIntegrationItem[mapping.Target] = pipelineArgs.TranslatedValue;
            }
          }
        }
      }
    }

    /// <summary>
    /// Update blob field from SharePoint to CMS item.
    /// </summary>
    /// <param name="targetIntegrationItem">The item.</param>
    /// <param name="sourceSharepointDocumentItem">The source.</param>
    /// <param name="synchContext">The synchronization context.</param>
    public static void UpdateBlob([NotNull] Item targetIntegrationItem, [NotNull] DocumentItem sourceSharepointDocumentItem, [NotNull] SynchContext synchContext)
    {
      Assert.ArgumentNotNull(targetIntegrationItem, "targetIntegrationItem");
      Assert.ArgumentNotNull(sourceSharepointDocumentItem, "sourceSharepointDocumentItem");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      using (new IntegrationDisabler())
      {
        if (targetIntegrationItem.Fields[FieldNames.Blob] != null)
        {
          string extension = StringUtil.GetLastPostfix(sourceSharepointDocumentItem.Title, '.');
          var mediaItem = new MediaItem(targetIntegrationItem);
          Media media = MediaManager.GetMedia(mediaItem);
          media.SetStream(sourceSharepointDocumentItem.GetStream(), extension);
        }

        using (new EditContext(targetIntegrationItem))
        {
          if (targetIntegrationItem.Fields[FieldNames.SharepointDataModified] != null)
          {
            DateTime owsModified = Convert.ToDateTime(sourceSharepointDocumentItem["ows_Modified"]);
            targetIntegrationItem[FieldNames.SharepointDataModified] = DateUtil.ToIsoDate(owsModified);
          }
        }
      }
    }

    /// <summary>
    /// Set integration configuration data for integration folder or integration configuration item.
    /// </summary>
    /// <param name="synchContext">The synchronization context.</param>
    /// <param name="sourceSharepointFolderItem">The source.</param>
    /// <param name="targetIntegrationItem">The item  .</param>
    private static void UpdateIntegrationConfigData([NotNull] Item targetIntegrationItem, [NotNull] FolderItem sourceSharepointFolderItem, [NotNull] SynchContext synchContext)
    {
      Assert.ArgumentNotNull(targetIntegrationItem, "targetIntegrationItem");
      Assert.ArgumentNotNull(sourceSharepointFolderItem, "sourceSharepointFolderItem");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      IntegrationConfigData integrationConfigData = synchContext.IntegrationConfigData.Clone();
      integrationConfigData.Folder = StringUtil.RemovePrefix('/', StringUtil.EnsurePostfix('/', synchContext.IntegrationConfigData.Folder) + sourceSharepointFolderItem.Title + "/");

      IntegrationConfigDataProvider.SaveToItem(integrationConfigData, targetIntegrationItem);
    }
  }
}
