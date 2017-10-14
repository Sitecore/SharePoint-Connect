// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseConnectionConfiguration.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the BaseConnectionConfiguration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.WebServices.ConnectionConfigurations
{
  using System.Web.Services.Protocols;
  using Sitecore.Sharepoint.Common;

  public abstract class BaseConnectionConfiguration
  {
    public abstract void Initialize(HttpWebClientProtocol service, SpContextBase context);
  }
}