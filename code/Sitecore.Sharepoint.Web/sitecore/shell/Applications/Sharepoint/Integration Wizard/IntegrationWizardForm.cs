// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationWizardForm.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the codebeside for IntegrationWizardForm wizard
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Web.Shell.Applications.IntegrationWizard
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Security.Principal;
  using System.Text;
  using System.Xml;
  using Sitecore.Collections;
  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
  using Sitecore.Jobs;
  using Sitecore.SecurityModel.License;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Common.Configuration;
  using Sitecore.Sharepoint.Common.Texts;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Sharepoint.ObjectModel;
  using Sitecore.Sharepoint.ObjectModel.Entities;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;
  using Sitecore.Text;
  using Sitecore.Web;
  using Sitecore.Web.UI.HtmlControls;
  using Sitecore.Web.UI.Pages;
  using Sitecore.Web.UI.Sheer;
  using Sitecore.Web.UI.WebControls;
  using Sitecore.Web.UI.XmlControls;

  public enum ListType { Library, List }

  /// <summary>
  /// Defines the integration configuration item creator form class.
  /// </summary>
  public class IntegrationWizardForm : WizardForm
  {
    [Serializable]
    protected struct ListInfo
    {
      public ListType ListType;
      public string ListName;
      public Dictionary<string, KeyValuePair<string, List<string>>> Views;
    }

    #region Controls

    [UsedImplicitly]
    private Edit server;

    [UsedImplicitly]
    private Literal currentUser;

    [UsedImplicitly]
    private Literal currentConfiguration;

    [UsedImplicitly]
    private Combobox configuration;

    [UsedImplicitly]
    private Checkbox isAdvanced;

    [UsedImplicitly]
    private Checkbox isCredentials;

    [UsedImplicitly]
    private Dictionary<string, ListInfo> savedSpInfo;

    [UsedImplicitly]
    private Listbox listSelector;

    [UsedImplicitly]
    private Listbox viewSelector;

    [UsedImplicitly]
    private Listbox mappingSelector;

    [UsedImplicitly]
    private Control mappingContainer;

    [UsedImplicitly]
    private Literal confirmSite;

    [UsedImplicitly]
    private Literal confirmList;

    [UsedImplicitly]
    private Literal confirmview;

    [UsedImplicitly]
    private Literal confirmUserName;

    [UsedImplicitly]
    private Literal confirmTemplate;

    [UsedImplicitly]
    private Literal confirmMappings;

    [UsedImplicitly]
    private Literal confirmScheduledBlobTransfer;

    [UsedImplicitly]
    private Literal confirmExpiration;

    [UsedImplicitly]
    private Literal confirmLimit;

    [UsedImplicitly]
    private Literal confirmSharepointUpdate;

    [UsedImplicitly]
    private Literal templateAdditionalInfo;

    [UsedImplicitly]
    private GridPanel mappingInfoPanel;

    [UsedImplicitly]
    private Edit templateName;

    [UsedImplicitly]
    private Checkbox scheduledBlobTransferEnabled;

    [UsedImplicitly]
    private Edit expiration;

    [UsedImplicitly]
    private Edit itemLimit;

    [UsedImplicitly]
    private Edit itemName;

    [UsedImplicitly]
    private Checkbox updateSharepoint;

    [UsedImplicitly]
    private Edit knownServers;

    [UsedImplicitly]
    private Edit defaultUser;

    [UsedImplicitly]
    private Edit defaultConfiguration;

    [UsedImplicitly]
    private RadioItem defaultCredentials;

    [UsedImplicitly]
    private RadioItem alternativeCredentials;

    [UsedImplicitly]
    private RadioItem useExistentMapping;

    [UsedImplicitly]
    private RadioItem createNewMapping;

    [UsedImplicitly]
    private Edit userName;


    [UsedImplicitly]
    private Edit pwd;

    [UsedImplicitly]
    private Database database;

    [UsedImplicitly]
    private Item parentItem;

    [UsedImplicitly]
    private Edit integrationConfigItemIdHolder;

    [UsedImplicitly]
    private Label itemNameValidationMark;

    [UsedImplicitly]
    private Label templateNameValidationMark;

    [UsedImplicitly]
    private Label expirationValidationMark;

    [UsedImplicitly]
    private Label itemLimitValidationMark;

    [UsedImplicitly]
    private Checkbox waitSyncIntegration;

    private static readonly ID uploadFileBranchID = new ID("{0FBEBDBC-55B4-4883-90DF-29906DB4EB47}");
    private static readonly ID uploadFileBranchAdvancedID = new ID("{381BEC54-BC94-4FBB-98B3-C56B37237EE3}");

    #endregion

    /// <summary>
    /// Called when the active page is changing.
    /// </summary>
    /// <param name="page">The page that is being left.</param>
    /// <param name="newpage">The new page that is being entered.</param>
    /// <returns>
    /// True, if the change is allowed, otherwise false.
    /// </returns>
    /// <remarks>Set the newpage parameter to another page ID to control the
    /// path through the wizard pages.</remarks>
    /// <contract>
    ///   <requires name="page" condition="not null"/>
    ///   <requires name="newpage" condition="not null"/>
    /// </contract>
    protected override bool ActivePageChanging([NotNull] string page, [NotNull] ref string newpage)
    {
      Assert.ArgumentNotNull(page, "page");
      Assert.ArgumentNotNull(newpage, "newpage");

      if ((page == "ListSelectorPage") && (newpage == "ConnectToSp"))
      {
        newpage = "ConnectToSharePoint";
      }

      if (page == "ListSelectorPage" && newpage == "ViewSelectorPage")
      {
        if (string.IsNullOrEmpty(this.listSelector.Value))
        {
          this.ShowError(UIMessages.PleaseSelectAList, UIMessages.ValidationFailed);
          return false;
        }
      }

      if (page == "ViewSelectorPage" && newpage != "ListSelectorPage")
      {
        if (string.IsNullOrEmpty(this.viewSelector.Value))
        {
          this.ShowError(UIMessages.PleaseSelectAView, UIMessages.ValidationFailed);
          return false;
        }
      }


      if (newpage == "ExistentMappingsPage" && (this.GetExistentMappings().Count == 0 || this.IsLibrary()))
      {
        if (page == "ViewSelectorPage")
        {
          newpage = "MappingPage";
        }
        else
        {
          newpage = "ViewSelectorPage";
        }
      }

      if (page == "MappingPage" && newpage == "AdvancedSettings")
      {
        bool validationFailed = false;
        foreach (MappingEntryControl mappingEntryControl in this.Mappings)
        {
          if (!mappingEntryControl.Validate())
          {
            Context.ClientPage.ClientResponse.Refresh(mappingEntryControl);
            validationFailed = true;
          }
          if (validationFailed)
          {
            this.ShowError(UIMessages.OneOrMoreSitecoreFieldNamesAreInvalid, UIMessages.ValidationFailed);
            return false;
          }

          if (mappingEntryControl.SitecoreField.ToLower() == FieldNames.SharepointDataModified.ToLower())
          {
            this.ShowError(string.Format(UIMessages.The0FieldIsReservedPleaseSelectAnotherSitecoreField, FieldNames.SharepointDataModified), UIMessages.ValidationFailed);
            return false;
          }
        }
      }

      if (page == "AdvancedSettings" && newpage == "Confirmation")
      {
        if (this.ValidateAdvancedSettings())
        {
          this.ShowError(UIMessages.ValidationFailed, UIMessages.ValidationFailed);
          return false;
        }

      }

      return base.ActivePageChanging(page, ref newpage);
    }

    protected virtual void ShowError(string shortDescription, string longDescription)
    {
      Context.ClientPage.ClientResponse.Alert(shortDescription, string.Empty, longDescription);
    }

    protected void ProcessValidationAdvancedSettings()
    {
      Context.ClientPage.ClientResponse.SetReturnValue(true);
      this.ValidateAdvancedSettings();
    }

    protected bool ValidateAdvancedSettings()
    {
      this.expirationValidationMark.Visible = false;
      this.itemLimitValidationMark.Visible = false;

      bool validationFailed = !this.ValidateItemName(this.itemName, this.itemNameValidationMark);
      validationFailed = !this.ValidateItemName(this.templateName, this.templateNameValidationMark) || validationFailed;

      uint result;
      if (!uint.TryParse(this.expiration.Value, out result) || result == 0)
      {
        validationFailed = !this.SetupValidationMark(this.expirationValidationMark, "This setting must be an integer greater than zero") || validationFailed;
      }

      if (!uint.TryParse(this.itemLimit.Value, out result))
      {
        validationFailed = !this.SetupValidationMark(this.itemLimitValidationMark, "This setting must be an integer") || validationFailed;
      }

      return validationFailed;
    }

    /// <summary>
    /// Validates control so it contains valid item name
    /// </summary>
    /// <param name="control">The control.</param>
    /// <param name="validationMark">The validation mark.</param>
    /// <returns>true if name is valid</returns>
    protected bool ValidateItemName([NotNull] Edit control, [NotNull] Label validationMark)
    {
      Assert.ArgumentNotNull(control, "control");
      Assert.ArgumentNotNull(validationMark, "validationMark");

      return this.SetupValidationMark(validationMark, ItemUtil.GetItemNameError(control.Value));
    }

    /// <summary>
    /// Setups the validation mark. Based on error parameter
    /// </summary>
    /// <param name="validationMark">The validation mark.</param>
    /// <param name="error">The error.</param>
    /// <returns>true if error is empty</returns>
    protected bool SetupValidationMark([NotNull] Label validationMark, [NotNull] string error)
    {
      Assert.ArgumentNotNull(validationMark, "validationMark");
      Assert.ArgumentNotNull(error, "error");

      if (!string.IsNullOrEmpty(error))
      {
        validationMark.ToolTip = error;
        validationMark.Visible = true;
        Context.ClientPage.ClientResponse.Refresh(validationMark);
        return false;
      }

      validationMark.Visible = false;
      Context.ClientPage.ClientResponse.Refresh(validationMark);
      return true;
    }

    /// <summary>
    /// This method is run when Active page has been changed
    /// </summary>
    /// <param name="page">The new page.</param>
    /// <param name="oldPage">The old page.</param>
    /// <contract>
    /// <requires name="page" condition="not null"/>
    /// <requires name="oldPage" condition="not null"/>
    /// </contract>
    protected override void ActivePageChanged([NotNull] string page, [NotNull] string oldPage)
    {
      Assert.ArgumentNotNull(page, "page");
      Assert.ArgumentNotNull(oldPage, "oldPage");

      base.ActivePageChanged(page, oldPage);
      switch (page)
      {
        case "ConnectToSharePoint":
          {
            var knownServersList = new List<string>();
            foreach (var entry in Sharepoint.Common.Configuration.Settings.PredefinedServerEntries)
            {
              var domain = string.Empty;
              var user = this.GetDefaultUserName();
              var serverConfigurator = this.GetDefaultConfiguratorDisplayName();

              var credential = entry.Credentials as NetworkCredential;
              if (credential != null)
              {
                domain = credential.Domain;
                user = credential.UserName;
              }
              
              if (!string.IsNullOrEmpty(entry.ConnectionConfiguration))
              {
                ConnectionConfigurationEntry connectionConfigurationEntry;
                if (Common.Configuration.Settings.ConnectionConfigurations.TryGetValue(entry.ConnectionConfiguration, out connectionConfigurationEntry))
                {
                  serverConfigurator = connectionConfigurationEntry.DisplayName;
                }
                else
                {
                  Log.Error(string.Format("Connection configuration {0} is not found", entry.ConnectionConfiguration), this);
                }
              }

              knownServersList.Add(string.Format("{0}^{1}^{2}", StringUtil.Combine(domain, user, "\\"), entry.Url, serverConfigurator));
            }

            this.knownServers.Value = string.Join("|", knownServersList);
            
            break;
          }

        case "ConnectToSp":
          {
            this.NextButton.Disabled = true;
            this.BackButton.Disabled = true;
            this.CancelButton.Disabled = true;
            this.LoadDataAsync();
            break;
          }

        case "ListSelectorPage":
          {
            if (oldPage != "ViewSelectorPage")
            {
              this.listSelector.Controls.Clear();
              foreach (var list in this.SpInfo)
              {
                this.listSelector.Controls.Add(new ListItem
                {
                  Value = list.Key,
                  Header = list.Value.ListName
                });
              }

              this.listSelector.Size = "10";
              Context.ClientPage.ClientResponse.Refresh(this.listSelector);
            }

            break;
          }

        case "ViewSelectorPage":
          {
            if (oldPage == "ListSelectorPage")
            {
              this.Template = this.IntegrationConfigItemName;
              this.viewSelector.Controls.Clear();
              foreach (var viewName in this.AvailableViews(this.ListName))
              {
                this.viewSelector.Controls.Add(new ListItem
                {
                  Value = viewName.Value,
                  Header = viewName.Key
                });
              }

              this.viewSelector.Size = "10";
              Context.ClientPage.ClientResponse.Refresh(this.viewSelector);
            }

            break;
          }

        case "ExistentMappingsPage":
          {
            if (oldPage == "ViewSelectorPage")
            {
              this.mappingSelector.Controls.Clear();

              Dictionary<ID, IntegrationConfigData> existentMappings = this.GetExistentMappings();
              foreach (KeyValuePair<ID, IntegrationConfigData> data in existentMappings)
              {
                Item template = Database.GetItem(data.Value.TemplateID);
                if (template == null)
                {
                  continue;
                }
                this.mappingSelector.Controls.Add(new ListItem
                {
                  Value = data.Key.ToString(),
                  Header = template.Name
                });
              }

              this.mappingSelector.Size = "10";
              this.createNewMapping.Checked = true;
              Context.ClientPage.ClientResponse.SetAttribute(this.createNewMapping.ID, "checked", "1");
              Context.ClientPage.ClientResponse.SetAttribute(this.useExistentMapping.ID, "checked", string.Empty);
              Context.ClientPage.ClientResponse.Refresh(this.mappingSelector);
              Context.ClientPage.ClientResponse.Eval("createNewMappingRadioClick();");
            }

            break;
          }

        case "MappingPage":
          {
            if (oldPage != "AdvancedSettings")
            {
              List<string> spFields = this.AvailableFields(this.ListName, this.ViewName);
              this.ClearMappings();
              IEnumerable<Pair<string, string>> mappingPairs;
              if (this.UseAlreadyExistentMapping)
              {
                IntegrationConfigData data = SelectedExistentMapping;
                List<Pair<string, string>> tempMappings = new List<Pair<string, string>>();
                foreach (IntegrationConfigData.FieldMapping mapping in data.FieldMappings)
                {
                  string sourceField = StringUtil.RemovePrefix("ows_", mapping.Source);
                  int index = spFields.Contains(sourceField) ? spFields.IndexOf(sourceField) : spFields.IndexOf("ows_" + sourceField);
                  if (index > -1)
                  {
                    tempMappings.Add(new Pair<string, string>(spFields[index], mapping.Target));
                  }
                }
                mappingPairs = tempMappings;

              }
              else
              {
                mappingPairs = spFields.Select(field => new Pair<string, string>(field, XmlConvert.DecodeName(field)));
              }
              foreach (Pair<string, string> field in mappingPairs)
              {
                // Skip ows_Modified field in order to update blob using this field
                if (field.Part1.ToLower() == FieldNames.SharepointDataModified.ToLower())
                {
                  continue;
                }

                MappingEntryControl mapping = ControlFactory.GetControl("Sharepoint.MappingEntry") as MappingEntryControl;
                Assert.IsNotNull(mapping, "Can't load Sharepoint.MappingEntry control");
                mapping.Key = Control.GetUniqueID("me");
                mapping.AvailableFields = spFields;
                mapping.SharepointField = field.Part1;
                mapping.SitecoreField = field.Part2;
                this.mappingContainer.Controls.Add(mapping);
              }
              Context.ClientPage.ClientResponse.Refresh(this.mappingContainer);
            }
            break;
          }

        case "AdvancedSettings":
          {
            this.templateName.ReadOnly = false;
            Context.ClientPage.ClientResponse.Refresh(this.templateName);
            this.templateAdditionalInfo.Visible = false;
            if (this.UseAlreadyExistentMapping)
            {
              if (this.GetAddedMappings().Count > 0)
              {
                // Some new mappings have been added. So new template need to be created but it should be
                // inherited from the selected template
                Item template = this.Database.GetItem(this.SelectedExistentMapping.TemplateID);
                if (template != null)
                {
                  this.templateAdditionalInfo.Visible = true;
                  this.templateAdditionalInfo.Text = Translate.Text(UIMessages.ThisTemplateWillBeBasedOn_0, template.Name);
                }
              }
              else
              {
                this.templateName.ReadOnly = true;
                SheerResponse.SetStyle(this.templateName.ID, "color", "InactiveCaptionText");
                SheerResponse.SetStyle(this.templateName.ID, "background", "InactiveBorder");
              }
            }

            Context.ClientPage.ClientResponse.Refresh(this.templateAdditionalInfo);
            NextButton.Header = "Next >";
            break;
          }

        case "Confirmation":
          {
            this.confirmSite.Text = this.server.Value;
            this.confirmList.Text = this.ListHeader;
            this.confirmview.Text = this.ViewDisplayName;
            this.confirmUserName.Text = this.SharepointUser;
            this.confirmTemplate.Text = this.Template;

            if (this.UseAlreadyExistentMapping && this.GetAddedMappings().Count > 0)
            {
              Item template = this.Database.GetItem(this.SelectedExistentMapping.TemplateID);
              if (template != null)
              {
                this.confirmTemplate.Text += Translate.Text(UIMessages.BasedOn_0, template.Name);
              }
            }

            this.confirmMappings.Text = this.Mappings.Count.ToString();
            this.confirmScheduledBlobTransfer.Text = this.IsScheduledBlobTransferEnabled ? Translate.Text(UIMessages.Enabled) : Translate.Text(UIMessages.Disabled);
            this.confirmExpiration.Text = this.ExpirationInterval;
            this.confirmLimit.Text = this.SpItemLimit;
            this.confirmSharepointUpdate.Text = this.IsSharepointEditable ? Translate.Text(UIMessages.EnabledSharePointItemsMightBeUpdatedIfNecessary) : Translate.Text(UIMessages.DisabledSharePointItemsWillNotBeUpdatedBySitecore);
            NextButton.Header = "Create";
            break;
          }

        case "LastPage":
          {
            ID templateID = this.CreateTemplate();
            using (new IntegrationDisabler())
            {
              Item integrationConfigItem = this.CreateIntegrationConfigItem();
              this.FillIntegrationConfigItem(integrationConfigItem, templateID);
            }

            CancelButton.Header = "Close";
            BackButton.Visible = false;
            NextButton.Visible = false;
            break;
          }

      }
    }

    private string GetDefaultConfiguratorDisplayName()
    {
      return Common.Configuration.Settings.DefaultConfigurator.DisplayName;
    }

    private string GetDefaultUserName()
    {
      return WindowsIdentity.GetCurrent().Name;
    }

    /// <summary>
    /// Fills the integration configuration item with all necessary information so ItemProvider can connect to SharePoint
    /// </summary>
    /// <param name="integrationConfigItem">The integration configuration item.</param>
    /// <param name="templateID">The template ID.</param>
    protected void FillIntegrationConfigItem([NotNull] Item integrationConfigItem, [NotNull] ID templateID)
    {
      Assert.ArgumentNotNull(integrationConfigItem, "integrationConfigItem");
      Assert.ArgumentNotNull(templateID, "templateID");

      IntegrationConfigData data = new IntegrationConfigData(this.SharepointServer, this.ListName, templateID.ToString());

      uint itemRetrievingLimit;
      if (uint.TryParse(this.SpItemLimit, out itemRetrievingLimit))
      {
        data.ItemLimit = itemRetrievingLimit;
      }
      else
      {
        data.ItemLimit = Sharepoint.Common.Configuration.Settings.ItemRetrievingLimit;
      }

      ulong expInterval;
      if (ulong.TryParse(this.ExpirationInterval, out expInterval))
      {
        data.ExpirationInterval = expInterval;
      }
      else
      {
        data.ExpirationInterval = SharepointProvider.DefaultTimeout;
      }

      List<IntegrationConfigData.FieldMapping> mappings = new List<IntegrationConfigData.FieldMapping>();
      foreach (MappingEntryControl mapping in this.Mappings)
      {
        mappings.Add(new IntegrationConfigData.FieldMapping("ows_" + mapping.SharepointField, mapping.SitecoreField));
      }

      data.FieldMappings = mappings;
      data.Web = this.SharepointWeb;

      data.View = this.ViewName;

      if (this.Credentials != null)
      {
        data.SetCredentials(this.userName.Value, this.pwd.Value);
      }
      else
      {
        data.SetCredentials(null, null);
      }

      data.ScheduledBlobTransfer = this.IsScheduledBlobTransferEnabled;
      data.BidirectionalLink = this.IsSharepointEditable;
      data.ConnectionConfiguration = this.GetConnectionConfiguration();

      IntegrationConfigDataProvider.SaveToItem(data, integrationConfigItem);

      using (new EditContext(integrationConfigItem))
      {
        integrationConfigItem[FieldIDs.IsIntegrationItem] = "1";
      }
    }

    private string GetConnectionConfiguration()
    {
      if (Context.ClientPage.ClientRequest.Form["Creds_Creds"] == "alt" && this.isAdvanced.Checked)
      {
        return this.configuration.SelectedItem.Value;
      }

      return null;
    }

    /// <summary>
    /// Creates the integration configuration item.
    /// </summary>
    /// <returns>The integration configuration item.</returns>
    [NotNull]
    protected Item CreateIntegrationConfigItem()
    {
      Item integrationConfigItem;
      if (string.IsNullOrEmpty(this.IntegrationConfigItemID))
      {
        integrationConfigItem = this.ParentItem.Add(this.IntegrationConfigItemName, new TemplateID(TemplateIDs.IntegrationConfig));
        this.IntegrationConfigItemID = integrationConfigItem.ID.ToString();
      }
      else
      {
        integrationConfigItem = this.Database.GetItem(this.IntegrationConfigItemID);
      }

      if (this.IsLibrary())
      {
        AddBranchToItem(integrationConfigItem, uploadFileBranchID);
        AddBranchToItem(integrationConfigItem, uploadFileBranchAdvancedID);
        AddBranchToItem(integrationConfigItem, TemplateIDs.IntegrationFolder);
      }

      return integrationConfigItem;
    }

    private bool IsLibrary()
    {
      return this.SpInfo[this.ListName].ListType == ListType.Library;
    }

    /// <summary>
    /// Add branch to the Insert Options.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <param name="branchID">
    /// The branch id.
    /// </param>
    private static void AddBranchToItem(Item item, ID branchID)
    {
      var str = new ListString(item[Sitecore.FieldIDs.Branches]);
      if (!str.Contains(branchID.ToString()))
      {
        str.Add(branchID.ToString());
        item.Editing.BeginEdit();
        item[Sitecore.FieldIDs.Branches] = str.ToString();
        item.Editing.EndEdit();
      }
    }



    /// <summary>
    /// Ends the wizard.
    /// </summary>
    protected override void EndWizard()
    {
      if (this.Active == "LastPage")
      {
        ProcessIntegrationItemsOptions options = ProcessIntegrationItemsOptions.DefaultOptions;
        options.Recursive = true;

        Item integrationConfigItem = this.Database.GetItem(this.IntegrationConfigItemID);

        if (this.waitSyncIntegration.Checked)
        {
          SharepointProvider.ProcessTree(integrationConfigItem, options);
        }
        else
        {
          options.AsyncIntegration = true;
          SharepointProvider.ProcessTree(integrationConfigItem, options);
          System.Threading.Thread.Sleep(3000);
        }

        Context.ClientPage.ClientResponse.SetDialogValue(this.IntegrationConfigItemID);
      }

      base.EndWizard();
    }

    /// <summary>
    /// Raises the load event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    /// <remarks>
    /// This method notifies the server control that it should perform actions common to each HTTP
    /// request for the page it is associated with, such as setting up a database query. At this
    /// stage in the page lifecycle, server controls in the hierarchy are created and initialized,
    /// view state is restored, and form controls reflect client-side data. Use the IsPostBack
    /// property to determine whether the page is being loaded in response to a client postback,
    /// or if it is being loaded and accessed for the first time.
    /// </remarks>
    protected override void OnLoad([NotNull] EventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");
      License.DemandModule("Sitecore.Sharepoint");
      base.OnLoad(e);
      this.alternativeCredentials.Attributes["value"] = "alt";
      this.defaultCredentials.Attributes["value"] = "def";
      this.createNewMapping.Attributes["value"] = "new";
      this.useExistentMapping.Attributes["value"] = "exist";

      if (!Context.ClientPage.IsEvent && !Context.ClientPage.IsPostBack)
      {
        foreach (var connectionConfiguration in Common.Configuration.Settings.ConnectionConfigurations.Values)
        {
          this.configuration.Controls.Add(new ListItem
          {
            Header = connectionConfiguration.DisplayName,
            Value = connectionConfiguration.Name
          });
        }

        this.currentUser.Text = this.defaultUser.Value = this.GetDefaultUserName();
        this.currentConfiguration.Text = this.defaultConfiguration.Value = this.GetDefaultConfiguratorDisplayName();
      }
    }

    protected void listSelector_OnChange()
    {
      itemName.Value = string.Empty;
      this.Template = this.IntegrationConfigItemName;
    }

    protected void MappingSelectionChanged()
    {
      IntegrationConfigData data = this.SelectedExistentMapping;
      this.FillMappingInfo(data);
      Context.ClientPage.ClientResponse.Refresh(this.mappingInfoPanel);
      Context.ClientPage.ClientResponse.Eval("window.updateInfoPanel()");
    }

    /// <summary>
    /// Creates the template for reflections.
    /// </summary>
    /// <returns>The ID of template.</returns>
    [NotNull]
    protected ID CreateTemplate()
    {
      if (this.IsLibrary())
      {
        return ID.Null;
      }

      Database db = Database;
      Item sharepointTemplatesRoot = db.GetItem(TemplateIDs.IntegrationItemTemplatesRoot);
      List<IntegrationConfigData.FieldMapping> newFields = this.GetAddedMappings();
      Assert.IsNotNull(sharepointTemplatesRoot, "Can't find root folder to generate template it");
      if (!UseAlreadyExistentMapping || newFields.Count > 0)
      {
        IntegrationConfigData selectedIntegrationData = this.SelectedExistentMapping;
        TemplateItem template = db.Templates.CreateTemplate(this.Template, sharepointTemplatesRoot);
        TemplateSectionItem section = template.AddSection(UIMessages.SharePointData);
        if (selectedIntegrationData != null && newFields.Count > 0)
        {
          // need to create a new template but inherites it from selected template and add only newly added fields
          foreach (IntegrationConfigData.FieldMapping mapping in newFields)
          {
            this.AddTextField(section, mapping.Target);
          }
          using (new EditContext(template.InnerItem))
          {
            template.InnerItem[Sitecore.FieldIDs.BaseTemplate] = selectedIntegrationData.TemplateID;
          }
        }
        else
        {
          foreach (MappingEntryControl mapping in this.Mappings)
          {
            this.AddTextField(section, mapping.SitecoreField);
          }
          using (new EditContext(template.InnerItem))
          {
            template.InnerItem[Sitecore.FieldIDs.BaseTemplate] = TemplateIDs.IntegrationBase.ToString();
          }
        }
        return template.ID;
      }
      // user selected existent mapping and didn't add any new field
      return ID.Parse(SelectedExistentMapping.TemplateID);
    }

    private void AddTextField([NotNull] TemplateSectionItem section, [NotNull] string fieldName)
    {
      Assert.ArgumentNotNull(section, "section");
      Assert.ArgumentNotNullOrEmpty(fieldName, "fieldName");

      TemplateFieldItem templateItem = section.AddField(fieldName);
      templateItem.BeginEdit();
      templateItem.Type = "Single-Line Text";
      templateItem.EndEdit();
    }

    protected void LoadDataAsync()
    {
      object[] parameters = new object[2];

      // parameters items can't be null as Sitecore tryies to uderstand the type of each element
      parameters[0] = Credentials ?? CredentialCache.DefaultNetworkCredentials;
      parameters[1] = this.GetConnectionConfiguration() ?? string.Empty;

      JobOptions options = new JobOptions("Retrieve data from SharePoint", "SharePoint Integration", Client.Site.Name, this, "LoadSharepointDataAsync", parameters)
      {
        ContextUser = Context.User
      };
      Job job = JobManager.Start(options);
      Context.ClientPage.ServerProperties["handle"] = job.Handle;
      Context.ClientPage.ClientResponse.Timer("spintegration:CheckStatus(handle=" + job.Handle + ")", 500);
    }

    /// <summary>
    /// Goto the page and disable all buttons.
    /// </summary>
    /// <param name="pageName">Name of the page.</param>
    protected void GotoPage([NotNull] string pageName)
    {
      Assert.ArgumentNotNull(pageName, "pageName");

      Active = pageName;
      NextButton.Disabled = false;
      BackButton.Disabled = false;
      CancelButton.Disabled = false;
    }

    protected void ClearMappings()
    {
      Mappings.ForEach(mapping => mappingContainer.Controls.Remove(mapping));
    }

    #region Message handlers

    /// <summary>
    /// Checks the status of connecting to SharePoint
    /// </summary>
    /// <param name="message">The message.</param>
    [HandleMessage("spintegration:CheckStatus")]
    protected void CheckStatus([NotNull] Message message)
    {
      Assert.ArgumentNotNull(message, "message");

      string str = message.Arguments["handle"];
      Assert.IsNotNullOrEmpty(str, "handle");
      Job job = JobManager.GetJob(Handle.Parse(str));
      if (job == null)
      {
        this.GotoPage("ConnectToSharePoint");
        this.ShowError(Translate.Text(UIMessages.TalkingToSharePointFinishedUnexpectedly), string.Empty);
      }
      else if (job.Status.Failed)
      {
        StringBuilder errorText = new StringBuilder(Translate.Text(UIMessages.TalkingToSharePointFinishedWithErrors));
        errorText.Append("\n");
        if (job.Status.Messages.Count > 1)
        {
          // Need to remove the first and the last messages as they are useless
          for (int i = 1; i < job.Status.Messages.Count - 1; i++)
          {
            errorText.AppendFormat("{0}\n", job.Status.Messages[i]);
          }
        }
        this.ShowError(errorText.ToString(), string.Empty);
        this.GotoPage("ConnectToSharePoint");
      }
      else if (job.IsDone)
      {
        SpInfo = job.Options.CustomData as Dictionary<string, ListInfo>;
        this.GotoPage("ListSelectorPage");
      }
      else
      {
        Context.ClientPage.ClientResponse.Timer("spintegration:CheckStatus(handle=" + job.Handle + ")", 500);
      }


    }

    /// <summary>
    /// Tests the connection.
    /// </summary>
    /// <param name="args">The arguments.</param>
    [HandleMessage("spintegration:testconnection", true)]
    protected void TestConnection([NotNull] ClientPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      if (this.LoadSharepointData())
      {
        Context.ClientPage.ClientResponse.Alert(Translate.Text(UIMessages.TestConnectionOK));
      }
      else
      {
        Context.ClientPage.ClientResponse.Alert(Translate.Text(UIMessages.TestConnectionFailedCheckYourLoginDetailsAreCorrect));
      }
    }

    /// <summary>
    /// Removes the mapping.
    /// </summary>
    /// <param name="args">message that contains additional information (like id of mapping to remove)</param>
    [HandleMessage("spintegration:removemapping")]
    protected void RemoveMapping([NotNull] Message args)
    {
      Assert.ArgumentNotNull(args, "args");

      for (int i = 0; i < mappingContainer.Controls.Count; i++)
      {
        System.Web.UI.Control control = mappingContainer.Controls[i];
        if (control is MappingEntryControl)
        {
          if ((control as MappingEntryControl).Key == args.Arguments["key"])
          {
            mappingContainer.Controls.Remove(control);
            Context.ClientPage.ClientResponse.Remove(args.Arguments["key"]);
            break;
          }
        }
      }
    }

    /// <summary>
    /// Adds the mapping.
    /// </summary>
    /// <param name="args">The message with additional info</param>
    [HandleMessage("spintegration:addmapping")]
    protected void AddMapping([NotNull] Message args)
    {
      Assert.ArgumentNotNull(args, "args");

      MappingEntryControl mapping = ControlFactory.GetControl("Sharepoint.MappingEntry") as MappingEntryControl;
      if (mapping != null)
      {
        mapping.Key = Control.GetUniqueID("me");
        mapping.AvailableFields = this.AvailableFields(this.ListName, this.ViewName);
        Context.ClientPage.AddControl(this.mappingContainer, mapping);
      }

      Context.ClientPage.ClientResponse.Refresh(this.mappingContainer);
    }
    #endregion

    #region Connect to SharePoint facilities

    protected bool LoadSharepointData()
    {
      try
      {
        SpInfo = this.DoLoadSharepointData(Credentials, this.GetConnectionConfiguration());
      }
      catch (Exception e)
      {
        Log.Error("SharePoint integration wizard error", e, this);
        return false;
      }
      return true;
    }

    /// <summary>
    /// Loads the sharepoint data async.
    /// </summary>
    /// <param name="credentials">The credentials to be used to connect to SharePoint</param>
    protected void LoadSharepointDataAsync([NotNull] ICredentials credentials, string connectionConfiguration = null)
    {
      Assert.ArgumentNotNull(credentials, "credentials");

      Job job = Context.Job;
      try
      {
        Assert.IsNotNull(job, "Sharepoint retrieving job is null");
        if (credentials == CredentialCache.DefaultCredentials)
        {
          // Can't pass DefaultCredentials as it means that current user will be used to access to Sharepoint
          credentials = null;
        }

        job.Options.CustomData = this.DoLoadSharepointData(credentials, connectionConfiguration);
      }
      catch (Exception e)
      {
        Job.AddMessage(e.Message);
        job.Status.Failed = true;
      }
    }

    protected Dictionary<string, ListInfo> DoLoadSharepointData(ICredentials credentials, string connectionConfiguration = null)
    {
      Dictionary<string, ListInfo> info = new Dictionary<string, ListInfo>();
      Uri serverUrl = new Uri(this.server.Value);
      var context = SpContextProviderBase.Instance.CreateDataContext(this.SharepointServer, this.SharepointWeb, credentials, connectionConfiguration);
      Server spServer = Server.Connect(context);
      Web web = spServer.GetWeb(SharepointWeb);
      if (web != null)
      {
        string message = string.Format("Web {0} has been retrieved from {1}", web.Name, serverUrl);
        Log.Debug(message);
      }
      else
      {
        string message = string.Format("Web can't be retrieved from {1}", serverUrl);
        Log.Debug(message);
      }

      if (web != null)
      {
        foreach (BaseList list in web.Lists)
        {
          if (list["Hidden"].ToLower() == "true")
          {
            Log.Debug(string.Format("List {0} has not been added to collection as it's hidden", list.Name));
            continue;
          }

          Log.Debug(string.Format("List {0} added to collection", list.Name));
          var listinfo = new ListInfo()
          {
            ListName = list.Name,
            ListType = (list is Library) ? ListType.Library : ListType.List,
            Views = new Dictionary<string, KeyValuePair<string, List<string>>>()
          };
          info.Add(list.ID, listinfo);
          Sharepoint.ObjectModel.Entities.Collections.ViewCollection listViews = list.Views;
          List<View> localViews = listViews.ToList();
          localViews.RemoveAll(view => view.Hidden);
          localViews.RemoveAll(view => MainUtil.GetBool(view["RequiresClientIntegration"], false));
          localViews.RemoveAll(view => view.Type != "HTML");

          foreach (View view in localViews)
          {
            Log.Debug(string.Format("   View {0} added to collection for list {1}", view.DisplayName, list.Name));
            List<string> fields = new List<string>();
            listinfo.Views.Add(view.Name, new KeyValuePair<string, List<string>>(view.DisplayName, fields));
            foreach (string fieldName in view.FieldNames)
            {
              Log.Debug(string.Format("      Field {0} added to collection for view {1}", fieldName, view.DisplayName));
              fields.Add(fieldName);
            }
          }
        }
      }

      return info;
    }

    protected Dictionary<string, ListInfo> SpInfo
    {
      get
      {
        if (savedSpInfo == null)
        {
          savedSpInfo = ServerProperties["SpInfo"] as Dictionary<string, ListInfo>;
        }
        return savedSpInfo;
      }
      set
      {
        ServerProperties["SpInfo"] = value;
      }
    }
    protected List<KeyValuePair<string, string>> AvailableViews(string listName)
    {
      List<KeyValuePair<string, string>> views = new List<KeyValuePair<string, string>>();
      foreach (var keyValuePair in this.SpInfo[listName].Views)
      {

        views.Add(new KeyValuePair<string, string>(keyValuePair.Value.Key, keyValuePair.Key));
      }

      return views;
    }
    protected StringList AvailableFields(string listName, string viewName)
    {
      StringList fields = new StringList();
      fields.AddRange(this.SpInfo[listName].Views[viewName].Value);
      return fields;
    }

    #endregion

    #region Properties

    [NotNull]
    protected List<MappingEntryControl> Mappings
    {
      get
      {
        List<MappingEntryControl> mappings = new List<MappingEntryControl>();
        for (int i = mappingContainer.Controls.Count - 1; i >= 0; i--)
        {
          System.Web.UI.Control control = mappingContainer.Controls[i];
          if (control is MappingEntryControl)
          {
            mappings.Add(control as MappingEntryControl);
          }
        }
        return mappings;
      }
    }

    protected Web SelectedWeb
    {
      get
      {
        return ServerProperties["SelectedWeb"] as Web;
      }
      set
      {
        ServerProperties["SelectedWeb"] = value;
      }
    }

    protected string ListName
    {
      get
      {
        return listSelector.Value;
      }
    }

    protected string ListHeader
    {
      get
      {
        return listSelector.SelectedItem.Header;
      }
    }

    protected string ViewDisplayName
    {
      get
      {
        return (viewSelector.SelectedItem != null) ? viewSelector.SelectedItem.Header : null;
      }
    }

    protected string ViewName
    {
      get
      {
        return viewSelector.Value;
      }
    }

    /// <summary>
    /// Gets the full username that will be used to connect to SharePoint
    /// </summary>
    /// <value>The sharepoint user.</value>
    [NotNull]
    protected string SharepointUser
    {
      get
      {
        NetworkCredential localCredentials = this.Credentials;
        if (localCredentials == null)
        {
          ServerEntry entry = Sharepoint.Common.Configuration.Settings.PredefinedServerEntries.GetFirstEntry(this.SharepointServer + this.SharepointWeb, "Provider");
          if (entry != null)
          {
            NetworkCredential credentials = entry.Credentials as NetworkCredential;
            if (credentials != null)
            {
              return StringUtil.Combine(credentials.Domain, credentials.UserName, "\\");
            }
          }
          return Translate.Text(UIMessages.UnknownUser);
        }

        return StringUtil.Combine(localCredentials.Domain, localCredentials.UserName, "\\");
      }
    }

    protected string Template
    {
      get
      {
        return templateName.Value;
      }
      set
      {
        templateName.Value = value;
      }
    }

    protected bool IsScheduledBlobTransferEnabled
    {
      get
      {
        return this.scheduledBlobTransferEnabled.Checked;
      }
    }

    protected string ExpirationInterval
    {
      get
      {
        return expiration.Value;
      }
    }

    protected string SpItemLimit
    {
      get
      {
        return itemLimit.Value;
      }
    }

    protected bool IsSharepointEditable
    {
      get
      {
        return updateSharepoint.Checked;
      }
    }

    protected bool UseAlreadyExistentMapping
    {
      get
      {
        return Context.ClientPage.ClientRequest.Form["mappingsSelectionGroup_mappingsSelectionGroup"] == "exist";
      }
    }


    protected IntegrationConfigData SelectedExistentMapping
    {
      get
      {
        ID integrationConfigItemID;
        if (this.mappingSelector.SelectedItem != null && ID.TryParse(this.mappingSelector.SelectedItem.Value, out integrationConfigItemID))
        {
          Item integrationConfigItem = this.Database.GetItem(integrationConfigItemID);
          if (integrationConfigItem != null)
          {
            return IntegrationConfigDataProvider.GetFromItem(integrationConfigItem);
          }
        }
        return null;
      }
    }

    protected NetworkCredential Credentials
    {
      get
      {
        if (Context.ClientPage.ClientRequest.Form["Creds_Creds"] == "alt" && this.isCredentials.Checked)
        {
          string domain = StringUtil.GetPrefix(this.userName.Value, '\\');
          string shortUsername = StringUtil.GetPostfix(this.userName.Value, '\\', this.userName.Value);
          return new NetworkCredential(shortUsername, this.pwd.Value, domain);
        }

        return null;
      }
    }

    protected Database Database
    {
      get
      {
        if (database == null)
        {
          string databaseName = WebUtil.GetQueryString("database", "master");
          database = Factory.GetDatabase(databaseName);
        }
        return database;
      }
    }

    protected Item ParentItem
    {
      get
      {
        if (parentItem == null)
        {
          string parentID = WebUtil.GetQueryString("pid");
          parentItem = Database.GetItem(parentID);
        }
        return parentItem;
      }
    }

    protected string IntegrationConfigItemID
    {
      get
      {
        return this.integrationConfigItemIdHolder.Value;
      }
      set
      {
        this.integrationConfigItemIdHolder.Value = value;
      }
    }
    protected string IntegrationConfigItemName
    {
      get
      {
        if (string.IsNullOrEmpty(itemName.Value))
        {
          this.IntegrationConfigItemName = StringUtil.GetLastPostfix(this.SharepointWeb, '/', this.SharepointWeb) + "_" + this.ListHeader;
        }
        return itemName.Value;
      }
      set
      {
        itemName.Value = value;
      }
    }
    protected string SharepointServer
    {
      get
      {
        Uri serverUrl = new Uri(this.server.Value);
        return serverUrl.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
      }
    }
    protected string SharepointWeb
    {
      get
      {
        Uri serverUrl = new Uri(StringUtil.RemovePostfix('/', this.server.Value));
        return serverUrl.LocalPath;
      }
    }

    #endregion

    /// <summary>
    /// Fills the mapping info.
    /// </summary>
    /// <param name="data">Integration data to copy information from</param>
    protected void FillMappingInfo([NotNull] IntegrationConfigData data)
    {
      Assert.ArgumentNotNull(data, "data");

      this.mappingInfoPanel.Controls.Clear();
      if (data.FieldMappings.Count > 0)
      {

        this.mappingInfoPanel.Controls.Add(new Literal(UIMessages.SharePoint)
        {
          Class = "MappingHeader"
        });
        this.mappingInfoPanel.Controls.Add(new Literal(UIMessages.Sitecore)
        {
          Class = "MappingHeader"
        });

        foreach (IntegrationConfigData.FieldMapping mapping in data.FieldMappings)
        {
          var sourceField = new Literal(mapping.Source);
          this.mappingInfoPanel.Controls.Add(sourceField);

          var targetField = new Literal(mapping.Target);
          this.mappingInfoPanel.Controls.Add(targetField);
        }
      }
      else
      {
        this.mappingInfoPanel.Controls.Add(new Literal(UIMessages.TheTemplateDoesNotContainAnyMappings));
      }
    }

    /// <summary>
    /// This method returns a list with added mappings compared to the list selected on "Choose existent mappings" page
    /// You may think about this list as a list of mappings that need a new field in sitecore item.
    /// </summary>
    /// <returns></returns>
    [NotNull]
    protected List<IntegrationConfigData.FieldMapping> GetAddedMappings()
    {
      List<IntegrationConfigData.FieldMapping> result = new List<IntegrationConfigData.FieldMapping>();
      this.templateName.Enabled = true;
      if (this.UseAlreadyExistentMapping)
      {
        List<IntegrationConfigData.FieldMapping> existentMappings = this.SelectedExistentMapping.FieldMappings;
        bool customMapping = false;
        foreach (MappingEntryControl mapping in this.Mappings)
        {
          MappingEntryControl mappingControl = mapping;
          if (existentMappings.FirstOrDefault(existMap => existMap.Target == mappingControl.SitecoreField) == null)
          {
            result.Add(new IntegrationConfigData.FieldMapping(mapping.SharepointField, mapping.SitecoreField));
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Gets the existent mappings already registered in the system.
    /// </summary>
    /// <returns>Dictionary with pairs integrationConfigItemID, IntegrationConfigData</returns>
    [NotNull]
    protected Dictionary<ID, IntegrationConfigData> GetExistentMappings()
    {
      string query = string.Format("fast://*[@@templateid = '{0}']", TemplateIDs.IntegrationConfig);

      // Get all integration configuration items from database
      Item[] integrationConfigItems = Database.SelectItems(query);

      // Convert list of integration configuration items to list of IntegrationConfigData using XML stored in items
      var sysDataList = new Dictionary<ID, IntegrationConfigData>();

      foreach (Item integrationConfigItem in integrationConfigItems)
      {
        IntegrationConfigData integrationConfigData = IntegrationConfigDataProvider.GetFromItem(integrationConfigItem);
        if (integrationConfigData != null)
        {
          sysDataList.Add(integrationConfigItem.ID, integrationConfigData);
        }
      }

      Dictionary<ID, IntegrationConfigData> result = new Dictionary<ID, IntegrationConfigData>();

      // Filter list of all IntegrationConfigData by removing systemData-s pointed to the same TemplateID
      foreach (KeyValuePair<ID, IntegrationConfigData> systemData in sysDataList)
      {
        IntegrationConfigData localIntegrationData = systemData.Value;
        if (localIntegrationData != null && !result.Any(data => data.Value.TemplateID == localIntegrationData.TemplateID) && Database.GetItem(localIntegrationData.TemplateID) != null)
        {
          result.Add(systemData.Key, localIntegrationData);
        }
      }

      return result;
    }
  }
}
