// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogMessages.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines LogMessages class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Texts
{
  using System;

  public static class LogMessages
  {
    public const string CanNotFindRequired0AttributeOfSharePointServerSetting = "Can not find required \"{0}\" attribute of SharePoint server setting.";
    
    public const string CanNotFind0AttributeOfSharePointServerSettingDefaultValueWillBeUsed = "Can not find \"{0}\" attribute of SharePoint server setting. Default value will be used.";

    public const string Control0CouldNotBeLoaded = "Control \"{0}\" could not be loaded.";

    public const string IntegrationConfigurationDataOf0ItemDoesNotContainValidTemplateIDSetting = "Integration configuration data of \"{0}\" item does not contain valid TemplateID setting.";

    public const string IntegrationItem0IsLockedAndCouldNotBeSynchronizedWithSharePointServer = "Integration item \"{0}\" is locked and could not be synchronized with SharePoint server.";

    public const string SharepointItem0IsCheckedOutAndCouldNotBeSynchronizedWithSitecore = "Sharepoint item \"{0}\" is checked out and could not be synchronized with Sitecore.";

    public const string SitecoreItem0IsDeletedAndCouldNotBeSynchronizedWithSharepoint = "Sitecore item \"{0}\" is deleted and could not be synchronized with Sharepoint.";

    public const string Text0PipelineFailedToExecute = "{0} pipeline failed to execute.";

    [Obsolete ("Standard message from Sitecore.Kernel is used instead.")]
    public const string Text0PipelineNotPresentOrUnreadableInWebConfig = "{0} pipeline not present, or unreadable, in web.config.";

    public const string IntegrationConfigItemID02 = "Integration config item ID: {0}, {1}";

    public const string Web01List23 = "Web: {0}{1} List: {2}{3}";

    public const string Text0IntegrationItem1HasBeenFailed = "{0} integration item {1} has been failed";

    public const string Text0SharePointItem1HasBeenFailed = "{0} SharePoint item {1} has been failed";

    public const string Text0Item1 = "{0} Item: {1}";
  }
}
