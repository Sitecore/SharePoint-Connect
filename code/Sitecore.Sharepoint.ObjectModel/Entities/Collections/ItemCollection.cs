// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListItemCollection.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ItemCollection class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Sitecore.Sharepoint.ObjectModel.Entities.Collections
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Connectors;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Factories;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;
  using Sitecore.Sharepoint.ObjectModel.Options;

  /// <summary>
  /// Represents SharePoint List of ListItem
  /// </summary>
  public class ItemCollection : BaseCollection<BaseItem>
  {
    private int pageIndex;

    private ItemCollectionConnector connector;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemCollection"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="list">The list.</param>
    /// <param name="options">The options.</param>
    public ItemCollection([NotNull] SpContext context, [NotNull] BaseList list, [NotNull] ItemsRetrievingOptions options)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(options, "options");

      this.Context = context;
      this.List = list;
      this.Options = options;
      this.PageIndex = 1;
      this.PagingQueryCollection = new Dictionary<int, string>();
      this.PagingQueryCollection.Add(1, string.Empty);
    }

    [Obsolete("It is never used and will be removed in future versions.")]
    protected ItemCollectionConnector Connector
    {
      get
      {
        return this.connector ?? (this.connector = new ItemCollectionConnector(this.Context, this.List.WebUrl));
      }

      set
      {
        this.connector = value;
      }
    }

    protected readonly SpContext Context;
    protected readonly BaseList List;
    protected ItemsRetrievingOptions Options { get; set; }
    protected Dictionary<int, string> PagingQueryCollection { get; set; }

    protected int PageIndex
    {
      get
      {
        return this.pageIndex;
      }

      set
      {
        if (this.pageIndex != value)
        {
          this.Entities = null;
          this.pageIndex = value;
        }
      }
    }

    public virtual bool MoveToNextPage()
    {
      return this.MoveToPage(this.PageIndex + 1);
    }

    public virtual bool MoveToPreviousPage()
    {
      return this.MoveToPage(this.PageIndex - 1);
    }

    public virtual bool CanMoveToNextPage()
    {
      return this.PagingQueryCollection.ContainsKey(this.PageIndex + 1);
    }

    public virtual bool CanMoveToPreviousPage()
    {
      return this.PageIndex > 1;
    }

    protected virtual bool MoveToPage(int pageIndexToRetrieve)
    {
      string pagingQuery;

      if (this.PagingQueryCollection.TryGetValue(pageIndexToRetrieve, out pagingQuery))
      {
        this.PageIndex = pageIndexToRetrieve;
        this.Options.PagingQuery = pagingQuery;
        this.GetEntities();

        return true;
      }

      return false;
    }

    [NotNull]
    protected override List<BaseItem> GetEntities()
    {
      if (this.Entities == null)
      {
        EntityValues itemValuesCollection = new ItemCollectionConnector(this.Context, this.List.WebUrl).GetItems(this.List, this.Options);

        this.Entities = new List<BaseItem>();
        foreach (EntityValues itemValues in itemValuesCollection["Items"])
        {
          this.Entities.Add(ItemFactory.CreateItemObject(itemValues, this.List, this.Context));
        }

        EntityPropertyValue newNextPagingQuery = itemValuesCollection.Properties["ListItemCollectionPositionNext"];
        this.UpdatePagingQueryCollection(newNextPagingQuery != null ? newNextPagingQuery.Value : null);
      }

      return this.Entities;
    }

    protected virtual void UpdatePagingQueryCollection(string newNextPagingQuery)
    {
      string nextPagingQuery;

      if (this.PagingQueryCollection.TryGetValue(this.PageIndex + 1, out nextPagingQuery))
      {
        if (string.IsNullOrEmpty(newNextPagingQuery) || nextPagingQuery != newNextPagingQuery)
        {
          var newPagingQueryCollection = new Dictionary<int, string>();
          foreach (KeyValuePair<int, string> currentPagingQuery in this.PagingQueryCollection)
          {
            if (currentPagingQuery.Key < this.PageIndex + 1)
            {
              newPagingQueryCollection.Add(currentPagingQuery.Key, currentPagingQuery.Value);
            }
          }

          if (!string.IsNullOrEmpty(newNextPagingQuery))
          {
            newPagingQueryCollection.Add(this.PageIndex + 1, newNextPagingQuery);
          }

          this.PagingQueryCollection = newPagingQueryCollection;
        }
      }
      else
      {
        if (!string.IsNullOrEmpty(newNextPagingQuery))
        {
          this.PagingQueryCollection.Add(this.PageIndex + 1, newNextPagingQuery);
        }
      }
    }
  }
}