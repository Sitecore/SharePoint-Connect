// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlUtils.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines XmlUtils class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Utils
{
  using System.Collections.Generic;
  using System.Xml;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;

  /// <summary>
  /// This is util class to make work with xml easily.
  /// </summary>
  public class XmlUtils
  {
    /// <summary>
    /// Parse data from xml to StringDictionary.
    /// </summary>
    /// <param name="node">The sorce data.</param>
    /// <returns>Parsed data in StringDictionary</returns>
    [NotNull]
    public static EntityProperties LoadProperties([NotNull] XmlNode node)
    {
      Assert.ArgumentNotNull(node, "node");

      var properties = new EntityProperties();
      foreach (XmlAttribute attribute in node.Attributes)
      {
        properties.Add(attribute.Name, new EntityPropertyValue(CleanupProperyValue(attribute.Value)));
      }

      foreach (XmlNode childNode in node.ChildNodes)
      {
        properties.Add(childNode.Name, new EntityPropertyValue(childNode.InnerText));
      }

      return properties;
    }

    /// <summary>
    /// Cleanup value of propery.
    /// </summary>
    /// <param name="value">The value of propery.</param>
    /// <returns>Cleanup value of proprty.</returns>
    public static string CleanupProperyValue(string value)
    {
      int spacePosition = value.IndexOf(' ');
      int anchorPosition = value.IndexOf(";#");
      //All text should be removed before anchor but there should not be a space before anchor - to prevent distructing html
      return value.Substring(anchorPosition > 0 && (anchorPosition<spacePosition || spacePosition ==-1) ? value.IndexOf('#') + 1 : 0);
    }

    /// <summary>
    /// Return value from XmlNode by path.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="xpath">The xpath.</param>
    /// <returns>Selected data.</returns>
    public static XmlNodeList Select(XmlNode node, string xpath)
    {
      var manager = new XmlNamespaceManager(new NameTable());
      manager.AddNamespace("f", "http://schemas.microsoft.com/sharepoint/soap/");
      return node.SelectNodes(xpath, manager);
    }

    public static EntityValues GetEntityValues(XmlNode node)
    {
      var property = LoadProperties(node);

      var fields = new List<EntityValues>();
      var manager = new XmlNamespaceManager(new NameTable());
      manager.AddNamespace("f", "http://schemas.microsoft.com/sharepoint/soap/");
      foreach (XmlNode fieldXml in node.SelectNodes(".//f:Field", manager))
      {
        var field = new EntityValues()
        {
          Properties = LoadProperties(fieldXml)
        };

        fields.Add(field);
      }

      var values = new EntityValues();
      values.Properties = property;
      values.AddRange("Fields", fields);

      return values;
    }
  }
}
