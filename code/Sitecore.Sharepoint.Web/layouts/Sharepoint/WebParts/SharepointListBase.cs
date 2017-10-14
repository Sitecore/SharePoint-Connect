// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharepointListBase.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines SharepointListBase class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Intranet.Sharepoint.Web.Layouts.WebParts
{
  using System;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
  using Sitecore.Intranet.WebParts;
  using Sitecore.Sharepoint.Common.Texts;

  using SharepointListBaseControl = Sitecore.Sharepoint.Web.SharepointListBase;

  public abstract class SharepointListBase : GenericCatalogWebPart
  {
    protected const string ServerSettingName = "Server";
    protected const string WebSettingName = "Web";
    protected const string ListSettingName = "List";

    [NotNull]
    protected abstract string InnerControlVirtualPath { get; }

    [NotNull]
    protected virtual string LoadInnerControlErrorMessage
    {
      get
      {
        string message = Translate.Text(UIMessages.Layout_0_CouldNotBeLoaded);
        return string.Format(message, this.InnerControlVirtualPath);
      }
    }

    [NotNull]
    protected virtual string EmptyRequiredSettingsErrorMessage
    {
      get
      {
        string message = Translate.Text(UIMessages.Text0And1WebPartSettingsAreRequired);
        return string.Format(message, WebSettingName, ListSettingName);
      }
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      this.AddInnerControl();
    }

    protected virtual void AddInnerControl()
    {
      var innerControl = this.LoadControl(this.InnerControlVirtualPath) as SharepointListBaseControl;
      if (innerControl == null)
      {
        this.AddErrorMessage(this.LoadInnerControlErrorMessage);
        return;
      }

      if (!this.SetInnerControlProperties(innerControl))
      {
        return;
      }

      this.Controls.Add(innerControl);
    }

    protected virtual bool SetInnerControlProperties([NotNull] SharepointListBaseControl innerControl)
    {
      Assert.ArgumentNotNull(innerControl, "innerControl");

      string server = this.GetSettingValue(ServerSettingName);
      if (!string.IsNullOrEmpty(server))
      {
        innerControl.Server = server;
      }

      string web = this.GetSettingValue(WebSettingName);
      if (string.IsNullOrEmpty(web))
      {
        this.AddErrorMessage(this.EmptyRequiredSettingsErrorMessage);
        return false;
      }

      innerControl.Web = web;

      string list = this.GetSettingValue(ListSettingName);
      if (string.IsNullOrEmpty(list))
      {
        this.AddErrorMessage(this.EmptyRequiredSettingsErrorMessage);
        return false;
      }

      innerControl.ListName = list;

      return true;
    }

    [NotNull]
    protected virtual string GetSettingValue([NotNull] string settingName)
    {
      Assert.ArgumentNotNullOrEmpty(settingName, "settingName");

      string settingValue = this.GetCustomSetting(settingName);
      if (!string.IsNullOrEmpty(settingValue))
      {
        return settingValue;
      }

      if (this.ConfigItem == null)
      {
        return string.Empty;
      }

      return this.ConfigItem[settingName];
    }

    protected virtual void AddErrorMessage([NotNull] string errorMessage)
    {
      Assert.ArgumentNotNull(errorMessage, "errorMessage");

      var errorControl = new Label
      {
        CssClass = "webPartErrorMessage",
        Text = errorMessage
      };

      this.Controls.Add(errorControl);
    }

    [CanBeNull]
    protected virtual Control LoadControl([NotNull] string virtualPath)
    {
      Assert.ArgumentNotNullOrEmpty(virtualPath, "virtualPath");

      try
      {
        return Page.LoadControl(virtualPath);
      }
      catch (Exception ex)
      {
        string message = string.Format(LogMessages.Control0CouldNotBeLoaded, virtualPath);
        Log.Error(message, ex, this);

        return null;
      }
    }
  }
}