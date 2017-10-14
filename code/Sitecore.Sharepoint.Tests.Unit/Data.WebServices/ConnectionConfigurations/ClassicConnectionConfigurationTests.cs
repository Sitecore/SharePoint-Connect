// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassicConnectionConfigurationTests.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ClassicConnectionConfigurationTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.WebServices.ConnectionConfigurations
{
  using System.Net;
  using System.Web.Services.Protocols;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Data.WebServices.ConnectionConfigurations;
  using Xunit;

  public class ClassicConnectionConfigurationTests
  {
    private readonly ClassicConnectionConfiguration classicConfiguration;

    public ClassicConnectionConfigurationTests()
    {
      this.classicConfiguration = new ClassicConnectionConfiguration();
    }

    [Fact]
    public void should_fill_service_with_credentials()
    {
      // Arrange
      var credentials = Substitute.For<ICredentials>();
      var service = Substitute.For<HttpWebClientProtocol>();
      var context = Substitute.For<SpContextBase>();

      context.Credentials.Returns(credentials);

      // Act
      this.classicConfiguration.Initialize(service, context);

      // Assert
      service.Credentials.Should().Be(credentials);
    }

    [Fact]
    public void should_set_preauthenticate_flag()
    {
      // Arrange
      var service = Substitute.For<HttpWebClientProtocol>();
      var context = Substitute.For<SpContextBase>();
      
      // Act
      this.classicConfiguration.Initialize(service, context);

      // Assert
      service.PreAuthenticate.Should().BeTrue();
    }
  }
}
