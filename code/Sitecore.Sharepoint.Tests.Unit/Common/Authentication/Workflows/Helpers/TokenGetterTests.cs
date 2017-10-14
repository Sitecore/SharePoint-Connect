// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenGetterTests.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the TokenGetterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Common.Authentication.Workflows.Helpers
{
  using System;
  using System.IdentityModel.Protocols.WSTrust;
  using System.Net;
  using System.ServiceModel;
  using System.ServiceModel.Description;
  using System.ServiceModel.Security;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Sharepoint.Common.Authentication.Workflows.Helpers;
  using Xunit;

  public class TokenGetterTests
  {
    private const string UrnStsendpoint = "urn:stsEndpoint";
    private const string ApplyToUri = "urn:123";
    private readonly NetworkCredential credential = new NetworkCredential("userName", "Password");
    private readonly ChannelFactory<IWSTrustChannelContract> channelFactory = Substitute.For<ChannelFactory<IWSTrustChannelContract>>(new ServiceEndpoint(new ContractDescription("fake")));
    private readonly IWSTrustChannelContract channel = Substitute.For<IWSTrustChannelContract>();
    private readonly TokenGetter tokenGetter;
    private readonly WSTrustChannelFactoryWrapper channelFactoryWrapper = Substitute.For<WSTrustChannelFactoryWrapper>();
    private RequestSecurityTokenResponse requestSecurityTokenResponse;

    public TokenGetterTests()
    {
      this.channelFactoryWrapper.CreateFactory(TrustVersion.Default).ReturnsForAnyArgs(this.channelFactory);
      this.tokenGetter = new TokenGetter(this.channelFactoryWrapper);

      this.channel.Issue(Arg.Any<RequestSecurityToken>(), out this.requestSecurityTokenResponse).Returns(x =>
      {
        x[1] = new RequestSecurityTokenResponse();
        return null;
      });
      this.channelFactory.CreateChannel(new EndpointAddress(UrnStsendpoint)).Returns(this.channel);
    }

    [Fact]
    public void should_set_username()
    {
      // Act
      this.tokenGetter.GetToken(UrnStsendpoint, TrustVersion.Default, ApplyToUri, this.credential);

      // Assert
// ReSharper disable PossibleNullReferenceException
      this.channelFactory.Credentials.UserName.UserName.Should().Be(this.credential.UserName);
// ReSharper restore PossibleNullReferenceException
    }

    [Fact]
    public void should_set_password()
    {
      // Act
      this.tokenGetter.GetToken(UrnStsendpoint, TrustVersion.Default, ApplyToUri, this.credential);

      // Assert
// ReSharper disable PossibleNullReferenceException
      this.channelFactory.Credentials.UserName.Password.Should().Be(this.credential.Password);
// ReSharper restore PossibleNullReferenceException
    }

    [Fact]
    public void should_set_right_binding()
    {
      // Act
      this.tokenGetter.GetToken(UrnStsendpoint, TrustVersion.Default, ApplyToUri, this.credential);

      // Assert
      this.channelFactory.Endpoint.Binding.Should().Match<WSHttpBinding>(
        x =>
          x.Security.Mode == SecurityMode.TransportWithMessageCredential &&
            x.Security.Message.ClientCredentialType == MessageCredentialType.UserName &&
            x.Security.Message.EstablishSecurityContext == false &&
            x.Security.Message.NegotiateServiceCredential == false &&
            x.Security.Transport.ClientCredentialType == HttpClientCredentialType.None);
    }

    [Fact]
    public void should_call_CreateChannel_with_stsEndpoint()
    {
      // Act
      this.tokenGetter.GetToken(UrnStsendpoint, TrustVersion.Default, ApplyToUri, this.credential);

      // Assert
      this.channelFactory.Received().CreateChannel(new EndpointAddress(UrnStsendpoint));
    }

    [Fact]
    public void should_match_request_token()
    {
      // Arrange
      RequestSecurityToken requestSecurityToken = null;
      this.channel.Issue(
        Arg.Do<RequestSecurityToken>(x => requestSecurityToken = x),
        out this.requestSecurityTokenResponse);

      // Act
      this.tokenGetter.GetToken(UrnStsendpoint, TrustVersion.Default, ApplyToUri, this.credential);

      // Assert
      requestSecurityToken.Should().Match<RequestSecurityToken>(
        x =>
          x.RequestType == RequestTypes.Issue
            && x.AppliesTo.Uri == new Uri(ApplyToUri)
            && x.KeyType == KeyTypes.Bearer);
    }

    [Fact]
    public void should_set_trust_version()
    {
      // Arrange

      // Act
      this.tokenGetter.GetToken(UrnStsendpoint, TrustVersion.WSTrustFeb2005, ApplyToUri, this.credential);

      // Assert
      this.channelFactoryWrapper.Received().CreateFactory(TrustVersion.WSTrustFeb2005);
    }

    [Fact]
    public void should_close_factory()
    {
      // Arrange

      // Act
      this.tokenGetter.GetToken(UrnStsendpoint, TrustVersion.Default, ApplyToUri, this.credential);

      // Assert
      this.channelFactoryWrapper.Received().CloseFactory(this.channelFactory);
    }
  }
}