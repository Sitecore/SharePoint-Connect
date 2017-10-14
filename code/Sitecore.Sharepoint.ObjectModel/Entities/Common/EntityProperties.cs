// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityProperties.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines EntityProperties class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Common
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Collections;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Represents properties of SharePoint entities.
  /// </summary>
  public class EntityProperties : SafeDictionary<string, EntityPropertyValue>
  {
    private Dictionary<string, string> metaInfoProperties;

    public EntityProperties()
    {
      this.metaInfoProperties = new Dictionary<string, string>();
    }

    /// <summary>
    /// Adds a property with the specified name and value.
    /// Overrides base method to avoid exception when try to add property which already exist.
    /// Such situation can happen when SharePoint field has attribute and property (subnode) with same names.
    /// </summary>
    /// <param name="key">
    /// The name of the property to add.
    /// </param>
    /// <param name="value">
    /// The value of the property to add.
    /// </param>
    public new void Add(string key, EntityPropertyValue value)
    {
      if (!this.ContainsKey(key))
      {
        base.Add(key, value);
      }
    }

    [CanBeNull]
    public string GetMetaInfoProperty([NotNull] string propertyName)
    {
      Assert.ArgumentNotNull(propertyName, "propertyName");

      string propertyValue;
      if (this.metaInfoProperties.TryGetValue(propertyName, out propertyValue))
      {
        return propertyValue;
      }

      propertyValue = this.LoadMetaInfoProperty(propertyName);
      this.metaInfoProperties.Add(propertyName, propertyValue);
      
      return propertyValue;
    }

    [CanBeNull]
    private string LoadMetaInfoProperty([NotNull] string propertyName)
    {
      Assert.ArgumentNotNull(propertyName, "propertyName");

      EntityPropertyValue metaInfoProperty = this["ows_MetaInfo"];
      if (metaInfoProperty == null || string.IsNullOrEmpty(metaInfoProperty.OldValue))
      {
        return null;
      }

      string searchValue = propertyName + ":";
      int startIndex = 0;
      if (!metaInfoProperty.OldValue.StartsWith(searchValue))
      {
        searchValue = Environment.NewLine + searchValue;
        startIndex = metaInfoProperty.OldValue.IndexOf(searchValue);
        if (startIndex == -1)
        {
          return null;
        }
      }

      startIndex += searchValue.Length;
      startIndex = metaInfoProperty.OldValue.IndexOf('|', startIndex);
      if (startIndex == -1)
      {
        return null;
      }

      startIndex++;
      int endIndex = metaInfoProperty.OldValue.IndexOf(Environment.NewLine, startIndex);
      if (endIndex == -1)
      {
        return null;
      }

      return metaInfoProperty.OldValue.Substring(startIndex, endIndex - startIndex);
    }
  }
}
