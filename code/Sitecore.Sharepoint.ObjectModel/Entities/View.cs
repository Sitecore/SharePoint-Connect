// --------------------------------------------------------------------------------------------------------------------
// <copyright file="View.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the View type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Sitecore.Sharepoint.ObjectModel.Entities
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Connectors;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;

  /// <summary>
  /// Represents information about Sharepoint view and method for work with him.
  /// </summary>
  public class View : CommonEntity
  {
    protected readonly SpContext Context;
    /// <summary>
    /// Gets connector for view.
    /// </summary>
    [Obsolete("It is never used and will be removed in future versions.")]
    protected readonly ViewConnector Connector;

    /// <summary>
    /// List with field name
    /// </summary>
    private List<string> fieldNames;

    /// <summary>
    /// Indicate that fields is initialized.
    /// </summary>
    private bool initialized;

    /// <summary>
    /// Contains page size for this view.
    /// </summary>
    private int pageSize;

    /// <summary>
    /// Indicate that paging is enabled.
    /// </summary>
    private bool pagingEnabled;

    protected BaseList List { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="list">The list.</param>
    /// <param name="context">The context.</param>
    public View([NotNull] EntityValues values, [NotNull] BaseList list, [NotNull] SpContext context)
      : base(values.Properties, context)
    {
      Assert.ArgumentNotNull(values, "values");
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(context, "context");

      this.Connector = new ViewConnector(context, list.WebUrl);
      this.Context = context;
      this.List = list;

      if (values["ViewFields"].Length > 0 && values.Properties["RowLimit"] != null)
      {
        this.Initialize(values);
      }
    }

    /// <summary>
    /// Gets names of field.
    /// </summary>
    /// <value>The field names.</value>
    [NotNull]
    public List<string> FieldNames
    {
      get
      {
        this.Initialize();
        return this.fieldNames;
      }
    }

    /// <summary>
    /// Gets display name of View.
    /// </summary>
    /// <value>The display name.</value>
    [CanBeNull]
    public string DisplayName
    {
      get
      {
        return this["DisplayName"];
      }
    }

    /// <summary>
    /// Gets name of view.
    /// </summary>
    /// <value>The name of View.</value>
    [CanBeNull]
    public string Name
    {
      get
      {
        return this["Name"];
      }
    }

    /// <summary>
    /// Gets url of View.
    /// </summary>
    /// <value>The URL of View.</value>
    [CanBeNull]
    public string Url
    {
      get
      {
        return this["Url"];
      }
    }

    /// <summary>
    /// Gets type of View.
    /// </summary>
    /// <value>The type of View.</value>
    [CanBeNull]
    public string Type
    {
      get
      {
        return this["Type"];
      }
    }

    /// <summary>
    /// Gets a value indicating whether that view is hidden.
    /// </summary>
    public bool Hidden
    {
      get
      {
        return MainUtil.GetBool(this["Hidden"], false);
      }
    }

    /// <summary>
    /// Gets or sets page size of View.
    /// </summary>
    public int PageSize
    {
      get
      {
        this.Initialize();
        return this.pageSize;
      }

      protected set
      {
        this.pageSize = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether that paging in view is enabled.
    /// </summary>
    public bool PagingEnabled
    {
      get
      {
        this.Initialize();
        return this.pagingEnabled;
      }

      protected set
      {
        this.pagingEnabled = value;
      }
    }

    /// <summary>
    /// Get instance of specified View
    /// </summary>
    /// <param name="list">The list object.</param>
    /// <param name="viewName">The view name.</param>
    /// <param name="context">The context.</param>
    /// <returns>Instance of View.</returns>
    [NotNull]
    public static View GetView([NotNull] BaseList list, [NotNull] string viewName, [NotNull] SpContext context)
    {
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(viewName, "viewName");
      Assert.ArgumentNotNull(context, "context");

      var viewConnector = new ViewConnector(context, list.WebUrl);
      EntityValues viewValues = viewConnector.GetView(list.ID, viewName);

      return new View(viewValues, list, context);
    }

    /// <summary>
    /// Initialize class.
    /// </summary>
    protected void Initialize(EntityValues values)
    {
      this.fieldNames = new List<string>();
      foreach (EntityValues fieldValues in values["ViewFields"])
      {
        this.fieldNames.Add(fieldValues.Properties["Name"].Value);
      }

      EntityPropertyValue rowLimit = values.Properties["RowLimit"];
      if (rowLimit != null && !string.IsNullOrEmpty(rowLimit.Value) && int.TryParse(rowLimit.Value, out this.pageSize))
      {
        this.PagingEnabled = true;
      }
      else
      {
        this.PagingEnabled = false;
      }

      this.initialized = true;
    }

    protected void Initialize()
    {
      if (!this.initialized)
      {
        EntityValues viewValues = new ViewConnector(this.Context, this.List.WebUrl).GetView(this.List.ID, this.Name);
        this.Initialize(viewValues);
      }
    }
  }
}