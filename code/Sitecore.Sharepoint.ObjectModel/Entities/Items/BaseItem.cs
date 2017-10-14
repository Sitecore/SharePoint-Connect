// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseItem.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the BaseItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Items
{
  using System;
  using System.Collections.Specialized;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Texts;
  using Sitecore.Sharepoint.ObjectModel.Actions;
  using Sitecore.Sharepoint.ObjectModel.Connectors;
  using Sitecore.Sharepoint.ObjectModel.Entities.Collections;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;
  using Sitecore.Sharepoint.ObjectModel.Options;

  /// <summary>
  /// Base class for all library and list items.
  /// </summary>
  public class BaseItem : CommonEntity, IEditable
  {
    /// <summary>
    /// Gets url of Web for ListItem.
    /// </summary>
    public readonly Uri WebUrl;

    /// <summary>
    /// Gets or sets ID of list for Item.
    /// </summary>
    protected readonly string ListName;

    protected string RootFolder
    {
      get
      {
        return this.List.RootFolder;
      }
    }

    private BaseList list;

    [NotNull]
    protected BaseList List
    {
      get
      {
        if (this.list == null)
        {
          this.list = BaseList.GetList(this.WebUrl, this.ListName, this.Context);
        }

        Assert.ArgumentNotNull(this.list, "list");

        return this.list;
      }
    }

    /// <summary>
    /// Initializes static members of the <see cref="BaseItem"/> class.
    /// </summary>
    static BaseItem()
    { 
      Error.AssertLicense("Sitecore.Sharepoint", "SharePoint Integration Framework");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="list">The list.</param>
    /// <param name="context">The context.</param>
    public BaseItem([NotNull] EntityProperties property, [NotNull] BaseList list, [NotNull] SpContext context)
      : this(property, list.ID, list.WebUrl, context)
    {
      Assert.ArgumentNotNull(list, "list");

      this.list = list;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="listName">The list ID.</param>
    /// <param name="webUrl">The url of Web.</param>
    /// <param name="context">The context.</param>
    public BaseItem([NotNull] EntityProperties property, [NotNull] string listName, [NotNull] Uri webUrl, [NotNull] SpContext context)
      : base(property, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(listName, "listName");
      Assert.ArgumentNotNull(webUrl, "webUrl");
      Assert.ArgumentNotNull(context, "context");

      this.ListName = listName;
      this.WebUrl = webUrl;

      this.Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseItem"/> class.
    /// </summary>
    /// <param name="listName">The list name.</param>
    /// <param name="webUrl">The url of Web.</param>
    /// <param name="context">The context.</param>
    protected BaseItem([NotNull] string listName, [NotNull] Uri webUrl, [NotNull] SpContext context)
      : base(context)
    {
      Assert.ArgumentNotNull(listName, "listName");
      Assert.ArgumentNotNull(webUrl, "webUrl");
      Assert.ArgumentNotNull(context, "context");

      this.ListName = listName;
      this.WebUrl = webUrl;

      this.Initialize();
    }

    /// <summary>
    /// Gets ID of ListItem.
    /// </summary>
    [CanBeNull]
    public string ID
    {
      get
      {
        return this["ows_ID"];
      }

      private set
      {
        this["ows_ID"] = value;
      }
    }

    /// <summary>
    /// Gets GUID of the ListItem
    /// </summary>
    [CanBeNull]
    public virtual string GUID
    {
      get
      {
        return this["ows_GUID"];
      }
    }

    /// <summary>
    /// Gets the URL of the form that displays the current list item on the SharePoint server.
    /// </summary>
    [NotNull]
    public virtual string PropertiesUrl
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets unique ID of ListItem.
    /// </summary>
    [NotNull]
    public string UniqueID
    {
      get
      {
        var values = new StringCollection
        {
          this.WebUrl.ToString(),
          this.ListName,
          this.ID,
          this.Folder
        };
        return StringUtil.StringCollectionToString(values);
      }
    }

    /// <summary>
    /// Gets a value indicating whether that ListItem is new.
    /// </summary>
    public virtual bool IsNew
    {
      get
      {
        return this.ID == "New";
      }
    }

    /// <summary>
    /// Gets Title for item.
    /// </summary>
    [CanBeNull]
    public virtual string Title
    {
      get { return this["ows_Title"]; }
    }

    /// <summary>
    /// Gets folder for current item.
    /// </summary>
    [CanBeNull]
    protected virtual string Folder
    {
      get
      {
        string fileDirRef = StringUtil.RemovePrefix("/", StringUtil.RemovePostfix("/", this["ows_FileDirRef"]));
        string rootFolder = StringUtil.RemovePrefix("/", StringUtil.RemovePostfix("/", this.RootFolder));

        return StringUtil.RemovePrefix("/", StringUtil.RemovePrefix(rootFolder, fileDirRef));
      }
    }

    /// <summary>
    /// Delete ListItem from SharePoint.
    /// </summary>
    public virtual void Delete()
    {
      var connector = new ItemConnector(this.Context, this.WebUrl);
      connector.DeleteItem(this.Properties, this.ListName);
    }

    /// <summary>
    /// Update item in SharePoint.
    /// </summary>
    /// <returns>
    /// The result of command.
    /// </returns>
    public virtual EntityProperties Update()
    {
      var connector = new ItemConnector(this.Context, this.WebUrl);
      EntityProperties result = connector.UpdateItem(this.Properties, this.ListName);

      FillProperty(result);

      return result;
    }

    /// <summary>
    /// Gets the actions.
    /// </summary>
    /// <returns>List item actions.</returns>
    [NotNull]
    public virtual ActionCollection GetActions()
    {
      var actionsList = new ActionCollection
      {
        new ServerAction("delete", UIMessages.Delete, this.BaseItemDelete, this)
      };
      return actionsList;
    }

    /// <summary>
    /// Method for delete item from SharePoint.
    /// </summary>
    /// <param name="sender">The object which calling this method.</param>
    /// <param name="e">The arguments.</param>
    private void BaseItemDelete([CanBeNull] object sender, [CanBeNull] EventArgs e)
    {
      this.Delete();
    }

    /// <summary>
    /// Initialize item
    /// </summary>
    private void Initialize()
    {
      if (String.IsNullOrEmpty(this["ows_ID"]))
      {
        this.ID = "New";
      }
    }

    [CanBeNull]
    public static BaseItem GetItem([NotNull] string uniqueID, [NotNull] SpContext context, string viewName = null)
    {
      Assert.ArgumentNotNull(uniqueID, "uniqueID");
      Assert.ArgumentNotNull(context, "context");

      string[] values = StringUtil.Split(uniqueID, '|', true);
      string webUrl = values[0];
      string listName = values[1];
      string id = values[2];
      string folder = values[3];

      string wherePart = string.Format("<Eq><FieldRef Name=\"{0}\"/><Value Type=\"Integer\">{1}</Value></Eq>", "ID", id);
      var itemsRetrievingOptions = new ItemsRetrievingOptions
      {
        WherePart = wherePart,
        ViewName = viewName
      };
      if (!string.IsNullOrEmpty(folder))
      {
        itemsRetrievingOptions.Folder = folder;
      }

      BaseList list = BaseList.GetList(webUrl, listName, context);
      ItemCollection items = list.GetItems(itemsRetrievingOptions);

      if (items.Count == 1)
      {
        return items[0];
      }

      if (items.Count > 1)
      {
        throw new Exception("More than document has been found by ID");
      }

      throw new Exception("Document can't be found");
    }
  }
}