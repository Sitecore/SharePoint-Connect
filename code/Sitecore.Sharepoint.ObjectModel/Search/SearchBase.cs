using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitecore.Sharepoint.ObjectModel.Search
{
  using Sitecore.Diagnostics;

  public abstract class SearchBase
  {
    /// <summary>
    /// Gets or sets Context for Item.
    /// </summary>
    protected readonly SpContext Context;

        /// <summary>
    /// Initializes a new instance of the <see cref="SearchBase"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public SearchBase([NotNull] SpContext context)
    {
      Assert.ArgumentNotNull(context, "context");

      this.Context = context;
    }

    /// <summary>
    /// Method for search.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="pageIndex">The page index.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>Result of search.</returns>
    [CanBeNull]
    public virtual SearchResultCollection Search([NotNull] string query, int pageIndex, int pageSize)
    {
      Assert.ArgumentNotNullOrEmpty(query, "query");

      Assert.ArgumentCondition(pageIndex > 0, "pageIndex", "Page index is less than 1.");
      Assert.ArgumentCondition(pageSize > 0, "pageSize", "Page size is less than 1.");
      Query queryObj = new Query
      {
        Context =
        {
          QueryText = query
        },
        Range = new QueryRange
        {
          StartAt = pageIndex,
          Count = pageSize
        }
      };
      return this.Search(queryObj);
    }

    /// <summary>
    /// Search using the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>Result of search.</returns>
    [NotNull]
    public abstract SearchResultCollection Search([NotNull] Query query);
  }
}
