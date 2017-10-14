// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseList.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines BaseList class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Sitecore.Sharepoint.ObjectModel.Entities.Lists
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Connectors;
  using Sitecore.Sharepoint.ObjectModel.Entities.Collections;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Factories;
  using Sitecore.Sharepoint.ObjectModel.Options;

  /// <summary>
  /// Represents information about Sharepoint list and method for work with him.
  /// </summary>
  public class BaseList : CommonEntity, IEditable, IList
  {
    /// <summary>
    /// Fields of current list.
    /// </summary>
    private List<Field> fields;

    private ContentTypeCollection contentTypes;

    /// <summary>
    /// Views of current list
    /// </summary>
    private ViewCollection views;

    /// <summary>
    /// The full URL of the current SharePoint list.
    /// </summary>
    private string url;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseList"/> class.
    /// </summary>
    /// <param name="propertyValues">
    /// The property Values.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="webUrl">
    /// The url of Web.
    /// </param>
    /// <param name="node">
    /// The node. Should be removed.
    /// </param>
    public BaseList([NotNull] EntityValues propertyValues, [NotNull] SpContext context, [NotNull] Uri webUrl)
      : base(propertyValues.Properties, context)
    {
      Assert.ArgumentNotNull(propertyValues, "propertyValues");
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");

      this.WebUrl = webUrl;
      this.fields = this.LoadFields(propertyValues["Fields"]);
    }

    /// <summary>
    /// Gets server template.
    /// </summary>
    /// <value>The server template of list.</value>
    [CanBeNull]
    public string ServerTemplate
    {
      get
      {
        return this["ServerTemplate"];
      }
    }

    /// <summary>
    /// Gets description of list.
    /// </summary>
    /// <value>The description.</value>
    [CanBeNull]
    public string Description
    {
      get
      {
        return this["Description"];
      }
    }

    /// <summary>
    /// Gets name of list.
    /// </summary>
    /// <value>The name of list.</value>
    [CanBeNull]
    public string Name
    {
      get
      {
        return this["Title"];
      }
    }

    /// <summary>
    /// Gets ID of list.
    /// </summary>
    /// <value>The ID of list.</value>
    [CanBeNull]
    public string ID
    {
      get
      {
        return this["ID"];
      }
    }

    /// <summary>
    /// Gets count of items in the list.
    /// </summary>
    public int ItemCount
    {
      get
      {
        int itemCount;
        int.TryParse(this["ItemCount"], out itemCount);
        return itemCount;
      }
    }

    /// <summary>
    /// Gets root folder for list.
    /// </summary>
    /// <value>The root folder.</value>
    [CanBeNull]
    public string RootFolder
    {
      get
      {
        return this["RootFolder"];
      }
    }

    /// <summary>
    /// Gets url for web in the list.
    /// </summary>
    /// <value>The web URL.</value>
    [CanBeNull]
    public Uri WebUrl { get; private set; }

    /// <summary>
    /// Gets the full URL of the current SharePoint list.
    /// </summary>
    /// <value>The URL of list.</value>
    [NotNull]
    public string Url
    {
      get
      {
        if (string.IsNullOrEmpty(this.url))
        {
          // TODO: [101209 dan] Verify that correct formula used for url calculation.
          this.url = StringUtil.EnsurePostfix('/', this.Context.Url) + StringUtil.RemovePrefix('/', this.RootFolder);
        }

        return this.url;
      }
    }

    /// <summary>
    /// Gets content types for list.
    /// </summary>
    /// <value>The content types.</value>
    [NotNull]
    public ContentTypeCollection ContentTypes
    {
      get
      {
        if (this.contentTypes == null)
        {
          this.contentTypes = new ContentTypeCollection(this.Context, this);
        }

        return this.contentTypes;
      }
    }

    /// <summary>
    /// Gets fields for list.
    /// </summary>
    /// <value>The fields.</value>
    [NotNull]
    public List<Field> Fields
    {
      get
      {
        if (this.fields == null)
        {
          var entityValues = new ListConnector(this.Context, this.WebUrl).GetList(this.ID);
          this.fields = entityValues != null ? this.LoadFields(entityValues["Fields"]) : new List<Field>();
        }

        return this.fields;
      }
    }

    /// <summary>
    /// Gets views for list.
    /// </summary>
    /// <value>The views.</value>
    [NotNull]
    public ViewCollection Views
    {
      get
      {
        if (this.views == null)
        {
          this.views = new ViewCollection(this.Context, this);
        }

        return this.views;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether that instance of object is new.
    /// </summary>
    public virtual bool IsNew { get; protected set; }

    /// <summary>
    /// Create new list.
    /// </summary>
    /// <param name="listName">The list name.</param>
    /// <param name="description">The description.</param>
    /// <param name="webUrl">The url of Web.</param>
    /// <param name="context">The context.</param>
    /// <param name="templateID">The template id.</param>
    /// <returns>Instance of new List.</returns>
    [CanBeNull]
    public static BaseList AddList([NotNull] string listName, [NotNull] string description, [NotNull] Uri webUrl, [NotNull] SpContext context, int templateID)
    {
      Assert.ArgumentNotNull(listName, "listName");
      Assert.ArgumentNotNull(description, "description");
      Assert.ArgumentNotNull(webUrl, "webUrl");
      Assert.ArgumentNotNull(context, "context");

      var connector = new ListConnector(context, webUrl);
      EntityValues listValues = connector.AddList(listName, description, templateID);

      return ListFactory.CreateListObject(listValues, webUrl, context);
    }

    /// <summary>
    /// Get specified list from web.
    /// </summary>
    /// <param name="webUrl">The web uri.</param>
    /// <param name="listName">The list name.</param>
    /// <param name="context">The context.</param>
    /// <returns>Specified list</returns>
    [CanBeNull]
    public static BaseList GetList([NotNull] Uri webUrl, [NotNull] string listName, [NotNull] SpContext context)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");
      Assert.ArgumentNotNull(listName, "listName");

      var connector = new ListConnector(context, webUrl);
      return ListFactory.CreateListObject(connector.GetList(listName), webUrl, context);
    }

    /// <summary>
    /// Get specified list from web.
    /// </summary>
    /// <param name="webName">The web name.</param>
    /// <param name="listName">The list name.</param>
    /// <param name="context">The context.</param>
    /// <returns>Specified list</returns>
    [CanBeNull]
    public static BaseList GetList([NotNull] string webName, [NotNull] string listName, [NotNull] SpContext context)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webName, "webName");
      Assert.ArgumentNotNull(listName, "listName");

      Uri url = GetUrlOfWeb(webName, context);
      return GetList(url, listName, context);
    }

    [NotNull]
    public virtual ItemCollection GetItems()
    {
      return this.GetItems(ItemsRetrievingOptions.DefaultOptions);
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>List items.</returns>
    [NotNull]
    public virtual ItemCollection GetItems([NotNull]ItemsRetrievingOptions options)
    {
      Assert.ArgumentNotNull(options, "options");

      return new ItemCollection(this.Context, this, options);
    }

    /// <summary>
    /// Update item in SharePoint.
    /// </summary>
    /// <returns>
    /// The result.
    /// </returns>
    [Obsolete]
    public virtual EntityProperties Update()
    {
      return null;
    }

    /// <summary>
    /// Delete list from SharePoint.
    /// </summary>
    public virtual void Delete()
    {
      new ListConnector(this.Context, this.WebUrl).DeleteList(this);
    }

    /// <summary>
    /// Return url of Web.
    /// </summary>
    /// <param name="webName">The web name.</param>
    /// <param name="context">The context.</param>
    /// <returns>Url of Web</returns>
    [NotNull]
    protected static Uri GetUrlOfWeb([NotNull] string webName, [NotNull] SpContext context)
    {
      Assert.ArgumentNotNull(webName, "webName");
      Assert.ArgumentNotNull(context, "context");

      string url;
      if (webName.Contains("://"))
      {
        url = webName;
      }
      else
      {
        url = StringUtil.EnsurePostfix('/', context.Url) + StringUtil.RemovePrefix('/', webName);
      }

      return new Uri(url);
    }

    /// <summary>
    /// Load field for list from XmlNode.
    /// </summary>
    /// <param name="fieldValuesCollection">
    /// The fields.
    /// </param>
    /// <returns>
    /// Fields for list.
    /// </returns>
    [NotNull]
    protected List<Field> LoadFields([NotNull] EntityValues[] fieldValuesCollection)
    {
      Assert.ArgumentNotNull(fieldValuesCollection, "fieldValuesCollection");

      var result = new List<Field>();
      foreach (var fieldValue in fieldValuesCollection)
      {
        result.Add(new Field(fieldValue.Properties, this.Context));
      }

      return result;
    }
  }
}