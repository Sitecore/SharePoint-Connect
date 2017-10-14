// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClaimsBasedConnectionConfiguration.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ClaimsBasedConnectionConfiguration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.WebServices.ConnectionConfigurations
{
  using System;
  using System.Net;
  using System.Web.Services.Protocols;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Common.Authentication.Workflows;
  using Sitecore.Sharepoint.Common.Caches;

  public class ClaimsBasedConnectionConfiguration : BaseConnectionConfiguration
  {
    private readonly ClaimsBasedWorkflow claimsBasedWorkflow;
    private readonly ICookieContainerCache cache;

    public ClaimsBasedConnectionConfiguration([NotNull] ClaimsBasedWorkflow claimsBasedWorkflow) : this(claimsBasedWorkflow, ConnectionConfigurationsDefaults.Instance.GetContainerCache(), new TimeSpan(0, 30, 0))
    {
      Assert.ArgumentNotNull(claimsBasedWorkflow, "claimsBasedWorkflow");
    }

    public ClaimsBasedConnectionConfiguration([NotNull] ClaimsBasedWorkflow claimsBasedWorkflow, [NotNull] ICookieContainerCache cache, TimeSpan cookieExpires)
    {
      Assert.ArgumentNotNull(claimsBasedWorkflow, "claimsBasedWorkflow");
      Assert.ArgumentNotNull(cache, "cache");
      
      this.claimsBasedWorkflow = claimsBasedWorkflow;
      this.cache = cache;
      this.CookiesExpirationInterval = cookieExpires;
    }

    protected TimeSpan CookiesExpirationInterval { get; set; }

    public override void Initialize([NotNull] HttpWebClientProtocol service, [NotNull] SpContextBase context)
    {
      Assert.ArgumentNotNull(service, "service");
      Assert.ArgumentNotNull(context, "context");

      if (string.IsNullOrEmpty(((NetworkCredential)context.Credentials).UserName))
      {
        string message = "Claims-based authentication requires username";
        if (context.Credentials == CredentialCache.DefaultNetworkCredentials)
        {
          message = "Default credentials isn't applicable for claims-based authentication";
        }
        
        throw new Exception(message);
      }
      
      var credential = (NetworkCredential)context.Credentials;
      var cookies = this.cache.GetUnexpired(context.Url, credential);
      if (cookies == null)
      {
        cookies = this.claimsBasedWorkflow.GetAuthenticationCookies(context.Url, credential);
        this.cache.Add(context.Url, credential, cookies, DateTime.Now.Add(this.CookiesExpirationInterval));
      }

      service.CookieContainer = cookies;
    }
  }
}