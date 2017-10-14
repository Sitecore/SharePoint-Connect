// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericKeyValurPairCollection.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the GenericKeyValurPairCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit
{
  using System.Collections;
  using System.Collections.Generic;

  internal class GenericKeyValurPairCollection<TKey, TValue> : IEnumerable
  {
    public GenericKeyValurPairCollection()
    {
      this.List = new List<KeyValuePair<TKey, TValue>>();
    }

    protected List<KeyValuePair<TKey, TValue>> List { get; set; }

    public IEnumerator GetEnumerator()
    {
      return this.List.GetEnumerator();
    }

    public virtual void Add(TKey key)
    {
      this.List.Add(new KeyValuePair<TKey, TValue>(key, default(TValue)));
    }

    public virtual void Add(TKey key, TValue value)
    {
      this.List.Add(new KeyValuePair<TKey, TValue>(key, value));
    }
  }
}