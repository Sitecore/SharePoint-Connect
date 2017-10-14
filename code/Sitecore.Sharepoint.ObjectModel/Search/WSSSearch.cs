namespace Sitecore.Sharepoint.ObjectModel.Search
{
  using System.Data;
  using System.Xml;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Connectors;

  public class WSSSearch : SearchBase
  {
    public WSSSearch([NotNull] SpContext context)
      : base(context)
    {
    }

    /// <summary>
    /// Search using the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>Result of search.</returns>
    [NotNull]
    public override SearchResultCollection Search([NotNull] Query query)
    {
      Assert.ArgumentNotNull(query, "query");

      DataSet result = new SearchWSSConnector(this.Context).Search(query);
      XmlDataDocument xmlDoc = new XmlDataDocument(result);
      SearchResultCollection searchResult = new SearchResultCollection(xmlDoc, query, this.Context);
      searchResult.SetProperties(result);
      return searchResult;
    }
  }
}
