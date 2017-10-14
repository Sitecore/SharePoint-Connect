// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateBlob.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines UpdateBlob class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.ProcessIntegrationItem
{
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;

  public class UpdateBlob
  {
    public virtual void Process([NotNull] ProcessIntegrationItemArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Assert.IsNotNull(args.Options, "args.Options");

      if (!args.Options.ScheduledBlobTransfer && args.SynchContext.IntegrationConfigData.ScheduledBlobTransfer)
      {
        return;
      }

      this.Process(args.IntegrationItem, args.SourceSharepointItem, args.SynchContext);
    }

    protected void Process(
                           [NotNull] Item integrationItem, 
                           [NotNull] ObjectModel.Entities.Items.BaseItem sourceSharepointItem, 
                           [NotNull] SynchContext synchContext)
    {
      Assert.ArgumentNotNull(integrationItem, "integrationItem");
      Assert.ArgumentNotNull(sourceSharepointItem, "sourceSharepointItem");
      Assert.ArgumentNotNull(synchContext, "synchContext");

      var sourceSharepointDocumentItem = sourceSharepointItem as DocumentItem;
      if (sourceSharepointDocumentItem == null)
      {
        return;
      }

      var sharepointDataModifiedField = integrationItem.Fields[FieldNames.SharepointDataModified];
      if (sharepointDataModifiedField != null &&
          DateUtil.ToServerTime(new DateField(sharepointDataModifiedField).DateTime) ==  DateUtil.ToServerTime(System.Convert.ToDateTime(sourceSharepointDocumentItem["ows_Modified"]).ToUniversalTime()))
      {
        return;
      }

      IntegrationItemProvider.UpdateBlob(integrationItem, sourceSharepointDocumentItem, synchContext);
    }
  }
}