// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharepointList.ascx.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines SharepointList User Control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Web
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Diagnostics;
  using System.Globalization;
  using System.Linq;
  using System.Net;
  using System.Web;
  using System.Web.Services.Protocols;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;
  using Sitecore.Globalization;
  using Sitecore.Resources;
  using Sitecore.Sharepoint.Common.Texts;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.ObjectModel;
  using Sitecore.Sharepoint.ObjectModel.Actions;
  using Sitecore.Sharepoint.ObjectModel.Entities;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;
  using Sitecore.Sharepoint.ObjectModel.Options;
  using Sitecore.Sharepoint.ObjectModel.Utils;
  using Sitecore.Sharepoint.Web.Extensions;
  using Sitecore.Text;

  using View = Sitecore.Sharepoint.ObjectModel.Entities.View;
  using UiItemCollection = Sitecore.Sharepoint.Web.Entities.Collections.ItemCollection;

  public partial class SharepointList : UserControl, IActionController
  {
    private string folderPathOld = "folder_path_old";

    private string spSortAscending = "sp_sort_ascending";

    private string spSortExpression = "sp_sort_expression";

    private string sharepointAttemptNumber = "Sharepoint_Attempt_Number";

    private string sharepointlcPaging = "sharepointlc_paging";

    private string server;
    private BaseList list;
    private View view;
    private bool listInitialized;

    public string Web { get; protected set; }
    public string ListName { get; protected set; }
    public bool ShowIfEmpty { get; set; }
    public SpContext context { get; protected set; }

    protected const string CurrentItemsKey = "CurrentItemsKey";
    private UiItemCollection currentItems;
    protected UiItemCollection CurrentItems
    {
      get
      {
        return this.currentItems ?? (this.currentItems = this.GetCurrentItemsFromViewState());
      }

      set
      {
        this.currentItems = value;
      }
    }

    /// <summary>
    /// This methom calling if data was changed.
    /// </summary>
    public void Rebind()
    {
      var options = new ItemsRetrievingOptions()
      {
        ViewName = this.View.Name,
        Folder = this.folderPath.Value,
        SortingInfo = new ItemsRetrievingOptions.SortingOptions { Ascending = this.SortAscending, FieldName = this.SortExpression }
      };

      this.CurrentItems = new UiItemCollection(this.context, this.List, options);
      this.spGridView.DataSource = this.CurrentItems;
      this.BuildGridView(this.List, this.View);
      this.spGridView.DataBind();
      this.CheckButtons();
    }

    /// <summary>
    /// This method calling if action is failed.
    /// </summary>
    /// <param name="e">
    /// The e     .
    /// </param>
    public void ShowError(Exception e)
    {
      var ex = e as WebException;
      if (ex != null)
      {
        var webResponse = ex.Response as HttpWebResponse;
        if (webResponse != null)
        {
          if (webResponse.StatusCode == HttpStatusCode.Unauthorized)
          {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "alert", string.Format("alert('{0}');", Translate.Text(UIMessages.YouDoNotHaveTheCorrectPermissionContactTheServerAdministrator)), true);
          }
        }
      }
    }

    protected void Initialize()
    {
      string queryString = Attributes["sc_parameters"];
      if (!string.IsNullOrEmpty(queryString))
      {
        NameValueCollection param = Sitecore.Web.WebUtil.ParseUrlParameters(queryString);
        if (string.IsNullOrEmpty(this.Web))
        {
            if (string.IsNullOrEmpty(param["Web"]) && string.IsNullOrEmpty(param["Server"]))
            {
                this.Web = SharepointUtils.CurrentWebPath;
            }
            else
            {
                this.Web = param["Web"];
            }
        }

        if (!string.IsNullOrEmpty(param["Server"]))
        {
          this.Server = param["Server"];
        }

        if (string.IsNullOrEmpty(this.ListName))
        {
          this.ListName = param["List"];
          this.ListName = this.ListName.Replace("%2b", " ");
          this.ListName = this.ListName.Replace("+", " ");
        }

        this.ShowIfEmpty = MainUtil.GetBool(param["ShowIfEmpty"], false);
      }

      this.context = SpContextProviderBase.Instance.CreateUIContext(this.Server, this.Web);
      NetworkCredential headerCredential = SharepointExtension.GetCredentialsFromHeader(Request);
      if (headerCredential != null)
      {
        this.context.Credentials = headerCredential;
      }
    }

    public string Server
    {
      get
      {
        if (string.IsNullOrEmpty(this.server))
        {
          server = SharepointUtils.CurrentSharepointServer;
        }
        return this.server;
      }

      set
      {
        this.server = value;
      }
    }

    public BaseList List
    {
      get
      {
        if (!this.listInitialized && !string.IsNullOrEmpty(this.ListName))
        {
          try
          {
            this.list = BaseList.GetList(this.Web, this.ListName, this.context);
          }
          catch (Exception e)
          {
            string message = String.Format("Can't get list from SharePoint [Server={0}, Web={1}, List={2}]", this.Server, this.Web, this.ListName);
            Sitecore.Diagnostics.Log.Error(message, e, this);
          }

          this.listInitialized = true;
        }

        return this.list;
      }
    }

    public View View
    {
      get
      {
        if (this.view == null && this.list != null)
        {
          this.view = View.GetView(this.List, this.spViewsList.SelectedValue, this.context);
        }

        return this.view;
      }
    }

    protected override void OnInit(EventArgs e)
    {
      base.OnInit(e);
      this.folderPathOld = this.ClientID + "_folder_path_old";
      this.spSortAscending = this.ClientID + "_sp_sort_ascending";
      this.spSortExpression = this.ClientID + "_sp_sort_expression";
      this.sharepointAttemptNumber = this.ClientID + "_Sharepoint_Attempt_Number";
      this.sharepointlcPaging = this.ClientID + "_sharepointlc_paging";
      this.spGridView.Attributes.Add("CellSpacing", "0");
    }

    protected override void OnPreRender(EventArgs e)
    {
      base.OnPreRender(e);

      this.SaveCurrentItemsToViewState();

      if (this.Visible)
      {
        ScriptManager.RegisterStartupScript(this.spGridView, this.GetType(), "hoverIntentScript_Init", "initHoverIntent('genericGrid_data');", true);
      }

      this.spGridView.Columns.Clear();
    }

    protected override void OnLoad(EventArgs e)
    {
      this.Initialize();
      base.OnLoad(e);
      if (!this.CheckCredentialsOnSharepointServer())
      {
        return;
      }

      if (List == null)
      {
        this.menubarDiv.Visible = false;
        this.controlButtonsDiv.Visible = false;
        this.dataDiv.Visible = false;
        this.labelErrorMessageDiv.Visible = true;
        this.labelErrorMessage.Text = string.IsNullOrEmpty(this.ListName) ? UIMessages.PleaseSelectASharePointListFromTheSPIFControlProperties : UIMessages.ConnectionToSharePointServerFailedPleaseCheckYourSharePointServerConfigurationDetails;
        return;
      }

      if (!IsPostBack)
      {
        this.SortExpression = string.Empty;
        this.SortAscending = true;
        string selectedView = null;

        var views = new List<View>();

        foreach (View view in List.Views)
        {
          if (!view.Hidden && !MainUtil.GetBool(view["RequiresClientIntegration"], false) && view.Type == "HTML")
          {
            views.Add(view);
            if (list["DefaultViewUrl"] == view.Url)
            {
              selectedView = view.Name;
            }
          }
        }

        this.spViewsList.DataSource = views;
        this.spViewsList.DataTextField = "DisplayName";
        this.spViewsList.DataValueField = "Name";
        this.spViewsList.DataBind();
        this.spViewsList.SelectedValue = selectedView;




        this.AttemptNumber = 0;
      }

      string folderPathValue = this.folderPath.Value;
      if ((this.FolderPath_Old != folderPathValue) || (this.CurrentItems == null))
      {
        var itemsOptions = new ItemsRetrievingOptions
        {
          Folder = folderPathValue,
          SortingInfo = new ItemsRetrievingOptions.SortingOptions { FieldName = this.SortExpression, Ascending = this.SortAscending },
          ViewName = this.View.Name
        };
        this.CurrentItems = new UiItemCollection(this.context, this.List, itemsOptions);

        this.FolderPath_Old = folderPathValue;
      }

      if (!this.IsPostBack && this.CurrentItems.Count == 0 && !this.ShowIfEmpty)
      {
        this.Visible = false;
      }

      this.spGridView.DataSource = this.CurrentItems;
      this.BuildGridView(this.List, this.View);

      if (this.spViewsList.UniqueID != this.Request.Form[System.Web.UI.Page.postEventSourceID])
      {
        this.spGridView.DataBind();
      }

      this.CheckButtons();

      this.LabelSharepointSite.Text = this.GetBreadcrumbValue(folderPathValue);
    }

    protected bool CheckCredentialsOnSharepointServer()
    {
      try
      {
        this.AttemptNumber = 0;
        this.HideErrorLabel();
      }
      catch (SoapException)
      {
        this.errorLbl.Text = string.Format("There is no '{0}' list in the '{1}' web.", this.ListName, this.Web);
        this.ShowErrorLabel();
        return false;
      }
      catch (WebException ex)
      {
        this.errorLbl.Text = Translate.Text(UIMessages.CouldntGetResponseFromSharepointServer);
        var webResponse = ex.Response as HttpWebResponse;
        if ((webResponse != null) && (webResponse.StatusCode == HttpStatusCode.Unauthorized) &&
            webResponse.Headers.AllKeys.Contains("WWW-Authenticate"))
        {
          this.errorLbl.Text = Translate.Text(UIMessages.YouDoNotHaveEnoughRights);
          if (this.AttemptNumber < 3)
          {
            SharepointExtension.WriteAuthenticationResponseBasic(this.Request, this.Response);
            this.AttemptNumber++;
          }
        }

        this.ShowErrorLabel();
        return false;
      }

      return true;
    }

    public string SharepointViewUrl
    {
      get
      {
        BaseList spList = List;
        string folderPathValue = this.FolderPath;
        View currentView = this.View;
        if (List == null && View == null)
        {
          return string.Empty;
        }


        //NEED REFACTORING

        var goToSpUrl = new UrlBuilder(string.Format("{0}{1}", StringUtil.EnsurePostfix('/', spList.WebUrl.ToString()), StringUtil.RemovePrefix('/', currentView.Url)));
        if (!string.IsNullOrEmpty(folderPathValue))
        {
          goToSpUrl.AddQueryString(
            string.Format(
              "{0}={1}{2}",
              "RootFolder",
              StringUtil.EnsurePostfix('/', StringUtil.EnsurePrefix('/', spList.RootFolder)),
              StringUtil.RemovePostfix('/', folderPathValue)));
        }

        return goToSpUrl.ToString();
      }
    }

    /// <summary>
    /// Shows the error label.
    /// </summary>
    private void ShowErrorLabel()
    {
      this.dataDiv.Visible = false;
      this.spViewsList.Visible = false;
      this.errorLbl.Visible = true;
    }

    /// <summary>
    /// Hides the error label.
    /// </summary>
    private void HideErrorLabel()
    {
      this.dataDiv.Visible = true;
      this.spViewsList.Visible = true;
      this.errorLbl.Visible = false;
    }

    /// <summary>
    /// Gets or sets the attempt number.
    /// </summary>
    /// <value>The attempt number.</value>
    protected int AttemptNumber
    {
      [DebuggerStepThrough]
      get
      {
        return MainUtil.GetInt(Session[this.sharepointAttemptNumber], 0);
      }

      [DebuggerStepThrough]
      set
      {
        Session[this.sharepointAttemptNumber] = value;
      }
    }

    protected string FolderPath_Old
    {
      get
      {
        return StringUtil.GetString(this.ViewState[this.folderPathOld], string.Empty);
      }

      set
      {
        this.ViewState[this.folderPathOld] = value;
      }
    }

    public string FolderPath
    {
      get
      {
        return this.folderPath.Value;
      }
    }

    private string GetBreadcrumbValue(string folderPathValue)
    {
      string webName = string.IsNullOrEmpty(this.Web) ? "default" : this.Web;
      string breadcrumbValue = string.Format("{0} > {1}", webName, this.ListName);
      if (!string.IsNullOrEmpty(folderPathValue))
      {
        string breadcrumbPath = string.Empty;
        breadcrumbValue =
          string.Format(
            "{0} > <a href=\"javascript:__doPostBack('{1}','')\" OnClick=\"javascript:setFolder('{2}','{4}'); return true;\">{3}</a>",
            webName,
            this.spGridView.ClientID,
            string.Empty,
            this.ListName,
            this.folderPath.ClientID);
        List<string> folderPathParts =
          folderPathValue.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        for (int i = 0; i < folderPathParts.Count; i++)
        {
          string folderName = folderPathParts[i];
          if (i < folderPathParts.Count - 1)
          {
            breadcrumbPath += folderName + "/";
            breadcrumbValue +=
              string.Format(
                " > <a href=\"javascript:__doPostBack('{0}','')\" OnClick=\"javascript:setFolder('{1}','{3}'); return true;\">{2}</a>",
                this.spGridView.ClientID,
                breadcrumbPath,
                folderName,
                this.folderPath.ClientID);
          }
          else
          {
            breadcrumbValue += string.Format(" > {0}", folderName);
          }
        }
      }

      return breadcrumbValue;
    }

    protected void spGridView_Sorting(object sender, GridViewSortEventArgs e)
    {
      if (this.SortExpression == e.SortExpression)
      {
        this.SortAscending = !this.SortAscending;
      }
      else
      {
        this.SortAscending = true;
      }
      this.SortExpression = e.SortExpression;
      var options = new ItemsRetrievingOptions()
      {
        ViewName = this.View.Name,
        SortingInfo = new ItemsRetrievingOptions.SortingOptions
        {
          FieldName = this.SortExpression,
          Ascending = this.SortAscending
        },
        Folder = this.folderPath.Value
      };

      this.CurrentItems = new UiItemCollection(this.context, this.List, options);
      this.spGridView.DataSource = this.CurrentItems;
      this.BuildGridView(this.List, this.View);
      this.spGridView.DataBind();
      this.CheckButtons();
    }

    protected void spViewsList_IndexChanged(object sender, EventArgs e)
    {
      this.folderPath.Value = string.Empty;
      this.SortAscending = true;
      this.SortExpression = string.Empty;
      this.LabelSharepointSite.Text = this.GetBreadcrumbValue(string.Empty);
      this.spGridView.AutoGenerateColumns = false;
      BaseList spList = BaseList.GetList(this.Web, this.ListName, this.context);
      View spView = View.GetView(spList, this.spViewsList.SelectedValue, this.context);
      var options = new ItemsRetrievingOptions { ViewName = spView.Name };
      this.CurrentItems = new UiItemCollection(this.context, spList, options);
      this.spGridView.DataSource = this.CurrentItems;
      this.BuildGridView(spList, spView);
      this.spGridView.DataBind();
      this.CheckButtons();
    }

    private void BuildGridView(BaseList spList, View spListView)
    {
      this.spGridView.Columns.Clear();
      foreach (string viewFieldName in spListView.FieldNames)
      {
        string fieldName = viewFieldName;
        Field viewField = spList.Fields.First(field => field.Name == fieldName);
        if (viewField != null)
        {
          ITemplate itemTemplate = this.GetItemFieldTemplate(viewField);
          TemplateField viewFieldColumn;
          if (!string.IsNullOrEmpty(viewField.HeaderImage))
          {
            viewFieldColumn = new TemplateField
              {
                ItemTemplate = itemTemplate,
                HeaderImageUrl = string.Format("{0}/_layouts/images/{1}", this.Server.Trim(new char[] { '/' }), viewField.HeaderImage),
                SortExpression = viewField.Name
              };
          }
          else
          {
            viewFieldColumn = new TemplateField
            {
              ItemTemplate = itemTemplate,
              HeaderText = viewField.DisplayName,
              SortExpression = viewField.Name
            };
          }

          this.spGridView.Columns.Add(viewFieldColumn);
        }
      }
    }

    /// <summary>
    /// Gets the item field template.
    /// </summary>
    /// <param name="viewField">The view field.</param>
    /// <returns></returns>
    protected virtual ITemplate GetItemFieldTemplate(Field viewField)
    {
      switch (viewField.Type)
      {
        case "Boolean":
        case "AllDayEvent":
          return new BooleanFieldValueTemplate(viewField);
        case "Number":
          return new NumberFieldValueTemplate(viewField);
        case "URL":
          return new LinkFieldValueTemplate(viewField);
        case "Currency":
          return new FieldValueTemplate(viewField, this.folderPath.ClientID, MainUtil.GetInt(viewField["LCID"], 1033), "{0:C}");
      }

      switch (viewField.Name)
      {
        case "FileSizeDisplay":
          return new FileSizeFieldValueTemplate(viewField, this.folderPath.ClientID);
      }

      return new FieldValueTemplate(viewField, this.folderPath.ClientID);
    }

    protected bool SortAscending
    {
      get
      {
        return MainUtil.GetBool(this.ViewState[this.spSortAscending], true);
      }

      set
      {
        this.ViewState[this.spSortAscending] = value;
      }
    }

    protected string SortExpression
    {
      get
      {
        return this.ViewState[this.spSortExpression] as string;
      }

      set
      {
        this.ViewState[this.spSortExpression] = value;
      }
    }

    protected void spPrevBtn_Click(object sender, EventArgs e)
    {
      this.CurrentItems.MoveToPreviousPage();
      this.spGridView.DataSource = this.CurrentItems;
      this.BuildGridView(this.List, this.View);
      this.spGridView.DataBind();
      this.CheckButtons();
    }

    protected void spNextBtn_Click(object sender, EventArgs e)
    {
      this.CurrentItems.MoveToNextPage();

      this.spGridView.DataSource = this.CurrentItems;
      this.BuildGridView(this.List, this.View);
      this.spGridView.DataBind();
      this.CheckButtons();
    }

    private void CheckButtons()
    {
      if (this.CurrentItems != null)
      {
        this.spPrevBtn.Enabled = this.CurrentItems.CanMoveToPreviousPage();
        this.spNextBtn.Enabled = this.CurrentItems.CanMoveToNextPage();
        if (this.CurrentItems.Count > 0)
        {
          this.lblItems.Text = string.Format("{0} - {1}", this.View.PageSize * (this.CurrentItems.CurrentPage - 1) + 1, this.View.PageSize * (this.CurrentItems.CurrentPage - 1) + this.CurrentItems.Count);
        }
        else
        {
          this.lblItems.Visible = false;
        }
      }
      else
      {
        this.spPrevBtn.Enabled = false;
        this.spNextBtn.Enabled = false;
        this.lblItems.Visible = false;
      }

      if (!this.spPrevBtn.Enabled)
      {
        this.spPrevBtn.CssClass = "disable";
      }
      else
      {
        this.spPrevBtn.CssClass = String.Empty;
      }

      if (!this.spNextBtn.Enabled)
      {
        this.spNextBtn.CssClass = "disable";
      }
      else
      {
        this.spNextBtn.CssClass = String.Empty;
      }
    }

    /// <summary>
    /// Handles the New entry BTN_ click event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
    protected void NewEntryBtn_Click(object sender, EventArgs e)
    {
      Sitecore.Web.WebUtil.Redirect(this.GetNewFormUrl(), true);
    }

    /// <summary>
    /// Gets the new form URL.
    /// </summary>
    /// <returns>The new form url.</returns>
    protected virtual string GetNewFormUrl()
    {
      string webName = this.Web;
      string listName = this.ListName;
      string serverUrl = this.Server;
      string folder = this.folderPath.Value;
      BaseList list = BaseList.GetList(webName, listName, this.context);
      string listRootFolder = list.RootFolder;
      string newFormUrl;
      if (list is ObjectModel.Entities.Lists.Library)
      {
        if (!string.IsNullOrEmpty(folder))
        {
          listRootFolder = string.Format("{0}{1}", StringUtil.EnsurePostfix('/', listRootFolder), StringUtil.RemovePrefix('/', folder));
        }
        listRootFolder = listRootFolder.TrimEnd(new char[] { '/' });
        listRootFolder = HttpUtility.UrlEncode(listRootFolder);
        newFormUrl = string.Format(
                                   "{0}{1}_layouts/Upload.aspx?List={2}&RootFolder={3}",
                                   StringUtil.EnsurePostfix('/', serverUrl),
                                   StringUtil.RemovePrefix('/', StringUtil.EnsurePostfix('/', webName)),
                                    HttpUtility.UrlEncode(list.ID),
                                    listRootFolder);
      }
      else
      {
        newFormUrl = string.Format(
          "{0}{1}NewForm.aspx?RootFolder={2}",
          StringUtil.EnsurePostfix('/', serverUrl),
          StringUtil.EnsurePostfix('/', StringUtil.RemovePrefix('/', listRootFolder)),
          HttpUtility.UrlPathEncode(listRootFolder));
      }

      newFormUrl = string.Format(
        "{0}&Source={1}", newFormUrl, HttpUtility.UrlEncode(string.Concat(Sitecore.Web.WebUtil.GetServerUrl(), Sitecore.Web.WebUtil.GetRawUrl())));
      return newFormUrl;
    }

    /// <summary>
    /// Handles the RowDataBound event of the spGridView control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
    protected void spGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
      if (e.Row.Cells.Count > 0)
      {
        e.Row.Cells[0].CssClass = "genericGrid_firstCell";
        e.Row.Cells[e.Row.Cells.Count - 1].CssClass = "genericGrid_lastCell";
      }

      if ((e.Row.RowType == DataControlRowType.DataRow) && (e.Row.Cells.Count > 0))
      {
        var dataItem = e.Row.DataItem as BaseItem;
        if ((dataItem != null) && (dataItem.GetActions().Count() > 0))
        {
          TableCell lastCell = e.Row.Cells[e.Row.Cells.Count - 1];
          var actionsSpanOut = new HtmlGenericControl("span");
          lastCell.Controls.Add(actionsSpanOut);
          actionsSpanOut.Attributes["class"] = "actions";

          var actionDiv = new HtmlGenericControl("div");
          actionDiv.InnerText = "Actions";
          actionDiv.Attributes.Add("onclick", "showHidePopUp(this);");
          actionDiv.Attributes.Add("class", "action_div");
          actionsSpanOut.Controls.Add(actionDiv);

          var actionsSpan = new HtmlGenericControl("span");
          actionsSpan.Attributes.Add("class", "action_popupspan");

          actionsSpanOut.Controls.Add(actionsSpan);
          foreach (ObjectModel.Actions.Action action in dataItem.GetActions())
          {
            var tempSpan = new HtmlGenericControl("div");
            var iControlable = action as IControlable;
            if (iControlable != null)
            {
              iControlable.ActionController = this;
            }

            tempSpan.Controls.Add(action.GetControl());
            actionsSpan.Controls.Add(tempSpan);
          }
        }
      }
    }

    /// <summary>
    /// Value template 
    /// </summary>
    public class FieldValueTemplate : ITemplate
    {
      private readonly string fieldName;
      private readonly Field field;

      private string folderPath;

      private readonly CultureInfo cultureInfo = CultureInfo.CurrentCulture;
      private readonly string format = "{0}";

      /// <summary>
      /// Gets the name of the field.
      /// </summary>
      /// <value>The name of the field.</value>
      public string FieldName
      {
        get
        {
          return this.fieldName;
        }
      }

      /// <summary>
      /// Gets the field.
      /// </summary>
      /// <value>The field.</value>
      protected Field Field
      {
        get
        {
          return this.field;
        }
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="FieldValueTemplate"/> class.
      /// </summary>
      /// <param name="field">The field.</param>
      public FieldValueTemplate(Field field, string folderPathClientId, int LCID, string Format)
        : this(field, folderPathClientId)
      {
        this.cultureInfo = new CultureInfo(LCID);
        this.format = Format;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="FieldValueTemplate"/> class.
      /// </summary>
      /// <param name="field">The field.</param>
      public FieldValueTemplate(Field field, string folderPathClientId)
      {
        this.folderPath = folderPathClientId;
        this.field = field;
        if (!field.Name.StartsWith("ows_"))
        {
          this.fieldName = "ows_" + field.Name;
        }
        else
        {
          this.fieldName = field.Name;
        }
      }

      /// <summary>
      /// When implemented by a class, defines the <see cref="T:System.Web.UI.Control"/> object that child controls and templates belong to. These child controls are in turn defined within an inline template.
      /// </summary>
      /// <param name="container">The <see cref="T:System.Web.UI.Control"/> object to contain the instances of controls from the inline template. </param>
      public virtual void InstantiateIn(Control container)
      {
        Control dataLabel = new Label();
        switch (this.fieldName)
        {
          case "ows_LinkFilename":
          case "ows_LinkTitle":
            dataLabel = new LinkButton();
            break;
          case "ows_DocIcon":
            dataLabel = new LinkButton() { CssClass = "overlayLink" };
            break;
        }

        dataLabel.DataBinding += this.dataLabel_DataBinding;
        container.Controls.Add(dataLabel);
      }

      /// <summary>
      /// Handles the DataBinding event of the dataLabel control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
      private void dataLabel_DataBinding(object sender, EventArgs e)
      {
        var target = (Control)sender;
        Control container = target.NamingContainer;
        var dataItem = DataBinder.GetDataItem(container) as BaseItem;
        if (dataItem != null)
        {
          object fieldValue;

          switch (this.Field.Type)
          {
            case "Currency":
              fieldValue = String.IsNullOrEmpty(dataItem[this.fieldName]) ? (object)dataItem[this.fieldName] : MainUtil.GetFloat(dataItem[this.fieldName], 0);
              break;
            default:
              fieldValue = dataItem[this.fieldName];
              break;
          }

          string serverUrl = StringUtil.EnsurePostfix('/', Sitecore.Web.WebUtil.GetServerUrl(dataItem.WebUrl, false));
          switch (this.fieldName)
          {
            case "ows_LinkFilename":
            case "ows_LinkTitle":
              var linkButtonControl = (LinkButton)sender;
              linkButtonControl.Text = (string)fieldValue;
              Control parentCell = linkButtonControl.Parent;
              while (!(parentCell is TableCell) && parentCell.Parent != null)
              {
                parentCell = parentCell.Parent;
              }

              if (parentCell is TableCell)
              {
                (parentCell as TableCell).CssClass = "NameCell";
              }
              if (!IsFolderItem(dataItem))
              {
                ClientAction defaultAction = dataItem.GetActions().GetDefault() as ClientAction ??
                                       dataItem.GetActions()["open"] as ClientAction;
                if (defaultAction != null)
                {
                  linkButtonControl.OnClientClick = defaultAction.Script;
                }
              }
              else
              {
                linkButtonControl.OnClientClick = string.Format("goToFolder('{0}','{1}');return true;", dataItem.Title, folderPath);
              }
              break;
            case "ows_DocIcon":
              var linkButton = (LinkButton)sender;
              string imageUrl = string.Empty;
              var img = new Image();
              img.Style.Add(HtmlTextWriterStyle.Margin, "1px");
              linkButton.Controls.Add(img);

              if (!IsFolderItem(dataItem))
              {
                if (dataItem is DocumentItem)
                {
                  imageUrl = ((DocumentItem)dataItem).Thumbnail;
                  if (((DocumentItem)dataItem).IsCheckedOut)
                  {
                    linkButton.Controls.Add(new Image()
                    {
                      CssClass = "overlayForIcon",
                      ImageUrl = "/~/icon/Applications/16x16/nav_up_left_green.png.aspx",
                      Width = Unit.Pixel(8),
                      Height = Unit.Pixel(8)
                    });
                  }
                }
                ClientAction defaultAction = dataItem.GetActions().GetDefault() as ClientAction ??
                                       dataItem.GetActions()["open"] as ClientAction;
                if (defaultAction != null)
                {
                  linkButton.OnClientClick = defaultAction.Script;
                }
              }
              else
              {
                linkButton.OnClientClick = string.Format("goToFolder('{0}','{1}');return true;", dataItem.Title, folderPath);
                imageUrl = ((FolderItem)dataItem).Thumbnail;
              }

              if (string.IsNullOrEmpty(imageUrl))
              {
                imageUrl = Images.GetThemedImageSource("Applications/16x16/document_plain.png");
              }

              img.ImageUrl = imageUrl;
              break;
            default:
              var labelControl = (Label)sender;
              if (!string.IsNullOrEmpty(Field.DisplayImage))
              {
                bool boolFieldValue = MainUtil.GetBool(fieldValue, false);
                if (boolFieldValue)
                {
                  labelControl.Controls.Add(
                    new Image
                      {
                        ImageUrl =
                          string.Format(
                          "{0}/_layouts/images/{1}", serverUrl.Trim(new char[] { '/' }), Field.DisplayImage),
                      });
                }
                else
                {
                  labelControl.Text = "&nbsp;";
                }
              }
              else
              {
                labelControl.Text = (fieldValue is string && String.IsNullOrEmpty((string)fieldValue)) ? "&nbsp;" : string.Format(this.cultureInfo, this.format, fieldValue);
              }
              break;
          }
        }
        else
        {
          target.Parent.Controls.Add(new Literal()
          {
            Text = "&nbsp;"
          });
        }
      }

      /// <summary>
      /// Determines whether [is folder item] [the specified item].
      /// </summary>
      /// <param name="item">The item.</param>
      /// <returns>
      ///   <c>true</c> if [is folder item] [the specified item]; otherwise, <c>false</c>.
      /// </returns>
      protected static bool IsFolderItem(BaseItem item)
      {
        // return string.IsNullOrEmpty(item.Properties["ows_DocIcon"]);
        return item is FolderItem;
      }
    }

    /// <summary>
    /// Boolean field value template.
    /// </summary>
    public class BooleanFieldValueTemplate : FieldValueTemplate
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="BooleanFieldValueTemplate"/> class.
      /// </summary>
      /// <param name="field">The field.</param>
      public BooleanFieldValueTemplate(Field field)
        : base(field, string.Empty)
      {
      }

      /// <summary>
      /// When implemented by a class, defines the <see cref="T:System.Web.UI.Control"/> object that child controls and templates belong to. These child controls are in turn defined within an inline template.
      /// </summary>
      /// <param name="container">The <see cref="T:System.Web.UI.Control"/> object to contain the instances of controls from the inline template.</param>
      public override void InstantiateIn(Control container)
      {
        var literal = new Literal();
        literal.DataBinding += this.checkBox_DataBinding;
        container.Controls.Add(literal);
      }

      void checkBox_DataBinding(object sender, EventArgs e)
      {
        var target = (Literal)sender;
        Control container = target.NamingContainer;
        var dataItem = DataBinder.GetDataItem(container) as BaseItem;
        if (dataItem != null)
        {
          bool fieldValue = MainUtil.GetBool(dataItem[this.FieldName], false);
          target.Text = fieldValue ? Translate.Text(UIMessages.Yes) : string.Empty;
        }
      }
    }

    /// <summary>
    /// Number field value template.
    /// </summary>
    public class NumberFieldValueTemplate : FieldValueTemplate
    {
      private readonly bool percentage;

      /// <summary>
      /// Gets a value indicating whether this <see cref="NumberFieldValueTemplate"/> is percentage.
      /// </summary>
      /// <value><c>true</c> if percentage; otherwise, <c>false</c>.</value>
      public bool Percentage
      {
        get
        {
          return this.percentage;
        }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="NumberFieldValueTemplate"/> class.
      /// </summary>
      /// <param name="viewField">The view field.</param>
      public NumberFieldValueTemplate(Field viewField)
        : base(viewField, string.Empty)
      {
        this.percentage = MainUtil.GetBool(viewField["Percentage"], false);
      }

      /// <summary>
      /// When implemented by a class, defines the <see cref="T:System.Web.UI.Control"/> object that child controls and templates belong to. These child controls are in turn defined within an inline template.
      /// </summary>
      /// <param name="container">The <see cref="T:System.Web.UI.Control"/> object to contain the instances of controls from the inline template.</param>
      public override void InstantiateIn(Control container)
      {
        if (this.Percentage)
        {
          var literalControl = new Literal();
          literalControl.DataBinding += this.literalControl_DataBinding;
          container.Controls.Add(literalControl);
          return;
        }

        base.InstantiateIn(container);
      }

      /// <summary>
      /// Handles the DataBinding event of the literalControl control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
      private void literalControl_DataBinding(object sender, EventArgs e)
      {
        var target = (Literal)sender;
        Control container = target.NamingContainer;
        var dataItem = DataBinder.GetDataItem(container) as BaseItem;
        if (dataItem != null)
        {
          float fieldValue = MainUtil.GetFloat(dataItem[this.FieldName], 0);
          target.Text = fieldValue.ToString("P");
        }
      }
    }

    /// <summary>
    /// Link field value template.
    /// </summary>
    public class LinkFieldValueTemplate : FieldValueTemplate
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="NumberFieldValueTemplate"/> class.
      /// </summary>
      /// <param name="viewField">The view field.</param>
      public LinkFieldValueTemplate(Field viewField)
        : base(viewField, string.Empty)
      {
      }

      /// <summary>
      /// When implemented by a class, defines the <see cref="T:System.Web.UI.Control"/> object that child controls and templates belong to. These child controls are in turn defined within an inline template.
      /// </summary>
      /// <param name="container">The <see cref="T:System.Web.UI.Control"/> object to contain the instances of controls from the inline template.</param>
      public override void InstantiateIn(Control container)
      {
        var link = new HyperLink();
        link.DataBinding += this.linkControl_DataBinding;
        container.Controls.Add(link);
        return;
      }

      /// <summary>
      /// Handles the DataBinding event of the literalControl control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
      private void linkControl_DataBinding(object sender, EventArgs e)
      {
        var target = (HyperLink)sender;
        Control container = target.NamingContainer;
        var dataItem = DataBinder.GetDataItem(container) as BaseItem;
        if (dataItem != null)
        {
          string[] values = dataItem[this.FieldName].Split(',');
          if (values.Length < 2)
          {
            target.Visible = false;
            return;
          }

          target.Visible = true;
          target.NavigateUrl = values[0];
          target.Target = "_blank";
          target.Text = (string.IsNullOrEmpty(values[1].Trim(' '))) ? values[0] : values[1];
        }
      }
    }

    public class FileSizeFieldValueTemplate : FieldValueTemplate
    {
      public FileSizeFieldValueTemplate(Field field, string folderPathClientId, int LCID, string Format) : base(field, folderPathClientId, LCID, Format)
      {
      }

      public FileSizeFieldValueTemplate(Field field, string folderPathClientId) : base(field, folderPathClientId)
      {
      }
      public override void InstantiateIn(Control container)
      {
        var literalControl = new Literal();
        literalControl.DataBinding += this.LiteralControlDataBinding;
        container.Controls.Add(literalControl);
        return;
      }

      private void LiteralControlDataBinding(object sender, EventArgs e)
      {
        var target = (Literal)sender;
        Control container = target.NamingContainer;
        var dataItem = DataBinder.GetDataItem(container) as BaseItem;
        if (dataItem != null)
        {
          long fileSize = MainUtil.GetLong(dataItem[this.FieldName], 0);
          if (dataItem is FolderItem)
          {
            target.Text = string.Empty;
          }
          else
          {
            target.Text = MainUtil.FormatSize(fileSize);
          }
        }
      }
    }

    protected void SaveCurrentItemsToViewState()
    {
      object serializationObject = null;
      if (this.CurrentItems != null)
      {
        serializationObject = this.CurrentItems.Serialize();
      }

      this.ViewState[CurrentItemsKey] = serializationObject;
    }

    protected UiItemCollection GetCurrentItemsFromViewState()
    {
      object serializationObject = this.ViewState[CurrentItemsKey];

      if (serializationObject == null)
      {
        return null;
      }

      return new UiItemCollection(this.context, this.List, (string)serializationObject);
    }
  }
}