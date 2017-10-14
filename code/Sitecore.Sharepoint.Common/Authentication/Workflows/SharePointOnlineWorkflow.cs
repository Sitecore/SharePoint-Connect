// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharePointOnlineWorkflow.cs" company="Sitecore A/S">
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

  public class SharePointOnlineWorkflow : ClaimsBasedWorkflow
  {
    private readonly SharePointOnlineCredentialsWrapper sharePointOnlineCredentialsWrapper;

    public SharePointOnlineWorkflow()
      : this(WorkflowsDefaults.Instance.GetSharePointOnlineCredentialsWrapper())
    {
    }

    public SharePointOnlineWorkflow([NotNull] SharePointOnlineCredentialsWrapper sharePointOnlineCredentialsWrapper)
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
      cookieContainer.SetCookies(serverUri, authenticationCookie);

      return cookieContainer;
    }
  }
}