// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClaimsBasedWorkflow.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ClaimsBasedWorkflow type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Authentication.Workflows
{
  using System.Net;

  public abstract class ClaimsBasedWorkflow
  {
    public abstract CookieContainer GetAuthenticationCookies(string url, NetworkCredential credential);
  }
}