// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentType.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ContentType type.
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
  /// Represents SharePoint content type.
  /// </summary>
  public class ContentType : CommonEntity
  {
    protected readonly SpContext Context;
    /// <summary>
    /// Gets sharepoint list.
    /// </summary>
    public BaseList List { get; protected set; }
    private List<Field> fields;
    [Obsolete("It is never used and will be removed in future versions.")]
    private ContentTypeConnector Connector;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentType"/> class.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="list">The list.</param>
    /// <param name="context">The context.</param>
    public ContentType([NotNull] EntityValues values, [NotNull] BaseList list, [NotNull] SpContext context)
      : base(values.Properties, context)
    {
      Assert.ArgumentNotNull(values, "values");
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(context, "context");

      this.Context = context;
      List = list;
      Connector = new ContentTypeConnector(context, this.List.WebUrl);
    }

    /// <summary>
    /// Gets description of content type.
    /// </summary>
    public string Description
    {
      get
      {
        return this["Description"];
      }
    }

    /// <summary>
    /// Gets ID od content type.
    /// </summary>
    public string ID
    {
      get
      {
        return this["ID"];
      }
    }

    /// <summary>
    /// Gets name of contant type.
    /// </summary>
    public string Name
    {
      get
      {
        return this["Name"];
      }
    }

    /// <summary>
    /// Gets Fields of contant type.
    /// </summary>
    public List<Field> Fields
    {
      get
      {
        if (this.fields == null)
        {
          var entityValues = new ContentTypeConnector(this.Context, this.List.WebUrl).GetFields(this.List.ID, this.ID);
          this.fields = entityValues != null ? this.LoadFields(entityValues["Fields"]) : new List<Field>();
        }
        return this.fields;
      }
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
      foreach (var fieldValues in fieldValuesCollection)
      {
        result.Add(new Field(fieldValues.Properties, this.Context));
      }

      return result;
    }
  }
}