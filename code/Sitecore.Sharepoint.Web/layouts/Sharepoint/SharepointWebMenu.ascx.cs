// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharepointWebMenu.ascx.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SharepointWebMenu class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Intranet.Sharepoint.Web.Layouts
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Web;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using Sitecore.Collections;
  using Sitecore.Data.Items;
  using Sitecore.Data.Managers;
  using Sitecore.Diagnostics;
  using Sitecore.Intranet.Sharepoint.Web.Utils;
  using Sitecore.Intranet.Utils;
  using Sitecore.Links;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Common.Configuration;
  using Sitecore.Sharepoint.ObjectModel;
  using Sitecore.Sharepoint.ObjectModel.Entities;
  using Sitecore.Sharepoint.ObjectModel.Utils;
  using Sitecore.Sharepoint.Web;

  public struct MenuRenderingInfo
  {
    public MenuRenderingInfo(Item servingItem, Web web)
      : this()
    {
      this.Title = web.Title;
      this.Link = SharepointWebMenu.GetServingUrl(servingItem, web);
      Item = servingItem;
      this.Object = web;
      this.Selected = SharepointWebMenu.IsSelectedItem(this);
    }

    public MenuRenderingInfo(Item item)
      : this()
    {
      this.Title = IntranetLanguageResolver.GetMenuTitle(item);
      this.Link = LinkManager.GetItemUrl(item);
      if (!String.IsNullOrEmpty(item["Server"]))
      {
        this.Link = this.Link + "?spWebServer=" + HttpUtility.UrlEncode(item["Server"]);
      }

      Item = item;
      this.Object = null;
      this.Selected = SharepointWebMenu.IsSelectedItem(this);
    }

    public string Title { get; set; }

    public string Link { get; set; }

    public Item Item { get; set; }

    public object Object { get; set; }

    public bool Selected { get; set; }
  }

  public partial class SharepointWebMenu : UserControl
  {
    public Item MenuRootItem
    {
      get
      {
        return WebUtil.GetSectionRootItem();
      }
    }

    public static bool IsSelectedItem(object item)
    {
      if (item == null)
      {
        return false;
      }

      var renderInfo = (MenuRenderingInfo)item;
      if (renderInfo.Item == null)
      {
        return false;
      }

      if (renderInfo.Item.Name != "*")
      {
        return renderInfo.Item.Axes.IsAncestorOf(Sitecore.Context.Item);
      }

      var web = renderInfo.Object as Web;
      return web != null && renderInfo.Item.Axes.IsAncestorOf(Sitecore.Context.Item) && CurrentWebPath().StartsWith(web.Path);
    }

    public static string GetServingUrl(Item servingItem, Web web)
    {
      var path = new StringBuilder();
      if (servingItem.Name == "*")
      {
        path.Append(LinkManager.GetItemUrl(servingItem.Parent, new UrlOptions() { AddAspxExtension = false, LanguageEmbedding = LanguageEmbedding.Never }));
        path.Append("/web.aspx");
      }
      else
      {
        path.Append(LinkManager.GetItemUrl(servingItem, new UrlOptions() { AddAspxExtension = true, LanguageEmbedding = LanguageEmbedding.Never }));
      }

      path.AppendFormat("?spWebServer={0}&spWebPath={1}", HttpUtility.UrlEncode(web.ServerUrl), HttpUtility.UrlEncode(web.Path));

      return path.ToString();
    }

    public Item GetServeItem(Web web)
    {
      if (Sitecore.Context.Item.Name == "*")
      {
        return Sitecore.Context.Item.Parent;
      }

      return Sitecore.Context.Item;
    }

    protected static string CurrentWebPath()
    {
      return Sitecore.Context.Request.GetQueryString("spWebPath");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
      if (ScriptManager.GetCurrent(this.Page) == null || !ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
      {
        this.webList.DataSource = this.LoadData(this.MenuRootItem);
        this.webList.ItemDataBound += this.webList_ItemDataBound;
      }
    }

    protected void webList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
      if (IsSelectedItem(e.Item.DataItem))
      {
        var renderingInfo = (MenuRenderingInfo)e.Item.DataItem;
        renderingInfo.Selected = true;
        Control subWebList = this.CreateSubWebRendering(renderingInfo);
        if (subWebList != null)
        {
          e.Item.Controls.Add(subWebList);
        }
      }
    }

    protected List<MenuRenderingInfo> LoadSharepointWebs(Item webItem)
    {
      var resultedList = new List<MenuRenderingInfo>();
      var result = this.GetSourceWeb(webItem);
      string server = result.Part1;
      string webName = result.Part2;

      if (string.IsNullOrEmpty(server))
      {
        return resultedList;
      }

      SpContext context = new SpUiContext(server);
      Server sharepointServer = Sitecore.Sharepoint.ObjectModel.Entities.Server.Connect(context);

      if (webItem.Name == "*")
      {
        if (string.IsNullOrEmpty(webName))
        {
          // star on top level
          try
          {
            resultedList.AddRange(ObjectModelUtil.GetAccessibleWebs(sharepointServer.Webs).Select(web => new MenuRenderingInfo(webItem, web)));
          }
          catch (Exception e)
          {
            SharepointUtils.LogDebugInfo(context, "Can't retrieve webs from {0} from {1}", webName, server);
          }
        }
        else
        {
          try
          {
            Web subWeb = sharepointServer.GetWeb(webName);
            resultedList.AddRange(ObjectModelUtil.GetAccessibleWebs(subWeb.Webs).Select(web => new MenuRenderingInfo(webItem, web)));
          }
          catch (Exception e)
          {
            SharepointUtils.LogDebugInfo(context, "Can't load web {0} from {1}", webName, server);
            Log.Error("Can't load web " + webName + " from " + server + " server", e, this);
          }
        }
      }
      else
      {
        try
        {
          Web web = sharepointServer.GetWeb(webName);
          resultedList.Add(new MenuRenderingInfo(webItem, web));
        }
        catch (Exception e)
        {
          SharepointUtils.LogDebugInfo(context, "Can't load web {0} from {1}", webName, server);
          Log.Error("Can't load web " + webName + " from " + server + " server", e, this);
        }
      }

      return resultedList;
    }

    [NotNull]
    protected Pair<string, string> GetSourceWeb([NotNull] Item webItem)
    {
      Assert.ArgumentNotNull(webItem, "webItem");

      string serverName = webItem["Server"];
      string webName = webItem["Web"];
      
      if (webItem.Name == "*" && 
          string.IsNullOrEmpty(serverName) && 
          string.IsNullOrEmpty(webName) && 
          webItem.Parent != null &&
          TemplateManager.GetTemplate(webItem.Parent).DescendsFromOrEquals(TemplateIDs.SharepointWeb))
      {
        Pair<string, string> sourceWeb = this.GetSourceWeb(webItem.Parent);

        serverName = sourceWeb.Part1;
        webName = sourceWeb.Part2;
      }

      if (string.IsNullOrEmpty(serverName))
      {
        serverName = Settings.DefaultSharepointServer;
      }

      return new Pair<string, string>(serverName, webName);
    }

    private Control CreateSubWebRendering(MenuRenderingInfo renderingInfo)
    {
      var container = new Panel
      {
        CssClass = "subWebContainer"
      };
      var renderer = new Repeater
      {
        ItemTemplate = this.webList.ItemTemplate,
        HeaderTemplate = this.webList.HeaderTemplate,
        FooterTemplate = this.webList.FooterTemplate
      };
      renderer.ItemDataBound += this.webList_ItemDataBound;
      List<MenuRenderingInfo> source = this.LoadData(renderingInfo);
      renderer.DataSource = source;
      container.Controls.Add(renderer);
      renderer.DataBind();
      return container;
    }

    private List<MenuRenderingInfo> LoadData(MenuRenderingInfo info)
    {
      if (info.Item.Name == "*" && info.Object is Web)
      {
        var web = info.Object as Web;
        var result = new List<MenuRenderingInfo>();
        try
        {
          result.AddRange(ObjectModelUtil.GetAccessibleWebs(web.Webs).Select(subWeb => new MenuRenderingInfo(info.Item, subWeb)));
        }
        catch (Exception)
        {
        }

        return result;
      }

      return this.LoadData(info.Item);
    }

    private List<MenuRenderingInfo> LoadData(Item root)
    {
      var resultedList = new List<MenuRenderingInfo>();
      foreach (Item webItem in root.Children)
      {
        if (!webItem.TemplateID.Equals(TemplateIDs.SharepointWeb))
        {
          resultedList.Add(new MenuRenderingInfo(webItem));
          continue;
        }

        resultedList.AddRange(this.LoadSharepointWebs(webItem));
      }

      return resultedList;
    }
  }
}