// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ADFSWorkflow.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ADFSWorkflow type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Authentication.Workflows
{
  using System.Net;
  using System.ServiceModel.Security;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Authentication.Workflows.Helpers;

  public class ADFSWorkflow : ClaimsBasedWorkflow
  {
    public ADFSWorkflow([NotNull] string stsEndpoint) :
      this(stsEndpoint, WorkflowsDefaults.Instance.GetTrustVersion(), WorkflowsDefaults.Instance.GetTokenGetter(), WorkflowsDefaults.Instance.GetCookieHelper())
    {
      Assert.ArgumentNotNull(stsEndpoint, "stsEndpoint");
    }

    public ADFSWorkflow([NotNull] string stsEndpoint, [NotNull]TrustVersion trustVersion, [NotNull] TokenGetter tokenGetter, [NotNull] CookieGetter cookieGetter, string trustPath = "/_trust")
    {
      Assert.ArgumentNotNull(stsEndpoint, "stsEndpoint");
      Assert.ArgumentNotNull(trustVersion, "trustVersion");
      Assert.ArgumentNotNull(tokenGetter, "tokenGetter");
      Assert.ArgumentNotNull(cookieGetter, "cookieGetter");

      this.StsEndpoint = stsEndpoint;
      this.TrustPath = trustPath;
      this.TrustVersion = trustVersion;
      this.TokenGetter = tokenGetter;
      this.CookieGetter = cookieGetter;
    }

    [NotNull]
    protected string TrustPath { get; set; }

    [NotNull]
    protected string StsEndpoint { get; set; }

    [NotNull]
    protected TokenGetter TokenGetter { get; set; }

    [NotNull]
    protected CookieGetter CookieGetter { get; set; }

    [NotNull]
    protected TrustVersion TrustVersion { get; set; }

    [NotNull]
    public override CookieContainer GetAuthenticationCookies([NotNull] string url, [NotNull] NetworkCredential credential)
    {
      Assert.ArgumentNotNull(url, "url");
      Assert.ArgumentNotNull(credential, "credential");

      string trustUrl = url + this.TrustPath;
      var stsToken = this.TokenGetter.GetToken(this.StsEndpoint, this.TrustVersion, trustUrl, credential);
      return this.CookieGetter.GetCookieOnPremises(trustUrl, stsToken);
    }
  }
}