// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseCollection.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines BaseCollection class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Collections
{
  using System.Collections;
  using System.Collections.Generic;

  public abstract class BaseCollection<TEntity> : IEnumerable<TEntity>
  {
    public int Count
    {
      get
      {
        return this.GetEntities().Count;
      }
    }

    protected List<TEntity> Entities { get; set; }

    public TEntity this[int index]
    {
      get
      {
        return this.GetEntities()[index];
      }
    }

    #region Implementation of IEnumerable

    [NotNull]
    public IEnumerator<TEntity> GetEnumerator()
    {
      return this.GetEntities().GetEnumerator();
    }

    [NotNull]
    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion Implementation of IEnumerable

    [NotNull]
    protected abstract List<TEntity> GetEntities();
  }
}
