// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingEntryControl.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Mapping control to allow user work with mapping between Sharepoint and Sitecore fields
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Web.Shell.Applications.IntegrationWizard
{
  using System;
  using System.Collections.Generic;
  using System.Xml;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
  using Sitecore.Web.UI;
  using Sitecore.Web.UI.HtmlControls;
  using Sitecore.Web.UI.Sheer;
  using Sitecore.Web.UI.XmlControls;

  /// <summary>
  /// Mapping control to allow user work with mapping between Sharepoint and Sitecore fields
  /// </summary>
  public class MappingEntryControl : XmlControl, IMessageHandler
  {
    /// <summary>
    /// Combobox to store/select sharepoint fields
    /// </summary>
    [UsedImplicitly]
    protected Combobox sharepointField;

    /// <summary>
    /// Edit where user can type sitecore field name
    /// </summary>
    [UsedImplicitly]
    protected Edit sitecoreFieldName;

    /// <summary>
    /// main div around the control
    /// </summary>
    [UsedImplicitly]
    protected Border key;

    /// <summary>
    /// Image button to remove mapping from the page
    /// </summary>
    [UsedImplicitly]
    protected ImageButton removeField;

    /// <summary>
    /// validation mark that is shown if Sitecore item name is invalid
    /// </summary>
    [UsedImplicitly]
    protected Border validationMark;

    /// <summary>
    /// Container for validation mark
    /// </summary>
    [UsedImplicitly]
    protected Border vmContainer;

    /// <summary>
    /// Sets the available Sharepoint fields.
    /// </summary>
    /// <value>The available Sharepoint fields.</value>
    public List<string> AvailableFields
    {
      set
      {
        Assert.ArgumentNotNull(value, "value");

        foreach (string field in value)
        {
          this.sharepointField.Controls.Add(new ListItem { Header = XmlConvert.DecodeName(field), Value = field });
        }
      }
    }

    /// <summary>
    /// Gets or sets the selected sharepoint field.
    /// </summary>
    /// <value>The selected sharepoint field.</value>
    [NotNull]
    public string SharepointField
    {
      get
      {
        return this.sharepointField.SelectedItem.Value;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.sharepointField.Value = value;
      }
    }

    /// <summary>
    /// Gets or sets the name of sitecore field.
    /// </summary>
    /// <value>The sitecore field.</value>
    [NotNull]
    public string SitecoreField
    {
      get
      {
        return this.sitecoreFieldName.Value;
      }
    
      set
      {
        Assert.ArgumentNotNull(value, "value");
        this.sitecoreFieldName.Value = value;
      }
    }

    /// <summary>
    /// Gets or sets the key (unique ID for the whole control).
    /// </summary>
    /// <value>The ID for control (must be unique)</value>
    [NotNull]
    public string Key
    {
      get
      {
        return StringUtil.GetString(ViewState["key"]);
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        ViewState["key"] = value;
        this.key.ID = value;
        this.sharepointField.ID = "cb_" + value;
        this.sitecoreFieldName.ID = "ed_" + value;
        this.validationMark.ID = "vm_" + value;
        this.vmContainer.ID = "vmc_" + value;
        this.removeField.Click = "spintegration:removemapping(key=" + value + ")";
      }
    }

    /// <summary>
    /// Validate Sitecore item name
    /// </summary>
    /// <returns>
    /// true if item name is valid otherwise - false
    /// </returns>
    public bool Validate()
    {
      string errorMessage = ItemUtil.GetItemNameError(this.SitecoreField);
      if (!string.IsNullOrEmpty(errorMessage))
      {
        this.validationMark.ToolTip = Translate.Text(errorMessage);
        this.validationMark.Visible = true;
        return false;
      }
      else
      {
        this.validationMark.Visible = false;
        return true;
      }
    }

    /// <summary>
    /// Handles the message.
    /// </summary>
    /// <param name="message">The message.</param>
    public override void HandleMessage([NotNull] Message message)
    {
      Assert.ArgumentNotNull(message, "message");

      if (message.Name == "entry:SitecoreNameChange")
      {
        this.SitecoreNameChange();
      }
      else
      {
        base.HandleMessage(message);
      }
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnLoad([NotNull] EventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");

      base.OnLoad(e);

      // Workaround to prevent hiding image buttons when mappings container is updated
      this.removeField.RenderAs = RenderAs.WebControl;
      
      // Workaround to initialize all controls with necessary data. See setter of Key property
      this.Key = this.Key;
    }

    /// <summary>
    /// Event that is triggered if SitecoreField Edit has been changed
    /// </summary>
    [HandleMessage("entry:SitecoreNameChange")]
    protected void SitecoreNameChange()
    {
      this.Validate();
      Sitecore.Context.ClientPage.ClientResponse.Refresh(this.validationMark);
      Sitecore.Context.ClientPage.ClientResponse.SetReturnValue(true);
    }

  }
}