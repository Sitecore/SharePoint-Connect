// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEditable.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the IEditable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities
{
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;

  /// <summary>
  /// Represents  method for update item.
  /// </summary>
  public interface IEditable
  {
    /// <summary>
    /// Gets a value indicating whether that instance of object is new.
    /// </summary>
    bool IsNew { get; }

    /// <summary>
    /// Update item in SharePoint.
    /// </summary>
    /// <returns>
    /// The result.
    /// </returns>
    [CanBeNull]
    EntityProperties Update();

    /// <summary>
    /// Delete item from SharePoint.
    /// </summary>
    [CanBeNull]
    void Delete();
  }
}
