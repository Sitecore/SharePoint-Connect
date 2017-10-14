using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Sitecore.Collections;

namespace Sitecore.Sharepoint.ObjectModel.Search
{
  using Diagnostics;

  public class SearchResultCollection : List<SearchResult>
  {
    private List<SearchResult> items = new List<SearchResult>();
    private Query searchQuery;

    public SearchResultCollection([NotNull]XmlNode node, [NotNull]Query query, SpContext context)
    {
      Assert.ArgumentNotNull(node, "node");
      Assert.ArgumentNotNull(query, "query");
      Properties = new StringDictionary();
      searchQuery = query;
      LoadItems(node, context);
    }
    public void SetProperties ([NotNull]DataSet data)
    {
      Assert.ArgumentNotNull(data, "data");
      CopyProperties(data.ExtendedProperties);
      CopyProperties(data.Tables[0].ExtendedProperties);
    }
    protected void CopyProperties (PropertyCollection collection)
    {
      foreach (DictionaryEntry entry in collection)
      {
        Properties.Add(entry.Key.ToString(), entry.Value.ToString());
      }
    }

    private void LoadItems(XmlNode node, SpContext context)
    {
      foreach (XmlNode childNode in node.SelectNodes("//RelevantResults"))
      {
        SearchResult result = new SearchResult(childNode, context);
        this.Add(result);
      }
    }

    public StringDictionary Properties { get; protected set; }

    public int StartResultIndex
    {
      get
      {
        return searchQuery.Range != null ? searchQuery.Range.StartAt : 1;
      }
    }

    public int ResultsLimit
    {
      get
      {
        return searchQuery.Range != null ? searchQuery.Range.Count : 10;
      }
    }

    public int TotalRows
    {
      get
      {
        return MainUtil.GetInt(Properties["TotalRows"], -1);
      }
    }

    public bool IsTotalRowsExact
    {
      get
      {
        return MainUtil.GetBool(Properties["IsTotalRowsExact"], false);
      }
    }
  }
}
