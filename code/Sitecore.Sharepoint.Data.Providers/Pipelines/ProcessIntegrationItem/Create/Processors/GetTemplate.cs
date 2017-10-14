// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetTemplate.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines GetTemplate class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Pipelines.CreateIntegrationItem
{
  using Sitecore.Data;
  using Sitecore.Data.Managers;
  using Sitecore.Data.Templates;
  using Sitecore.Diagnostics;
  using Sitecore.Resources.Media;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Common.Texts;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Sharepoint.Pipelines.ProcessIntegrationItem;

  public class GetTemplate
  {
    public virtual void Process([NotNull] ProcessIntegrationItemArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Assert.IsNotNull(args.SourceSharepointItem, "args.SourceSharepointItem");
      Assert.IsNotNull(args.SynchContext, "args.SynchContext");

      if (args.SourceSharepointItem is DocumentItem)
      {
        string extension = StringUtil.GetLastPostfix(args.SourceSharepointItem.Title, '.');

        string templateName = MediaManager.Config.GetTemplate(extension, false);
        if (!string.IsNullOrEmpty(templateName))
        {
          Template template = TemplateManager.GetTemplate(templateName, args.SynchContext.Database);
          if (template != null)
          {
            args.IntegrationItemTemplateID = template.ID;
            return;
          }
        }
      }

      if (args.SourceSharepointItem is FolderItem)
      {
        args.IntegrationItemTemplateID = TemplateIDs.IntegrationFolder;
        return;
      }

      ID templateID;
      if (!ID.TryParse(args.SynchContext.IntegrationConfigData.TemplateID, out templateID))
      {
        string logMessage = string.Format(LogMessages.IntegrationConfigurationDataOf0ItemDoesNotContainValidTemplateIDSetting, args.SynchContext.ParentItem.Paths.Path);
        Log.Error(logMessage, this);

        return;
      }

      args.IntegrationItemTemplateID = templateID;
    }
  }
}