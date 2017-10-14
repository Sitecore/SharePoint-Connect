// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharePointOnlineWorkflowTests.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the SharePointOnlineWorkflowTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Common.Authentication.Workflows
{
  using System;
  using System.Linq;
  using System.Net;
  using FluentAssertions;
  using NSubstitute;

  using Ploeh.AutoFixture.Xunit;

  using Sitecore.Sharepoint.Common.Authentication.Workflows;
  using Sitecore.Sharepoint.Common.Authentication.Workflows.Helpers;
  using Xunit.Extensions;

  public class SharePointOnlineWorkflowTests : WorkflowTestsBase
  {
    private readonly SharePointOnlineCredentialsWrapper sharePointOnlineHelper = Substitute.For<SharePointOnlineCredentialsWrapper>();
    private readonly SharePointOnlineWorkflow sharepointOnlineWorkflow;

    public SharePointOnlineWorkflowTests()
    {
      this.sharepointOnlineWorkflow = new SharePointOnlineWorkflow(this.sharePointOnlineHelper);
    }

    [Theory, AutoData]
    public void Should_Add_Cookies(Uri uri, NetworkCredential credential, string cookieName, string cookieValue)
    {
      // Arrange
      this.sharePointOnlineHelper.GetAuthenticationCookie(uri, credential).Returns(string.Format("{0}={1}", cookieName, cookieValue));

      // Act
      var cookieContainer = this.sharepointOnlineWorkflow.GetAuthenticationCookies(uri.ToString(), credential);

      // Assert
      var cookie = cookieContainer.GetCookies(uri).Cast<Cookie>().Single();
      cookie.Name.Should().Be(cookieName);
      cookie.Value.Should().Be(cookieValue);
    }
  }
}
