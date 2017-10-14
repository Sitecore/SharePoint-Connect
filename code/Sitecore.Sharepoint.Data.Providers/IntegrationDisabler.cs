// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationDisabler.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the IntegrationDisabler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers
{
  using Sitecore.Common;

  /// <summary>
  /// This class is disabling ability for starting synchronization between SharePoint and Sitecore.
  /// </summary>
  public class IntegrationDisabler : Switcher<bool, IntegrationDisabler>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationDisabler"/> class.
    /// </summary>
    public IntegrationDisabler() : base(true)
    {
    }
  }
}
