// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActionController.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the IActionController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Actions
{
  /// <summary>
  /// Interface for rebind data and hangle exception.
  /// </summary>
  public interface IActionController
  {
    /// <summary>
    /// This method calling if data was changed.
    /// </summary>
    void Rebind();

    /// <summary>
    /// This method calling if action is failed.
    /// </summary>
    /// <param name="e">
    /// The e     .
    /// </param>
    void ShowError(System.Exception e);
  }
}
