using System.Collections.Generic;

namespace Sitecore.Sharepoint.ObjectModel.Search
{
  using System;
  using System.Web;
  using System.Xml.Serialization;
  using System.IO;
  using System.Text;

  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]

  public partial class Query
  {
    private List<string> contextTargets;

    public Query()
    {
      Context = new QueryContext();
      this.EnableStemming = true;
      this.TrimDuplicates = true;
      this.IgnoreAllNoiseQuery = true;
      this.IncludeRelevantResults = true;
    }
    public Query (string queryText):this()
    {
      Context.QueryText = queryText;
    }


    /// <remarks/>
    public string QueryId { get; set; }

    /// <remarks/>
    public QuerySupportedFormats SupportedFormats { get; set; }

    /// <remarks/>
    public QueryContext Context { get; set; }

    /// <remarks/>
    public QueryRange Range { get; set; }

    /// <remarks/>
    public bool EnableStemming { get; set; }

    /// <remarks/>
    public bool TrimDuplicates { get; set; }

    /// <remarks/>
    public bool IgnoreAllNoiseQuery { get; set; }

    /// <remarks/>
    public bool IncludeRelevantResults { get; set; }

    /// <summary>
    /// Adds the filter.
    /// </summary>
    /// <param name="web">The web.</param>
    /// <param name="list">The list.</param>
    public void AddFilter(string web, string list)
    {
      if (contextTargets == null)
      {
        contextTargets = new List<string>();
      }
      string contextTarget = web + "/" + list;
      contextTargets.Add(HttpUtility.UrlPathEncode(contextTarget));
    }
    public void AddFilter(string filter)
    {
      AddFilter(filter, string.Empty);
    }
    public string[] ContextTargets
    {
      get
      {
        if (contextTargets != null)
        {
          return contextTargets.ToArray();
        }
        return new string[0];
      }
    }

    public string ToSearchString(string serverUrl)
    {
      QueryPacket packet = new QueryPacket(this);
      string oldQuery = this.Context.QueryText;

      serverUrl = serverUrl.Substring(0, serverUrl.Length - new Uri(serverUrl).PathAndQuery.Length);

      if (contextTargets != null && contextTargets.Count > 0)
      {
        StringBuilder queryBuilder = new StringBuilder(oldQuery);
        foreach (string target in contextTargets)
        {
          string fullTargetUrl = (StringUtil.EnsurePostfix('/', serverUrl) + StringUtil.RemovePrefix('/', target));

          fullTargetUrl = StringUtil.RemovePostfix('/', fullTargetUrl);
          queryBuilder.AppendFormat(" SITE:{0}", fullTargetUrl);
        }
        this.Context.QueryText = queryBuilder.ToString();
      }
      XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(QueryPacket), "urn:Microsoft.Search.Query");

      StringBuilder result = new StringBuilder();
      TextWriter writer = new StringWriter(result);
      serializer.Serialize(writer, packet);
      Context.QueryText = oldQuery;
      return result.ToString();
    }
  }

  public class QueryPacket
  {
    public QueryPacket()
    {
    }
    public QueryPacket(Query query)
    {
      this.Query = query;
    }

    public Query Query { get; set; }
  }



  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  //[System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial struct QuerySupportedFormats
  {

    private string[] formatField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Format")]
    public string[] Format
    {
      get
      {
        return this.formatField;
      }
      set
      {
        this.formatField = value;
      }
    }
  }

  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public class QueryContext
  {
    /// <remarks/>
    public string QueryText { get; set; }

    /// <remarks/>
    public string OriginatorContext { get; set; }
  }

  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class QueryRange
  {
    /// <remarks/>
    public int StartAt { get; set; }

    /// <remarks/>
    public int Count { get; set; }
  }
}