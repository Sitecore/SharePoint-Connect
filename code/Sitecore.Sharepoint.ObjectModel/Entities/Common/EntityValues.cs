// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityValues.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines EntityValues class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Common
{
  using System.Collections.Generic;
  using Sitecore.Collections;
  using Sitecore.Diagnostics;

  public class EntityValues
  {
    public EntityValues()
    {
      this.Properties = new EntityProperties();
      this.Values = new SafeDictionary<string, List<EntityValues>>();
    }

    public EntityProperties Properties { get; set; }

    protected SafeDictionary<string, List<EntityValues>> Values { get; set; }

    [NotNull]
    public EntityValues[] this[string entityValueType]
    {
      get
      {
        return (this.Values[entityValueType] ?? new List<EntityValues>()).ToArray();
      }
    }

    public void Add([NotNull] string entityValueType, [NotNull] EntityValues entityValue)
    {
      Assert.ArgumentNotNull(entityValueType, "entityValueType");
      Assert.ArgumentNotNull(entityValue, "entityValue");

      if (this.Values[entityValueType] == null) 
      {
        this.Values[entityValueType] = new List<EntityValues>();
      }

      this.Values[entityValueType].Add(entityValue);
    }

    public void AddRange(string entityValueType, [NotNull] List<EntityValues> entityValueCollection)
    {
      if (this.Values[entityValueType] == null)
      {
        this.Values[entityValueType] = new List<EntityValues>();
      }

      this.Values[entityValueType].AddRange(entityValueCollection);
    }
  }
}
