namespace Sitecore.Sharepoint.Web
{
  using System;
  using System.Collections.Specialized;
  using System.Diagnostics;
  using System.Net;
  using System.Web.Services.Protocols;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using Sitecore.Globalization;
  using Sitecore.Sharepoint.Common.Texts;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.ObjectModel;
  using Sitecore.Sharepoint.ObjectModel.Entities;
  using Sitecore.Sharepoint.ObjectModel.Search;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Sharepoint.ObjectModel.Utils;
  using Sitecore.Sharepoint.Web.Extensions;

  public partial class SharepointSearch : System.Web.UI.UserControl
  {
    private const int pageSize = 10;

    private const string SharepointAttemptNumber = "Sharepoint_Attempt_Number";

    /// <summary>
    /// Gets or sets the server URL.
    /// </summary>
    /// <value>The server URL.</value>
    public string ServerUrl
    { get; protected set; }

    /// <summary>
    /// Gets or sets the web.
    /// </summary>
    /// <value>The web.</value>
    public string Web
    { get; protected set; }

    public string ListName { get; set; }

    public string SharepointServerType { get; set; }


    /// <summary>
    /// Gets the <see cref="T:System.Web.HttpContext"/> object associated with the server control for the current Web request.
    /// </summary>
    /// <value>The sp context.</value>
    /// <returns>
    /// The specified <see cref="T:System.Web.HttpContext"/> object associated with the current request.
    /// </returns>
    public SpContext SpContext
    { get; protected set; }

    protected void Page_Load(object sender, EventArgs e)
    {
      string queryString = Attributes["sc_parameters"];
      NameValueCollection param = Sitecore.Web.WebUtil.ParseUrlParameters(queryString);
      if (!string.IsNullOrEmpty(queryString))
      {
        if (!string.IsNullOrEmpty(param["Web"]))
        {
          this.Web = param["Web"];
        }

        if (!string.IsNullOrEmpty(param["Server"]))
        {
          this.ServerUrl = param["Server"];
        }

        if (!string.IsNullOrEmpty(param["SharepointServerType"]))
        {
          this.SharepointServerType = param["SharepointServerType"];
        }
      }

      if (string.IsNullOrEmpty(this.Web))
      {
        this.Web = SharepointUtils.CurrentWebPath;
      }

      if (string.IsNullOrEmpty(this.ServerUrl))
      {
        ServerUrl = SharepointUtils.CurrentSharepointServer;
      }

      this.SpContext = SpContextProviderBase.Instance.CreateUIContext(this.ServerUrl, this.Web);
      NetworkCredential headerCredential = SharepointExtension.GetCredentialsFromHeader(Request);
      if (headerCredential != null)
      {
        SpContext.Credentials = headerCredential;
      }

      if (!string.IsNullOrEmpty(queryString) && string.IsNullOrEmpty(this.ListName) && !string.IsNullOrEmpty(param["List"]))
      {
        var list = ObjectModel.Entities.Lists.BaseList.GetList(SpContext.Url, param["List"], SpContext);
        if (list != null)
        {
          this.ListName = list is ObjectModel.Entities.Lists.List ? "Lists/" + list.Name : list.Name;
        }
      }
    }
    protected override void OnPreRender(EventArgs e)
    {
      base.OnPreRender(e);
      if (this.Visible)
      {
        ScriptManager.RegisterStartupScript(this.searchResults, this.GetType(), "hoverIntentScript_Init", "initHoverIntent('');", true);
      }
    }
    protected void searchBtn_Click(object sender, EventArgs e)
    {
      string siteName = advancedSearchSitesPane.Visible ? sitesList.SelectedValue : this.Web;
      string listName = advancedSearchSitesPane.Visible ? listsList.SelectedValue : this.ListName;
      listName = (listName == Translate.Text(UIMessages.AllLists)) ? string.Empty : listName;

      Query searchQuery = new Query(searchBox.Text) { Range = new QueryRange { Count = pageSize, StartAt = 1 } };
      if (!string.IsNullOrEmpty(siteName) || !string.IsNullOrEmpty(listName))
      {
        searchQuery.AddFilter(siteName, listName);
      }

      try
      {
        SearchBase searchServer;
        if (this.SharepointServerType == "WSS")
        {
          searchServer = new WSSSearch(SpContext);
        }
        else
        {
          searchServer = new MOSSSearch(SpContext);
        }
        SearchResultCollection searchResultCollection = searchServer.Search(searchQuery);
        searchResults.DataSource = searchResultCollection;
        if ((searchResultCollection == null) || (searchResultCollection.Count == 0))
        {
          searchResults.HeaderTemplate =
            new CompiledTemplateBuilder(
              headerControl =>
              headerControl.Controls.Add(new Literal { Text = Translate.Text(UIMessages.NoResultsCouldBeFoundForThisQuery) }));
        }
        searchResults.DataBind();
        this.HideErrorLabel();
        AttemptNumber = 0;
      }
      catch (UriFormatException ex)
      {
        errorLbl.Text = Translate.Text(UIMessages.YouDoNotHaveAccessToSharepointSearchService);
        ShowErrorLabel();
        return;
      }
      catch (WebException ex)
      {
        errorLbl.Text = Translate.Text(UIMessages.CouldntGetResponseFromSharepointServer);
        HttpWebResponse webResponse = ex.Response as HttpWebResponse;
        if ((webResponse != null) && (webResponse.StatusCode == HttpStatusCode.Unauthorized) &&
            (webResponse.Headers.AllKeys.Contains("WWW-Authenticate")))
        {
          errorLbl.Text = Translate.Text(UIMessages.YouDoNotHaveEnoughRights);
          if (AttemptNumber < 3)
          {
            SharepointExtension.WriteAuthenticationResponseBasic(Request, Response);
            AttemptNumber++;
          }
        }
        this.ShowErrorLabel();
        return;
      }
      catch (SoapException ex)
      {
        if (ex.Detail.InnerText == "ERROR_ALL_NOISE")
        {
          this.errorLbl.Text = Translate.Text(UIMessages.YourQueryIncludedOnlyCommonWordsAndOrCharactersWhichWereRemovedNoResultsAreAvailableTryToAddQueryTerms);
        }
        else
        {
          errorLbl.Text = Translate.Text(UIMessages.YouDoNotHaveAccessToSharepointSearchService);
        }
        ShowErrorLabel();
        return;
      }
    }

