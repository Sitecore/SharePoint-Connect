// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogMessageFormatter.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the LogMessageFormatter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.Logging
{
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Texts;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;

  internal class LogMessageFormatter
  {
    [NotNull]
    public static string FormatIntegrationConfigItemID02([NotNull] SynchContext context)
    {
      Assert.ArgumentNotNull(context, "context");
      return string.Format(
        LogMessages.IntegrationConfigItemID02,
        context.ParentID,
        FormatWeb01List23(context.IntegrationConfigData));
    }

    [NotNull]
    public static string FormatWeb01List23([NotNull] IntegrationConfigData data)
    {
      Assert.ArgumentNotNull(data, "data");

      return string.Format(
        LogMessages.Web01List23,
        data.Server.Trim('/'),
        string.IsNullOrEmpty(data.Web) ? string.Empty : StringUtil.EnsurePrefix('/', data.Web.TrimEnd('/')),
        data.List,
        string.IsNullOrEmpty(data.Folder) ? string.Empty : " Folder: " + data.Folder.Trim('/'));
    }

    [NotNull]
    public static string FormatText0Item1([NotNull] IntegrationConfigData integrationConfigData, [NotNull] string itemName)
    {
      Assert.ArgumentNotNull(integrationConfigData, "integrationConfigData");
      Assert.ArgumentNotNullOrEmpty(itemName, "itemName");
      
      return string.Format(
        LogMessages.Text0Item1, 
        FormatWeb01List23(integrationConfigData), 
        itemName);
    }
  }
} 