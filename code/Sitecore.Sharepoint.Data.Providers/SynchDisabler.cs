// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchDisabler.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SynchDisabler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers
{
  using Sitecore.Common;

  /// <summary>
  /// This class is disabling ability for updating any information from SharePoint to CMS database.
  /// </summary>
  public class SynchDisabler : Switcher<bool, SynchDisabler>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SynchDisabler"/> class.
    /// </summary>
    public SynchDisabler() : base(true)
    {
    }
  }
}
