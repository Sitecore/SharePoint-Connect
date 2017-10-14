namespace Sitecore.Sharepoint.Web
{
  using System;
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Web.UI;
  using Sitecore.Sharepoint.Common.Configuration;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;
  using Sitecore.Sharepoint.ObjectModel.Options;
  using Sitecore.Sharepoint.ObjectModel.Utils;
  using Sitecore.Web;
  using Sitecore.Sharepoint.ObjectModel;
  using System.Collections;

  public class SharepointListBase : UserControl
  {
    private string server;
    protected SpContext context;
    public string Web { get; set; }
    public string ListName { get; set; }
    public bool ShowIfEmpty { get; set; }

    protected override void OnLoad(EventArgs e)
    {
      string queryString = Attributes["sc_parameters"];
      if (!string.IsNullOrEmpty(queryString))
      {
        NameValueCollection param = WebUtil.ParseUrlParameters(queryString);

        if (!string.IsNullOrEmpty(param["Web"]))
        {
          this.Web = param["Web"];
        }

        if (string.IsNullOrEmpty(this.Web))
        {
          this.Web = SharepointUtils.CurrentWebPath;
        }

        if (!string.IsNullOrEmpty(param["Server"]))
        {
          this.Server = param["Server"];
        }

        if (string.IsNullOrEmpty(this.ListName))
        {
          this.ListName = param["List"];
        }

        this.ShowIfEmpty = MainUtil.GetBool(param["ShowIfEmpty"], false);
      }

      this.context = SpContextProviderBase.Instance.CreateUIContext(this.Server, this.Web);
    }

    public string Server
    {
      get
      {
        if (string.IsNullOrEmpty(server))
        {
          server = SharepointUtils.CurrentSharepointServer;
        }
        return server;
      }
      set
      {
        server = value;
      }
    }

    protected virtual BaseList LoadDataSource()
    {
      BaseList list = null;
      try
      {
        list = BaseList.GetList(this.Web, this.ListName, this.context);
      }
      catch (Exception e)
      {
        this.Visible = false;
      }
      return list;
    }
  }
  public class SharepointGridViewListBase : SharepointListBase
  {
    /// <summary>
    /// documentsList control.
    /// </summary>
    /// <remarks>
    /// Auto-generated field.
    /// To modify move field declaration from designer file to code-behind file.
    /// </remarks>
    protected global::System.Web.UI.WebControls.GridView documentsList;

    /// <summary>
    /// folderPath control.
    /// </summary>
    /// <remarks>
    /// Auto-generated field.
    /// To modify move field declaration from designer file to code-behind file.
    /// </remarks>
    protected global::System.Web.UI.WebControls.HiddenField folderPath;

    /// <summary>
    /// LabelSharepointSite control.
    /// </summary>
    /// <remarks>
    /// Auto-generated field.
    /// To modify move field declaration from designer file to code-behind file.
    /// </remarks>
    protected global::System.Web.UI.WebControls.Label LabelSharepointSite;

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (!this.Page.IsPostBack)
      {
        this.DataBind();
      }
    }

    protected override void OnDataBinding(EventArgs e)
    {
      base.OnDataBinding(e);
      ReloadData();
    }

    protected override void OnPreRender(EventArgs e)
    {
      base.OnPreRender(e);

      if (!ShowIfEmpty && !this.IsPostBack && (this.documentsList.DataSource == null || !((IEnumerable)this.documentsList.DataSource).GetEnumerator().MoveNext()))
      {
        this.Visible = false;
      }
    }

    public virtual void ReloadData()
    {
      string folderPathValue = this.folderPath.Value;
      BaseList list = LoadDataSource();
      if (list != null)
      {
        documentsList.DataSource = list.GetItems(ItemsRetrievingOptions.DefaultOptions);
      }
      this.LabelSharepointSite.Text = this.GetBreadcrumbValue(folderPathValue);
    }

    protected string GetBreadcrumbValue(string folderPathValue)
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
            this.documentsList.ClientID,
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
                this.documentsList.ClientID,
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
  }
}