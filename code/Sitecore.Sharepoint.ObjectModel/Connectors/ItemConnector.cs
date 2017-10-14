// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemConnector.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Provide methods for work with ListItems.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Connectors
{
  using System;
  using System.Net;
  using System.Web;
  using System.Xml;
  using Diagnostics;
  using Sharepoint.Data.WebServices.SharepointLists;
  using Sitecore.Sharepoint.Data.WebServices;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Utils;
  using StringExtensions;

  /// <summary>
  /// Provide methods for work with ListItems.
  /// </summary>
  public class ItemConnector
  {
    /// <summary>
    /// Template of command  for deleting item.
    /// </summary>
    protected const string DeleteCommand = "<Batch OnError=\"Continue\"><Method ID=\"1\" Cmd=\"Delete\">" + 
                                           "<Field Name=\"ID\">{0}</Field>" + 
                                           "<Field Name=\"FileRef\">{1}</Field>" + 
                                           "</Method></Batch>";

    protected readonly Lists ListsWebService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemConnector"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="webUrl">The web URL.</param>
    /// <exception cref="Exception"><c>Throws exception if connector can't be created.</c>.</exception>
    public ItemConnector([NotNull] SpContext context, [NotNull] Uri webUrl)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");

      try
      {
        this.ListsWebService = new Lists();
        this.ListsWebService.SetServer(webUrl, context);
      }
      catch (Exception ex)
      {
        string errorMessage = "List item connector can't be created. Additional info:\n Server URL: {0}, webUrl: {1} ".FormatWith(context.Url, webUrl);
        Log.Error(errorMessage, ex, this);

        throw new Exception(errorMessage, ex);
      }
    }

    /// <summary>
    /// Delete instance of list item in SharePoint.
    /// </summary>
    /// <param name="properties">The property.</param>
    /// <param name="listName">Name of the list.</param>
    public void DeleteItem([NotNull] EntityProperties properties, [NotNull] string listName)
    {
      Assert.ArgumentNotNull(properties, "properties");
      Assert.ArgumentNotNull(listName, "listName");

      var doc = new XmlDocument();
      doc.LoadXml(String.Format(DeleteCommand, properties["ows_ID"].Value, properties["ows_FileRef"].Value));

      XmlNode result = this.ListsWebService.UpdateListItems(listName, doc);
      this.CheckCorrectUpdate(doc, result);
    }

    /// <summary>
    /// Update instance of ListItem to SharePoint.
    /// </summary>
    /// <param name="properties">The property.</param>
    /// <param name="listName">Name of the list.</param>
    /// <returns>New EntityProperties value.</returns>
    [NotNull]
    public EntityProperties UpdateItem([NotNull] EntityProperties properties, [NotNull] string listName)
    {
      Assert.ArgumentNotNull(properties, "properties");
      Assert.ArgumentNotNull(listName, "listName");

      XmlDocument doc = this.GetUpdateCommand(properties);

      if (doc != null)
      {
        XmlNode result = this.ListsWebService.UpdateListItems(listName, doc);

        try
        {
          this.CheckCorrectUpdate(doc, result);
        }
        catch (Exception)
        {
        }

        var xmlNamespaceManager = new XmlNamespaceManager(result.OwnerDocument.NameTable);
        xmlNamespaceManager.AddNamespace(result.Prefix, result.NamespaceURI);

        XmlNode data = result.SelectSingleNode("/*[local-name()='Result']/*[local-name()='row']");
        if (data != null)
        {
          return XmlUtils.LoadProperties(data);
        }
      }

      return properties;
    }

    /// <summary>
    /// Generate CAML script for update or create item in SharePoint.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>CAML script.</returns>
    [CanBeNull]
    protected virtual XmlDocument GetUpdateCommand([NotNull] EntityProperties property)
    {
      Assert.ArgumentNotNull(property, "property");

      var doc = new XmlDocument();

      XmlElement elementBatch = doc.CreateElement("Batch");
      elementBatch.SetAttribute("OnError", "Continue");
      elementBatch.SetAttribute("ListVersion", "0");
      elementBatch.SetAttribute("PreCalc", "TRUE");

      XmlElement elementMethod = doc.CreateElement("Method");
      elementMethod.SetAttribute("ID", "1");
      elementMethod.SetAttribute("Cmd", property["ows_ID"].IsNew ? "New" : "Update");

      elementBatch.PrependChild(elementMethod);

      XmlElement elementField = doc.CreateElement("Field");
      elementField.SetAttribute("Name", "ID");
      elementField.InnerXml = property["ows_ID"].Value;

      elementMethod.PrependChild(elementField);

      bool needUpdate = false;

      foreach (var key in property.Keys)
      {
        // TODO: [101209 dan] Research what is the difference between "empty" and "null" values of SharePoint fields. 
        // TODO: [101209 dan] Research in correct way to update SharePopint fields. 
        // We can't rely on "this.PropertiesCopy" property to find previously existed SharePoint properties, 
        // because "this.PropertiesCopy" does not contain all properties.
        if (property[key].IsChanged || property[key].IsNew)
        {
          elementField = doc.CreateElement("Field");
          elementField.SetAttribute("Name", key.Replace("ows_", string.Empty));
          elementField.InnerXml = HttpUtility.HtmlEncode(property[key].Value);
          elementMethod.PrependChild(elementField);
          needUpdate = true;
        }
      }

      doc.PrependChild(elementBatch);
      if (needUpdate)
      {
        return doc;
      }

      return null;
    }

    /// <summary>
    /// Check that update was succeed.
    /// </summary>
    /// <param name="source">The source of CAML script.</param>
    /// <param name="result">The result.</param>
    /// <returns>Value indicate that update was succeed.</returns>
    /// <exception cref="Exception"><c>Exception</c>.</exception>
    protected bool CheckCorrectUpdate([NotNull] XmlDocument source, [NotNull] XmlNode result)
    {
      Assert.ArgumentNotNull(source, "source");
      Assert.ArgumentNotNull(result, "result");

      bool sucess = false;
      string strErrorCode = null;

      result = result.FirstChild;
      result = result.FirstChild;
      if (result.Name == "ErrorCode")
      {
        strErrorCode = result.InnerXml;
        long lngErrorCode;
        if (long.TryParse(strErrorCode.Replace("0x", string.Empty), out lngErrorCode))
        {
          if (lngErrorCode == 0)
          {
            sucess = true;
          }
        }
      }

      if (!sucess)
      {
        Log.Error(
          "UpdateListItems method returned an error. Error Code = " + strErrorCode +
            ". Command = [" + source.OuterXml +
              "]. UpdateListItems method response = [" + result.OuterXml + "]",
          this);

        throw new Exception("UpdateListItems method returned an error. Error Code = " + strErrorCode + ". Command = [" + source.OuterXml + "]. UpdateListItems method response = [" + result.OuterXml + "]");
      }

      return true;
    }
  }
}
