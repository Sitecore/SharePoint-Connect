// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessIntegrationItemsOptions.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ProcessIntegrationItemsOptions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.Options
{
  public class ProcessIntegrationItemsOptions
  {
    /// <summary>
    /// Gets or sets a value indicating whether is force.
    /// This property is indicating that behavior should process items. Information about expirations should be ignored id value is set to true.
    /// </summary>
    public bool Force { get; set; }

    public bool ScheduledBlobTransfer { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is recursive.
    /// This property is indicating that behavior should process all nested items.
    /// </summary>
    public bool Recursive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether async integration can be used.
    /// </summary>
    /// <value><c>true</c> if async integration can be used; otherwise, <c>false</c>.</value>
    public bool AsyncIntegration { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether process is called from event handler.
    /// </summary>
    public bool IsEvent { get; set; }

    [NotNull]
    public static ProcessIntegrationItemsOptions DefaultOptions
    {
      get
      {
        return new ProcessIntegrationItemsOptions
        {
          Force = false,
          ScheduledBlobTransfer = false, 
          Recursive = false,
          AsyncIntegration = false,
          IsEvent = false
        };
      }
    }

    [NotNull]
    public static ProcessIntegrationItemsOptions DefaultAsyncOptions
    {
      get
      {
        var defaultOptions = DefaultOptions;
        defaultOptions.AsyncIntegration = true;
        return defaultOptions;
      }
    }
  }
}
