// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharePointOnlineCredentialsWrapper.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SharePointOnlineCredentialsWrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Authentication.Workflows.Helpers
{
  using System;
  using System.Net;
  using Microsoft.SharePoint.Client;
  using Sitecore.Diagnostics;

  public class SharePointOnlineCredentialsWrapper
  {
    [NotNull]
    public virtual string GetAuthenticationCookie([NotNull] Uri serverUri, [NotNull] NetworkCredential credential)
    {
      Assert.ArgumentNotNull(serverUri, "serverUri");
      Assert.ArgumentNotNull(credential, "credential");
      
      var credentials = new SharePointOnlineCredentials(credential.UserName, credential.SecurePassword);
      return credentials.GetAuthenticationCookie(serverUri);
    }
  }
}