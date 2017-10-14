// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateIsSharepointOnlinePostStepAction.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the UpdateIsSharepointOnlinePostStepAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Installer
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Text;
  using Sitecore.Data;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Install.Framework;
  using Sitecore.SecurityModel;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Data.Providers;

  public class UpdateIsSharepointOnlinePostStepAction : IPostStep
  {
    private const string ModifiedFieldMessageFormat = "If the following temlates: {0} utilize the Uploading Modified BLOBs Only feature, they should be reconfigured." +
      " See the documentation how to confiure it.";

    private readonly HashSet<ID> notWellConfiguredTemplates = new HashSet<ID>();
    
    private Field field;

    public void Run(ITaskOutput output, NameValueCollection metaData)
    {
      bool isSuccessful = true;
      using (new SecurityDisabler())
      {
        using (new IntegrationDisabler())
        {
          var ids = new[]
          {
            TemplateIDs.IntegrationConfig, TemplateIDs.IntegrationFolder
          };

          foreach (var id in ids)
          {
            string query = string.Format("fast://*[@@templateid = '{0}']", id);
            foreach (Item item in Context.ContentDatabase.SelectItems(query))
            {
              isSuccessful = this.UpdateIntegrationConfigData(item) && isSuccessful;
              this.UpdateModifiedFields(item);
            }
          }
        }
      }

      var message =
        isSuccessful ? "All integration config items updated successfully."
          : "Some integration config items haven't been updated. See log for details.";

      if (this.notWellConfiguredTemplates.Count > 0)
      {
        Log.Warn(string.Format(ModifiedFieldMessageFormat, string.Join(";", this.notWellConfiguredTemplates)), this);
        message += "\r\nUploading Modified BLOBs Only feature need to be reconfigured. See log for details.";
      }
      else
      {
        message += "\r\nUploading Modified BLOBs Only feature configured successfully.";
      }

      output.Alert(message);
    }

    protected void UpdateModifiedFields(Item item)
    {
      if (item[FieldIDs.IntegrationConfigData].Contains("<Source>ows_Modified</Source><Target>Modified</Target>"))
      {
        return;
      }

      foreach (Item descendant in item.GetChildren().Where(x => x.Fields.Any(y => y.ID == FieldIDs.IsIntegrationItem) && x.Fields.Any(z => z.Name == FieldNames.Blob && z.IsBlobField)))
      {
        var oldField = descendant.Fields["Modified"];
        if (oldField != null)
        {
          var newField = descendant.Fields[FieldNames.SharepointDataModified];
          if (newField != null)
          {
            using (new EditContext(descendant))
            {
              descendant[FieldNames.SharepointDataModified] = oldField.Value;
            }
          }
          else
          {
            this.notWellConfiguredTemplates.Add(descendant.TemplateID);
          }
        }
      }
    }

    protected bool UpdateIntegrationConfigData(Item item)
    {
      try
      {
        using (new EditContext(item))
        {
          item[FieldIDs.IntegrationConfigData] = this.ReplaceSharepointOnline(item[FieldIDs.IntegrationConfigData]);
        }
        
        return true;
      }
      catch (Exception exception)
      {
        var errorMessage = new StringBuilder("Updating integration config data has been failed.");
        errorMessage.AppendLine(string.Format("Integration config item: {0}", item.ID));
        
        Log.Error(errorMessage.ToString(), exception, this);

        return false;
      }
    }

    protected string ReplaceSharepointOnline(string configData)
    {
      var ret = configData;
      ret = ret.Replace("<SharepointOnline>False</SharepointOnline>", string.Empty);
      ret = ret.Replace("<SharepointOnline>True</SharepointOnline>", "<ConnectionConfiguration>SharePointOnline</ConnectionConfiguration>");
      return ret;
    }
  }
}