// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventSender.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the EventSender type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.Pipelines.ProcessIntegrationItem.Common
{
  public enum EventSender
  {
    /// <summary>
    /// Event sended from from Sitecore
    /// </summary>
    Sitecore,

    /// <summary>
    /// Event sended from from Sharepoint
    /// </summary>
    Sharepoint
  }
}
