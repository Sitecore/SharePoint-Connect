// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateItem.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines CreateItem class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.CreateSharepointItem
{
  using System;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Pipelines.ProcessSharepointItem;

  public class CreateItem
  {
    public virtual void Process([NotNull] ProcessSharepointItemArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Assert.IsNotNullOrEmpty(args.SourceIntegrationItemName, "args.SourceIntegrationItemName");
      Assert.IsNotNull(args.SourceIntegrationItemTemplateID, "args.SourceIntegrationItemTemplateID");
      Assert.IsNotNull(args.SynchContext, "args.SynchContext");

      TemplateItem templateItem = args.SynchContext.Database.GetTemplate(args.SourceIntegrationItemTemplateID);
      if (templateItem == null)
      {
        return;
      }

      TemplateFieldItem blobTemplateField = templateItem.GetField(FieldNames.Blob);
      if (blobTemplateField != null && blobTemplateField.Type.ToLower() == "attachment")
      {
        try
        {
          var sourceItem = args.SourceIntegrationItem;
          args.SharepointItem = SharepointItemProvider.CreateDocumentItem(args.SourceIntegrationItemName, args.SynchContext, sourceItem.Fields[FieldNames.Blob].GetBlobStream());
          
          using (new EditContext(sourceItem))
          {
            if (sourceItem.Fields[FieldNames.SharepointDataModified] != null)
            {
              DateTime owsModified = Convert.ToDateTime(args.SharepointItem["ows_Modified"]);
              sourceItem[FieldNames.SharepointDataModified] = DateUtil.ToIsoDate(owsModified);
            }
          }
        }
        catch (Exception e)
        {
          args.AddMessage(e.Message);
          args.AbortPipeline();
        }
      }

      if (args.SourceIntegrationItemTemplateID == TemplateIDs.IntegrationFolder)
      {
        args.SharepointItem = SharepointItemProvider.CreateFolderItem(args.SourceIntegrationItemName, args.SynchContext);
      }
    }
  }
}