    /// <summary>
    /// Shows the error label.
    /// </summary>
    private void ShowErrorLabel()
    {
      resultsDiv.Visible = false;
      errorLbl.Visible = true;
    }

    /// <summary>
    /// Hides the error label.
    /// </summary>
    private void HideErrorLabel()
    {
      resultsDiv.Visible = true;
      errorLbl.Visible = false;
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
        return MainUtil.GetInt(Session[SharepointAttemptNumber], 0);
      }
      [DebuggerStepThrough]
      set
      {
        Session[SharepointAttemptNumber] = value;
      }
    }

    protected virtual string GetIconUrl(object dataItem)
    {
      string iconUrl = string.Empty;
      SearchResult searchResult = dataItem as SearchResult;
      if (searchResult != null)
      {
        iconUrl = searchResult.Thumbnail;
      }
      return iconUrl;
    }

    protected virtual string GetTitle(object dataItem)
    {
      string title = string.Empty;
      SearchResult searchResult = dataItem as SearchResult;
      if (searchResult != null)
      {
        title = searchResult["Title"];
      }
      return title;
    }

    protected virtual string GetUrl(object dataItem)
    {
      string url = string.Empty;
      SearchResult searchResult = dataItem as SearchResult;
      if (searchResult != null)
      {
        url = searchResult["Path"];
      }
      return url;
    }

    protected virtual string GetSize(object dataItem, bool addSeparator)
    {
      string size = string.Empty;
      SearchResult searchResult = dataItem as SearchResult;
      if (searchResult != null && searchResult["Size"] != "0")
      {
        if (addSeparator)
        {
          size = " - ";
        }
        size += searchResult["Size"] + " " + Translate.Text(UIMessages.Bytes);
      }
      return size;
    }

    protected virtual string GetAuthor(object dataItem, bool addSeparator)
    {
      string author = string.Empty;
      SearchResult searchResult = dataItem as SearchResult;
      if (searchResult != null)
      {
        if (addSeparator)
        {
          author = " - ";
        }
        author += searchResult["Author"];
      }
      return author;
    }

