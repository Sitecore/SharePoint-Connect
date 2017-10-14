// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemsRetrievingOptions.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ItemsRetrievingOptions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Options
{
  using System;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Represents information about how we can retrieving items from SharePoint.
  /// </summary>
  [Serializable]
  public class ItemsRetrievingOptions
  {
    /// <summary>
    /// Gets or sets the name of the view.
    /// </summary>
    public string ViewName { get; set; }

    /// <summary>
    /// Gets or sets folder in view.
    /// </summary>
    public string Folder { get; set; }

    /// <summary>
    /// Gets or sets sorting information.
    /// </summary>
    public SortingOptions SortingInfo { get; set; }

    /// <summary>
    /// Gets or sets where part of query.
    /// </summary>
    public string WherePart { get; set; }

    /// <summary>
    /// Gets or sets a query to retrieve specified page of list items from SharePoint list.
    /// </summary>
    public string PagingQuery { get; set; }

    /// <summary>
    /// Gets or sets a value indicating max count of list items that can be retrieved from SharePoint list.
    /// </summary>
    public uint ItemLimit { get; set; }

    /// <summary>
    /// Clone current instance.
    /// </summary>
    /// <returns>New instance of object.</returns>
    [NotNull]
    public ItemsRetrievingOptions Clone()
    {
      var clone = new ItemsRetrievingOptions();
      clone.ViewName = this.ViewName;
      clone.Folder = this.Folder;
      clone.SortingInfo = this.SortingInfo;
      clone.WherePart = this.WherePart;
      clone.PagingQuery = this.PagingQuery;
      clone.ItemLimit = this.ItemLimit;

      return clone;
    }

    /// <summary>
    /// Copy data from instance.
    /// </summary>
    /// <param name="copyFrom">The copy from instance.</param>
    public void CopyFrom([NotNull] ItemsRetrievingOptions copyFrom)
    {
      Assert.ArgumentNotNull(copyFrom, "copyFrom");

      this.ViewName = copyFrom.ViewName;
      this.SortingInfo = copyFrom.SortingInfo;
      this.WherePart = copyFrom.WherePart;
      this.PagingQuery = copyFrom.PagingQuery;
      this.ItemLimit = copyFrom.ItemLimit;
    }

    /// <summary>
    /// Information about sorting.
    /// </summary>
    [Serializable]
    public struct SortingOptions
    {
      /// <summary>
      /// Sorting field name.
      /// </summary>
      public string FieldName;

      /// <summary>
      /// Indicate that sorting is ascending.
      /// </summary>
      public bool Ascending;
    }

    public static ItemsRetrievingOptions DefaultOptions
    {
      get
      {
        return new ItemsRetrievingOptions()
        {
          Folder = string.Empty,
          PagingQuery = string.Empty,
          SortingInfo = new SortingOptions()
          {
            Ascending = true,
            FieldName = string.Empty
          },
          ViewName = string.Empty,
          WherePart = string.Empty,
          ItemLimit = 0
        };
      }
    }
  }
}
