// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemCollectionConnector.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ItemCollectionConnector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Connectors
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Web;
  using System.Xml;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Configuration;
  using Sitecore.Sharepoint.Data.WebServices;
  using Sitecore.Sharepoint.Data.WebServices.SharepointLists;
  using Sitecore.Sharepoint.ObjectModel.Entities;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;
  using Sitecore.Sharepoint.ObjectModel.Options;
  using Sitecore.Sharepoint.ObjectModel.Utils;

  /// <summary>
  /// Provides possibility to work with sets of SharePoint list items.
  /// </summary>
  public class ItemCollectionConnector
  {
    /// <summary>
    /// Defines SharePoint web service which provides possibility to work with SharePoint lists.
    /// </summary>
    protected readonly Lists ListWebService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemCollectionConnector"/> class.
    /// </summary>
    /// <param name="context">The specified context.</param>
    /// <param name="webUrl">The specified web URL.</param>
    public ItemCollectionConnector([NotNull] SpContext context, [NotNull] Uri webUrl)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");

      this.ListWebService = new Lists();
      this.ListWebService.SetServer(webUrl, context);
    }

    /// <summary>
    /// Gets an object that contains information about list items from the specified SharePoint list.
    /// </summary>
    /// <param name="list">The specified SharePoint list.</param>
    /// <param name="options">Specifies which list items should be retrived from the specified SharePoint list.</param>
    /// <returns>
    /// An object that contains information about list items from the specified SharePoint list.
    /// </returns>
    [NotNull]
    public EntityValues GetItems([NotNull] BaseList list, [NotNull] ItemsRetrievingOptions options)
    {
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(options, "options");

      var optionsParser = new ItemsRetrievingOptionsParser(list, options);

      XmlNode itemsNode = this.ListWebService.GetListItems(
                                                           optionsParser.GetListName(),
                                                           optionsParser.GetViewName(),
                                                           optionsParser.GetQuery(),
                                                           optionsParser.GetViewFields(),
                                                           optionsParser.GetRowLimit(),
                                                           optionsParser.GetQueryOptions(),
                                                           optionsParser.GetWebID());

      var items = new EntityValues();

      XmlNode valuesNode = itemsNode.ChildNodes[1];
      if (valuesNode.Attributes["ListItemCollectionPositionNext"] != null)
      {
        items.Properties.Add("ListItemCollectionPositionNext", new EntityPropertyValue(valuesNode.Attributes["ListItemCollectionPositionNext"].Value));
      }

      foreach (XmlNode itemNode in valuesNode.ChildNodes)
      {
        if (itemNode.Name == "z:row")
        {
          var item = new EntityValues
          {
            Properties = XmlUtils.LoadProperties(itemNode)
          };

          items.Add("Items", item);
        }
      }

      return items;
    }

    /// <summary>
    /// Converts values of the specified <c>ItemsRetrievingOptions</c> to values that
    /// List web service requests to retrieve appropriate list items of the specified SharePoint list.
    /// </summary>
    private class ItemsRetrievingOptionsParser
    {
      /// <summary>
      /// Specifies the target SharePoint list.
      /// </summary>
      private readonly BaseList list;

      /// <summary>
      /// Indicates which list items should be retrived from the target SharePoint list.
      /// </summary>
      private readonly ItemsRetrievingOptions options;

      /// <summary>
      /// Initializes a new instance of the <see cref="ItemsRetrievingOptionsParser"/> class.
      /// </summary>
      /// <param name="list">The target SharePoint list.</param>
      /// <param name="options">Indicates which list items should be retrived from the target SharePoint list.</param>
      public ItemsRetrievingOptionsParser([NotNull] BaseList list, [NotNull] ItemsRetrievingOptions options)
      {
        Assert.ArgumentNotNull(list, "list");
        Assert.ArgumentNotNull(options, "options");

        this.list = list;
        this.options = options;
      }

      /// <summary>
      /// Gets the GUID of the target SharePoint list.
      /// </summary>
      /// <returns>The GUID of the target SharePoint list.</returns>
      [NotNull]
      public string GetListName()
      {
        return this.list.ID;
      }

      /// <summary>
      /// Gets the GUID of the specified SharePoint view.
      /// </summary>
      /// <returns>The GUID of SharePoint view if the specified options define it; otherwise, <c>null</c>.</returns>
      [CanBeNull]
      public string GetViewName()
      {
        return !string.IsNullOrEmpty(this.options.ViewName) ? this.options.ViewName : null;
      }

      /// <summary>
      /// Gets a <c>System.Xml.XmlNode</c> containing the query that determines which list items should be retrieved and in what order from the target SharePoint list.
      /// </summary>
      /// <returns>
      /// A <c>System.Xml.XmlNode</c> containing the query that determines which list items should be retrieved and in what order 
      /// if the specified options define at least one of this options; otherwise, <c>null</c>.
      /// </returns>
      [CanBeNull]
      public XmlNode GetQuery()
      {
        var xmlDocument = new XmlDocument();

        XmlNode queryElement = xmlDocument.CreateNode(XmlNodeType.Element, "Query", string.Empty);

        this.AddFilter(queryElement);

        this.AddSortOrder(queryElement);

        return queryElement.ChildNodes.Count > 0 ? queryElement : null;
      }

      /// <summary>
      /// Gets a <c>System.Xml.XmlNode</c> that specifies which fields of list item should be retrieved and in what order from the target SharePoint list.
      /// </summary>
      /// <returns>
      /// A <c>System.Xml.XmlNode</c> that specifies which fields of list item should be retrieved and in what order 
      /// if the specified options define that all fields should be retrieved; otherwise, <c>null</c>.
      /// </returns>
      [CanBeNull]
      public XmlNode GetViewFields()
      {
        var xmlDocument = new XmlDocument();

        XmlNode viewFieldsElement = xmlDocument.CreateNode(XmlNodeType.Element, "ViewFields", string.Empty);

        var view = this.list.Views.FirstOrDefault(v => v.Name == this.GetViewName());
        
        List<string> fieldNames = view != null
          ? view.FieldNames.Union(Settings.RequiredFields).ToList()
          : this.list.Fields.Select(x => x.Name).ToList();

        foreach (string fieldName in fieldNames)
        {
          XmlNode fieldRefElement = xmlDocument.CreateNode(XmlNodeType.Element, "FieldRef", string.Empty);

          XmlAttribute fieldNameAttribute = xmlDocument.CreateAttribute("Name");
          fieldNameAttribute.Value = fieldName;

          fieldRefElement.Attributes.Append(fieldNameAttribute);

          viewFieldsElement.AppendChild(fieldRefElement);
        }

        return viewFieldsElement;
      }

      /// <summary>
      /// Gets a value that specifies the number of list items that should be retrieved from the target SharePoint list.
      /// </summary>
      /// <returns>
      /// A value that specifies the number of list items that should be retrieved from the target SharePoint list 
      /// if the specified options define it; otherwise, <c>null</c>.
      /// </returns>
      [CanBeNull]
      public string GetRowLimit()
      {
        return this.options.ItemLimit > 0 ? this.options.ItemLimit.ToString() : null;
      }

      /// <summary>
      /// Gets a <c>System.Xml.XmlNode</c> that contains separate nodes for the various query options.
      /// </summary>
      /// <returns>
      /// Gets a <c>System.Xml.XmlNode</c> that contains separate nodes for the various query options 
      /// if the specified options define at least one of them; otherwise, <c>null</c>.
      /// </returns>
      [CanBeNull]
      public XmlNode GetQueryOptions()
      {
        var xmlDocument = new XmlDocument();

        XmlNode queryOptionsElement = xmlDocument.CreateNode(XmlNodeType.Element, "QueryOptions", string.Empty);

        queryOptionsElement.InnerXml = "<DateInUtc>FALSE</DateInUtc>";

        if (!string.IsNullOrEmpty(this.options.Folder))
        {
          queryOptionsElement.InnerXml += "<Folder>" + Common.Utils.StringUtil.ComposeSharepointPath(this.list.RootFolder, this.options.Folder) + "</Folder>";
        }

        if (!string.IsNullOrEmpty(this.options.PagingQuery))
        {
          queryOptionsElement.InnerXml += "<Paging ListItemCollectionPositionNext=\"" + HttpUtility.HtmlEncode(this.options.PagingQuery) + "\" />";
        }

        return !string.IsNullOrEmpty(queryOptionsElement.InnerXml) ? queryOptionsElement : null;
      }

      /// <summary>
      /// Gets the ID of the specified SharePoint web.
      /// </summary>
      /// <returns>Always <c>null</c>.</returns>
      [CanBeNull]
      public string GetWebID()
      {
        return null;
      }

      /// <summary>
      /// Adds the 'Where' clause that determines which list items should be retrieved from the target SharePoint list to the specified query.
      /// </summary>
      /// <param name="queryElement">The specified query.</param>
      private void AddFilter([NotNull] XmlNode queryElement)
      {
        Assert.ArgumentNotNull(queryElement, "queryElement");

        var xmlDocument = queryElement.OwnerDocument;

        if (!string.IsNullOrEmpty(this.options.WherePart))
        {
          XmlNode whereElement = xmlDocument.CreateNode(XmlNodeType.Element, "Where", string.Empty);
          whereElement.InnerXml = this.options.WherePart;

          queryElement.AppendChild(whereElement);
        }
      }

      /// <summary>
      /// Adds the 'OrderBy' clause that determines in what order list items should be retrieved from the target SharePoint list to the specified query.
      /// </summary>
      /// <param name="queryElement">The specified query.</param>
      private void AddSortOrder([NotNull] XmlNode queryElement)
      {
        Assert.ArgumentNotNull(queryElement, "queryElement");

        if (string.IsNullOrEmpty(this.options.SortingInfo.FieldName))
        {
          return;
        }

        var xmlDocument = queryElement.OwnerDocument;

        XmlNode orderByElement = xmlDocument.CreateNode(XmlNodeType.Element, "OrderBy", string.Empty);

        XmlNode fieldRefElement = xmlDocument.CreateNode(XmlNodeType.Element, "FieldRef", string.Empty);

        XmlAttribute fieldNameAttribute = xmlDocument.CreateAttribute("Name");
        fieldNameAttribute.Value = this.options.SortingInfo.FieldName;

        fieldRefElement.Attributes.Append(fieldNameAttribute);

        XmlAttribute sortOrderAttribute = xmlDocument.CreateAttribute("Ascending");
        sortOrderAttribute.Value = this.options.SortingInfo.Ascending ? "TRUE" : "FALSE";

        fieldRefElement.Attributes.Append(sortOrderAttribute);

        orderByElement.AppendChild(fieldRefElement);

        queryElement.AppendChild(orderByElement);
      }
    }
  }
}