    protected virtual string GetSynopsis(object dataItem)
    {
      string synopsis = string.Empty;
      SearchResult searchResult = dataItem as SearchResult;
      if (searchResult != null)
      {
        synopsis = searchResult["HitHighlightedSummary"];
      }
      return synopsis;
    }

    protected virtual string GetModified(object dataItem, bool addSeparator)
    {
      string modified = string.Empty;
      SearchResult searchResult = dataItem as SearchResult;
      if (searchResult != null)
      {
        if (addSeparator)
        {
          modified = " - ";
        }
        modified += searchResult["Write"];
      }
      return modified;
    }

    protected void pageLinkButton_click(object sender, EventArgs e)
    {
      LinkButton linkButton = sender as LinkButton;
      int pageIndex = MainUtil.GetInt(linkButton.CommandArgument, -1);
      pageIndex--;
      int startIndex = pageIndex * pageSize + 1;
      SearchBase searchServer;
      if (this.SharepointServerType == "WSS")
      {
        searchServer = new WSSSearch(SpContext);
      }
      else
      {
        searchServer = new MOSSSearch(SpContext);
      }

      string siteName = advancedSearchSitesPane.Visible ? sitesList.SelectedValue : this.Web;
      string listName = advancedSearchSitesPane.Visible ? listsList.SelectedValue : this.ListName;
      listName = (listName == Translate.Text(UIMessages.AllLists)) ? string.Empty : listName;
      Query searchQuery = new Query(searchBox.Text) { Range = new QueryRange { Count = pageSize, StartAt = startIndex } };
      if (!string.IsNullOrEmpty(siteName) || !string.IsNullOrEmpty(listName))
      {
        searchQuery.AddFilter(siteName, listName);
      }
      SearchResultCollection searchResultCollection = searchServer.Search(searchQuery);
      searchResults.DataSource = searchResultCollection;
      searchResults.DataBind();
    }

    protected void searchResults_dataBinding(object sender, EventArgs e)
    {
      List<SearchPageItem> searchPagerSource = new List<SearchPageItem>();
      SearchResultCollection searchResult = ((Repeater)sender).DataSource as SearchResultCollection;
      if (searchResult != null)
      {
        int startRow = searchResult.StartResultIndex;
        int rowLimit = searchResult.ResultsLimit;
        int rowsTotal = searchResult.TotalRows;
        int pageIndex = (int)Math.Floor((decimal)startRow / rowLimit);
        if (pageIndex == 0)
        {
          int lastPage = (((double)rowsTotal / rowLimit) == Math.Floor((double)rowsTotal / rowLimit))
                           ? (int)Math.Floor((decimal)rowsTotal / rowLimit)
                           : (int)Math.Floor((decimal)rowsTotal / rowLimit) + 1;
          lastPage = (lastPage > 5) ? 5 : lastPage;
          for (int i = 1; i <= lastPage; i++)
          {
            searchPagerSource.Add(
              new SearchPageItem { Title = i.ToString(), Index = i.ToString(), Current = i == pageIndex + 1 });
          }
        }
        else
        {
          searchPagerSource.Add(new SearchPageItem { Title = Translate.Text(UIMessages.ltPrev), Index = (pageIndex).ToString() });
          int lastPage = (((double)rowsTotal / rowLimit) == Math.Floor((double)rowsTotal / rowLimit))
                           ? (int)Math.Floor((decimal)rowsTotal / rowLimit)
                           : (int)Math.Floor((decimal)rowsTotal / rowLimit) + 1;
          lastPage = (pageIndex + 5 < lastPage) ? pageIndex + 5 : lastPage;
          int firstPage = lastPage > 10 ? pageIndex - 4 : 1;
          for (int i = firstPage; i <= lastPage; i++)
          {
            searchPagerSource.Add(
              new SearchPageItem { Title = i.ToString(), Index = i.ToString(), Current = i == pageIndex + 1 });
          }
        }
        if ((pageIndex + 1) * pageSize < rowsTotal)
        {
          searchPagerSource.Add(new SearchPageItem { Title = Translate.Text(UIMessages.NextGt), Index = (pageIndex + 2).ToString() });
        }
      }
      pagingRepeater.DataSource = searchPagerSource;
      pagingRepeater.DataBind();
    }

    public class SearchPageItem
    {
      public string Title
      { get; set; }
      public string Index
      { get; set; }
      public bool Current
      { get; set; }
    }

