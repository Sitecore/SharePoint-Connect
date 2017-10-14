// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharePointOnlineWorkflowTests.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SharePointOnlineWorkflowTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Common.Authentication.Workflows
{
  using System;
  using System.Net;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Sharepoint.Common.Authentication.Workflows;
  using Sitecore.Sharepoint.Common.Authentication.Workflows.Helpers;
  using Xunit;

  [Obsolete]
  public class LegacySharePointOnlineWorkflowTests : WorkflowTestsBase
  {
    private const string Url = "http://some.url.net";
    private const string AuthCookie = "SPOIDCRL=SharePointOnlineAuthenticationCookieData";

    private readonly NetworkCredential credential = new NetworkCredential("UserName", "Password");

    private readonly SharePointOnlineCredentialsWrapper sharePointOnlineHelper = Substitute.For<SharePointOnlineCredentialsWrapper>();
    private readonly LegacySharePointOnlineWorkflow sharepointOnlineWorkflow;

    public LegacySharePointOnlineWorkflowTests()
    {
      this.sharePointOnlineHelper.GetAuthenticationCookie(Arg.Any<Uri>(), Arg.Any<NetworkCredential>()).Returns(AuthCookie);
      WorkflowsDefaults.Instance.GetSharePointOnlineCredentialsWrapper().Returns(this.sharePointOnlineHelper);

      this.sharepointOnlineWorkflow = new LegacySharePointOnlineWorkflow();
    }

    [Fact]
    public void should_replace_string_with_FedAuth_cookie()
    {
      // Arrange

      // Act
      var cookieContainer = this.sharepointOnlineWorkflow.GetAuthenticationCookies(Url, this.credential);

      // Assert
      this.GetCookieFromContainer(cookieContainer).Name.Should().Be("FedAuth");
    }

    [Fact]
    public void should_replace_SPOIDCRL_in_FedAuth_cookie()
    {
      // Arrange
      
      // Act
      var cookieContainer = this.sharepointOnlineWorkflow.GetAuthenticationCookies(Url, this.credential);
      
      // Assert
      this.GetCookieFromContainer(cookieContainer).Value.Should().NotStartWith("SPOIDCRL=");
    }

    [Fact]
    public void should_fill_FedAut_cookie_with_data_from_SPOIDCRL_cookie()
    {
      // Arrange
      
      // Act
      var cookieContainer = this.sharepointOnlineWorkflow.GetAuthenticationCookies(Url, this.credential);

      // Assert
      this.GetCookieFromContainer(cookieContainer).Value.Should().Be("SharePointOnlineAuthenticationCookieData");
    }

    [NotNull]
    private Cookie GetCookieFromContainer([NotNull] CookieContainer cookieContainer)
    {
      return cookieContainer.GetCookies(new Uri(Url))[0];
    }
  }
}
