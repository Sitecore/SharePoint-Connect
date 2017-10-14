// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowsDefaultsTests.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the WorkflowsDefaultsTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Common.Authentication.Workflows
{
  using System.ServiceModel.Security;
  using FluentAssertions;
  using Sitecore.Sharepoint.Common.Authentication.Workflows;
  using Sitecore.Sharepoint.Common.Authentication.Workflows.Helpers;
  using Xunit;

  public class WorkflowsDefaultsTests
  {
    [Fact]
    public void DefaulWsTrustHelper_should_be_WsTrustHelper()
    {
      // Act
      var tokenGetter = WorkflowsDefaults.Instance.GetTokenGetter();

      // Assert
      tokenGetter.Should().BeOfType<TokenGetter>();
    }

    [Fact]
    public void DefaulCookieHelper_should_be_CookieHelper()
    {
      // Act
      var cookieHelper = WorkflowsDefaults.Instance.GetCookieHelper();

      // Assert
      cookieHelper.Should().BeOfType<CookieGetter>();
    }

    [Fact]
    public void DefaultTrustVersion_should_be_WsTrust13()
    {
      // Arrange

      // Act
      var trustVersion = WorkflowsDefaults.Instance.GetTrustVersion();

      // Assert
      trustVersion.Should().Be(TrustVersion.WSTrust13);
    }

    [Fact]
    public void should_return_default_SharePointOnlineCredentialWrapper()
    {
      // Arrange

      // Act
      var credentialsWrapper = WorkflowsDefaults.Instance.GetSharePointOnlineCredentialsWrapper();

      // Assert
      credentialsWrapper.Should().BeOfType<SharePointOnlineCredentialsWrapper>();
    }
  }
}
