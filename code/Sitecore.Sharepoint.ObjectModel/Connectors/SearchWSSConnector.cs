// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchWSSConnector.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines SearchWSSConnector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Connectors
{
  using System;
  using System.Data;
  using System.Net;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.WebServices;
  using Sitecore.Sharepoint.Data.WebServices.SharepointSearch;
  using Sitecore.Sharepoint.ObjectModel.Search;

  /// <summary>
  /// Provides methods for work with Webs.
  /// </summary>
  public class SearchWSSConnector
  {
    protected readonly QueryService SearchWebService;
    protected readonly SpContext Context;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchWSSConnector"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public SearchWSSConnector([NotNull] SpContext context)
    {
      Assert.ArgumentNotNull(context, "context");

      this.SearchWebService = new QueryService();
      //Should be refactored.
      this.SearchWebService.SetServer(new Uri(context.Url), context);
      this.Context = context;
    }

    /// <summary>
    /// Execute query in the Windows SharePoint Services Search.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>Return DataSet with search result.</returns>
    [NotNull]
    public DataSet Search([NotNull] Query query)
    {
      Assert.ArgumentNotNull(query, "query");

      return this.SearchWebService.QueryEx(query.ToSearchString(this.Context.Url));
    }
  }
}