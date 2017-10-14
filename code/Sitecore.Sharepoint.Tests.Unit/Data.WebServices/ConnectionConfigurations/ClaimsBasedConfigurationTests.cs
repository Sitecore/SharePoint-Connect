// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClaimsBasedConfigurationTests.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ClaimsBasedConfigurationTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.WebServices.ConnectionConfigurations
{
  using System;
  using System.Net;
  using System.Web.Services.Protocols;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Common.Authentication.Workflows;
  using Sitecore.Sharepoint.Common.Caches;
  using Sitecore.Sharepoint.Data.WebServices.ConnectionConfigurations;
  using Xunit;

  public class ClaimsBasedConfigurationTests : IDisposable
  {
    private readonly CookieContainer cookieContainer = Substitute.For<CookieContainer>();
    private readonly SpContextBase context = Substitute.For<SpContextBase>();
    private readonly ClaimsBasedConnectionConfiguration initializer;
    private readonly HttpWebClientProtocol service = Substitute.For<HttpWebClientProtocol>();
    private readonly ICookieContainerCache cache = Substitute.For<ICookieContainerCache>();
    private readonly ClaimsBasedWorkflow workflow = Substitute.For<ClaimsBasedWorkflow>();

    private readonly ConnectionConfigurationsDefaults prevInstance;

    public ClaimsBasedConfigurationTests()
    {
      this.prevInstance = ConnectionConfigurationsDefaults.Instance;
      ConnectionConfigurationsDefaults.Instance = Substitute.For<ConnectionConfigurationsDefaults>();

      this.workflow.GetAuthenticationCookies(null, null).ReturnsForAnyArgs(this.cookieContainer);

      this.context.Credentials.Returns(Substitute.For<NetworkCredential>("UserName", string.Empty, string.Empty));
      ConnectionConfigurationsDefaults.Instance.GetContainerCache().Returns(this.cache);

      this.initializer = new ClaimsBasedConnectionConfiguration(this.workflow);
    }

    [Fact]
    public void should_set_cookie_container()
    {
      // Arrange

      // Act
      this.initializer.Initialize(this.service, this.context);

      // Assert
      this.service.CookieContainer.Should().Be(this.cookieContainer);
    }

    [Fact]
    public void should_get_cookie_from_cache()
    {
      // Arrange
      var cachedCookieContainer = new CookieContainer();
      this.cache.GetUnexpired(Arg.Any<string>(), Arg.Any<NetworkCredential>()).Returns(cachedCookieContainer);

      // Act
      this.initializer.Initialize(this.service, this.context);

      // Assert
      this.service.CookieContainer.Should().Be(cachedCookieContainer);
    }

    [Fact]
    public void should_not_call_getcookie_if_they_are_in_cache()
    {
      // Arrange
      var cachedCookieContainer = new CookieContainer();
      this.cache.GetUnexpired(Arg.Any<string>(), Arg.Any<NetworkCredential>()).Returns(cachedCookieContainer);

      // Act
      this.initializer.Initialize(this.service, this.context);

      // Assert
      this.workflow.GetAuthenticationCookies(Arg.Any<string>(), Arg.Any<NetworkCredential>()).DidNotReceiveWithAnyArgs();
    }

    [Fact]
    public void should_throw_exception_when_user_name_is_not_specified()
    {
      // Arrange
      this.context.Credentials.Returns(Substitute.For<NetworkCredential>());
      
      // Act
      Action action = () => this.initializer.Initialize(this.service, this.context);

      // Assert
      action.ShouldThrow<Exception>().WithMessage("Claims-based authentication requires username");
    }

    [Fact]
    public void should_throw_exception_when_default_credentials_are_used()
    {
      // Arrange
      this.context.Credentials.Returns(CredentialCache.DefaultNetworkCredentials);

      // Act
      Action action = () => this.initializer.Initialize(this.service, this.context);

      // Assert
      action.ShouldThrow<Exception>().WithMessage("Default credentials isn't applicable for claims-based authentication");
    }

    public void Dispose()
    {
      ConnectionConfigurationsDefaults.Instance = this.prevInstance;
    }
  }
}
