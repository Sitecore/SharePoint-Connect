// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LegacySharePointOnlineWorkflow.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the SharePointOnlineWorkflow type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Authentication.Workflows
{
  using System;
  using System.Net;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Authentication.Workflows.Helpers;

  [Obsolete]
  public class LegacySharePointOnlineWorkflow : ClaimsBasedWorkflow
  {
    private readonly SharePointOnlineCredentialsWrapper sharePointOnlineCredentialsWrapper;

    public LegacySharePointOnlineWorkflow() : this(WorkflowsDefaults.Instance.GetSharePointOnlineCredentialsWrapper())
    {
    }

    public LegacySharePointOnlineWorkflow([NotNull] SharePointOnlineCredentialsWrapper sharePointOnlineCredentialsWrapper)
    {
      Assert.ArgumentNotNull(sharePointOnlineCredentialsWrapper, "sharePointOnlineCredentialsWrapper");
      
      this.sharePointOnlineCredentialsWrapper = sharePointOnlineCredentialsWrapper;
    }

    [NotNull]
    public override CookieContainer GetAuthenticationCookies([NotNull] string url, [NotNull] NetworkCredential credential)
    {
      Assert.ArgumentNotNull(url, "url");
      Assert.ArgumentNotNull(credential, "credential");
      
      var serverUri = new Uri(url);
      var authenticationCookie = this.sharePointOnlineCredentialsWrapper.GetAuthenticationCookie(serverUri, credential);

      var cookieContainer = new CookieContainer();
      cookieContainer.Add(new Cookie("FedAuth", authenticationCookie.Substring("SPOIDCRL=".Length), string.Empty, serverUri.Authority));

      return cookieContainer;
    }
  }
}