    protected void advancedSearchBtn_Click(object sender, EventArgs e)
    {
      advancedSearchBtn.Text = "»&nbsp;" + Translate.Text(UIMessages.AdvancedSearch);
      advancedSearchSitesPane.Visible = !advancedSearchSitesPane.Visible;
      advancedSearchListsPane.Visible = !advancedSearchListsPane.Visible;

      if (advancedSearchSitesPane.Visible)
      {
        advancedSearchBtn.Text = "»&nbsp;" + Translate.Text(UIMessages.AdvancedSearch);
        Server spServer = ObjectModel.Entities.Server.Connect(SpContext);
        try
        {
          List<System.Web.UI.WebControls.ListItem> webs = new List<System.Web.UI.WebControls.ListItem> { new System.Web.UI.WebControls.ListItem(Translate.Text(UIMessages.AllSites), this.Web) };
          webs.AddRange(spServer.Webs.Select(web => new ListItem(web.Title, web.Path)));
          if ((webs.Count > 0))
          {
            sitesList.DataSource = webs;
          }
          sitesList.DataTextField = "Text";
          sitesList.DataValueField = "Value";
          sitesList.DataBind();

          listsList.DataSource = new[] { new System.Web.UI.WebControls.ListItem(Translate.Text(UIMessages.AllLists)) };
          listsList.DataBind();
          AttemptNumber = 0;
        }
        catch (WebException ex)
        {
          sitesList.DataSource = new[] { new System.Web.UI.WebControls.ListItem(Translate.Text(UIMessages.AllSites), this.Web) };
          sitesList.DataBind();

          listsList.DataSource = new[] { new System.Web.UI.WebControls.ListItem(Translate.Text(UIMessages.AllLists)) };
          listsList.DataBind();

          errorLbl.Text = Translate.Text(UIMessages.CouldntGetResponseFromSharepointServer);
          HttpWebResponse webResponse = ex.Response as HttpWebResponse;
          if ((webResponse != null) && (webResponse.StatusCode == HttpStatusCode.Unauthorized) &&
              (webResponse.Headers.AllKeys.Contains("WWW-Authenticate")))
          {
            errorLbl.Text = Translate.Text(UIMessages.YouDoNotHaveEnoughRights);
            if (AttemptNumber < 3)
            {
              SharepointExtension.WriteAuthenticationResponseBasic(Request, Response);
              AttemptNumber++;
            }
          }
          this.ShowErrorLabel();
          return;
        }
        catch (SoapException ex)
        {
          SharepointUtils.LogDebugInfo(SpContext, "Can't retrieve info for advanced search");
          return;
        }
      }
    }

    protected void sitesList_IndexChanged(object sender, EventArgs e)
    {
      List<System.Web.UI.WebControls.ListItem> lists = new List<System.Web.UI.WebControls.ListItem> { new System.Web.UI.WebControls.ListItem(Translate.Text(UIMessages.AllLists)) };
      listsList.DataSource = null;
      listsList.Items.Clear();
      string siteName = sitesList.SelectedValue;
      if (!string.IsNullOrEmpty(siteName) && (sitesList.SelectedIndex > 0))
      {
        Server spServer = ObjectModel.Entities.Server.Connect(SpContext);
        try
        {
          Web selectedWeb = spServer.Webs.First(web => web.Path == siteName);
          if (selectedWeb != null)
          {
            lists.AddRange(selectedWeb.Lists.Select(list => new ListItem(list.Name, list is ObjectModel.Entities.Lists.List ? "Lists/" + list.Name : list.Name)));
          }
        }
        catch (WebException ex)
        {
          HttpWebResponse webResponse = ex.Response as HttpWebResponse;
          if ((webResponse != null) && (webResponse.StatusCode == HttpStatusCode.Unauthorized) &&
              (webResponse.Headers.AllKeys.Contains("WWW-Authenticate")))
          {
            SharepointExtension.WriteAuthenticationResponseBasic(Request, Response);
          }
          return;
        }
        catch (SoapException ex)
        {
          SharepointUtils.LogDebugInfo(SpContext, "Couldn't retrieve lists for {0} site.\n{1}", siteName, ex.StackTrace);
          return;
        }
      }
      listsList.DataTextField = "Text";
      listsList.DataValueField = "Value";
      listsList.DataSource = lists;
      listsList.DataBind();
    }
  }
}