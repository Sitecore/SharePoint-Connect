// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharepointList.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines SharepointList web part.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Intranet.Sharepoint.Web.Layouts.WebParts
{
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
  using Sitecore.Sharepoint.Common.Texts;
  using SharepointListBaseControl = Sitecore.Sharepoint.Web.SharepointListBase;

  public class SharepointList : SharepointListBase
  {
    protected const string InnerControlVirtualPathSettingName = "InnerControlVirtualPath";

    private string innerControlVirtualPath;

    protected override string InnerControlVirtualPath
    {
      get
      {
        return this.innerControlVirtualPath;
      }
    }

    protected override string EmptyRequiredSettingsErrorMessage
    {
      get
      {
        string message = Translate.Text(UIMessages.Text01And2WebPartSettingsAreRequired);
        return string.Format(message, InnerControlVirtualPathSettingName, WebSettingName, ListSettingName);
      }
    }

    protected override bool SetInnerControlProperties(SharepointListBaseControl innerControl)
    {
      Assert.ArgumentNotNull(innerControl, "innerControl");

      string virtualPath = this.GetSettingValue(InnerControlVirtualPathSettingName);
      if (string.IsNullOrEmpty(virtualPath))
      {
        this.AddErrorMessage(this.EmptyRequiredSettingsErrorMessage);
        return false;
      }

      this.innerControlVirtualPath = virtualPath;

      return base.SetInnerControlProperties(innerControl);
    }
  }
}