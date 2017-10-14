// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Web.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the Web type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Sitecore.Sharepoint.ObjectModel.Entities
{
  using System;
  using System.Collections.Generic;
  using System.Xml;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Connectors;
  using Sitecore.Sharepoint.ObjectModel.Entities.Collections;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Utils;
  using Sitecore.Text;

  /// <summary>
  /// Represents information about Sharepoint Web and method for work with him.
  /// </summary>
  public class Web : CommonEntity
  {
    /// <summary>
    /// Contains all Webs with hashe for current Web.
    /// </summary>
    [NotNull]
    private readonly Dictionary<int, Web> websCache;

    /// <summary>
    /// Contains all Lists for current Web.
    /// </summary>
    private ListCollection lists;

    /// <summary>
    /// Contains all Webs for current Web.
    /// </summary>
    private WebCollection webs;

    /// <summary>
    /// Contains name of Web.
    /// </summary>
    private string name;

    /// <summary>
    /// The parent path.
    /// </summary>
    private string parentPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="Web"/> class.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="context">The context.</param>
    public Web([NotNull] EntityValues values, [NotNull] SpContext context)
      : base(values.Properties, context)
    {
      Assert.ArgumentNotNull(values, "values");
      Assert.ArgumentNotNull(context, "context");

      this.websCache = new Dictionary<int, Web>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Web"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    protected Web([NotNull] SpContext context)
      : base(context)
    {
      Assert.ArgumentNotNull(context, "context");

      this.websCache = new Dictionary<int, Web>();
    }

    /// <summary>
    /// Gets Lists of Web.
    /// </summary>
    /// <value>The lists.</value>
    [NotNull]
    public ListCollection Lists
    {
      get
      {
        if (this.lists == null)
        {
          this.lists = new ListCollection(this.Context, new Uri(this.Url));
        }

        return this.lists;
      }
    }

    /// <summary>
    /// Gets Title.
    /// </summary>
    /// <value>The title.</value>
    [CanBeNull]
    public string Title
    {
      get
      {
        return this["Title"];
      }
    }

    /// <summary>
    /// Gets URL of Web.
    /// </summary>
    /// <value>The URL for Web.</value>
    [NotNull]
    public virtual string Url
    {
      get
      {
        return this["Url"];
      }
    }

    /// <summary>
    /// Gets server url of web.
    /// </summary>
    /// <value>The server URL.</value>
    [NotNull]
    public string ServerUrl
    {
      get
      {
        return this.Context.Url;
      }
    }

    /// <summary>
    /// Gets parent path.
    /// </summary>
    /// <value>The parent path.</value>
    [NotNull]
    public string ParentPath
    {
      get
      {
        if (this.parentPath == null)
        {
          var webUrl = new UrlString(this.Url);
          string webPath = webUrl.Path;

          int schemaIndex = webPath.IndexOf("://");
          if (schemaIndex > 0)
          {
            webPath = webPath.Substring(schemaIndex + 3);
          }

          string[] pathParts = webPath.Split('/');

          this.parentPath = string.Empty;

          for (int i = pathParts.Length - 2; i >= 1; i--)
          {
            this.parentPath = pathParts[i] + '/' + this.parentPath;
          }

          this.parentPath = StringUtil.RemovePostfix('/', this.parentPath);
        }

        return this.parentPath;
      }
    }

    /// <summary>
    /// Gets path to the Web.
    /// </summary>
    [CanBeNull]
    public string Path
    {
      get
      {
        return StringUtil.RemovePrefix('/', this.ParentPath + '/' + this.Name);
      }
    }

    /// <summary>
    /// Gets Webs for current Web.
    /// </summary>
    [NotNull]
    public WebCollection Webs
    {
      get
      {
        this.LoadWebs();
        return this.webs;
      }
    }

    /// <summary>
    /// Gets name of Web.
    /// </summary>
    /// <value>The name of Web.</value>
    [NotNull]
    public virtual string Name
    {
      get
      {
        if (this.name == null)
        {
          var webUrl = new UrlString(this.Url);
          string webPath = webUrl.Path;

          int schemaIndex = webPath.IndexOf("://");
          if (schemaIndex > 0)
          {
            webPath = webPath.Substring(schemaIndex + 3);
          }

          string[] pathParts = webPath.Split('/');
          this.name = string.Empty;
          if (pathParts.Length > 1)
          {
            this.name = pathParts[pathParts.Length - 1];
          }
        }

        return this.name;
      }
    }

    /// <summary>
    /// Get specified web by Name.
    /// </summary>
    /// <param name="webName">The name of web.</param>
    /// <returns>Specified web by Name</returns>
    [CanBeNull]
    public Web GetWeb([NotNull] string webName)
    {
      string webUrl = StringUtil.EnsurePostfix('/', this.Context.Url) + StringUtil.RemovePrefix('/', webName);
      var connector = new WebConnector(this.Context, new Uri(webUrl));
      return new Web(connector.GetWeb(), this.Context);
    }

    ////////public bool AddList(string name, string type)
    ////////{
    ////////  return false;
    ////////}
    ////////public bool RemoveList(string name, string type)
    ////////{
    ////////  return false;
    ////////}

    /// <summary>
    /// Load all Webs for current object.
    /// </summary>
    private void LoadWebs()
    {
      if (this.webs == null)
      {
        this.webs = new WebCollection(this.Context, new Uri(this.Url));
      }
    }
  }
}