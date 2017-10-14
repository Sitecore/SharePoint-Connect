// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentTypeCollection.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ContentTypeCollection class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Collections
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Connectors;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;

  public class ContentTypeCollection : BaseCollection<ContentType>
  {
    [Obsolete("It is never used and will be removed in future versions.")]
    protected readonly ContentTypeCollectionConnector Connector;
    protected readonly SpContext Context;
    protected readonly BaseList List;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentTypeCollection"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="list">The list.</param>
    public ContentTypeCollection([NotNull] SpContext context, [NotNull] BaseList list)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(list, "list");

      this.Connector = new ContentTypeCollectionConnector(context, list.WebUrl);
      this.Context = context;
      this.List = list;
    }

    [NotNull]
    protected override List<ContentType> GetEntities()
    {
      if (this.Entities == null)
      {
        EntityValues contentTypeValuesCollection = new ContentTypeCollectionConnector(this.Context, this.List.WebUrl).GetContentTypes(this.List.ID);

        this.Entities = new List<ContentType>();
        foreach (EntityValues contentTypeValues in contentTypeValuesCollection["ContentTypes"])
        {
          this.Entities.Add(new ContentType(contentTypeValues, this.List, this.Context));
        }
      }

      return this.Entities;
    }
  }
}
