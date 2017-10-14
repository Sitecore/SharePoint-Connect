// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ADFSWorkflowTests.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ADFSWorkflowTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Common.Authentication.Workflows
{
  using System.IdentityModel.Protocols.WSTrust;
  using System.Net;
  using System.ServiceModel.Security;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Sharepoint.Common.Authentication.Workflows;
  using Sitecore.Sharepoint.Common.Authentication.Workflows.Helpers;
  using Xunit;

  public class ADFSWorkflowTests : WorkflowTestsBase
  {
    private readonly TokenGetter tokenGetter = Substitute.For<TokenGetter>(new WSTrustChannelFactoryWrapper());
    private readonly CookieGetter cookieGetter = Substitute.For<CookieGetter>();

    public ADFSWorkflowTests()
    {
      WorkflowsDefaults.Instance.GetCookieHelper().Returns(this.cookieGetter);
      WorkflowsDefaults.Instance.GetTokenGetter().Returns(this.tokenGetter);
      WorkflowsDefaults.Instance.GetTrustVersion().Returns(TrustVersion.WSTrustFeb2005);
    }
   
    [Fact]
    public void ADFSWorkflow_should_return_CookieContainer()
    {
      // Arrange
      var urnAaa = "urn:aaa";
      var stsEndpoint = "sts";

      var cookieContainer = new CookieContainer();
      var requestSecurityTokenResponse = new RequestSecurityTokenResponse();
      var networkCredential = new NetworkCredential();
      this.tokenGetter.GetToken(stsEndpoint, TrustVersion.WSTrustFeb2005, Arg.Is<string>(x => x.StartsWith(urnAaa)), networkCredential).Returns(requestSecurityTokenResponse);
      this.cookieGetter.GetCookieOnPremises(Arg.Is<string>(x => x.StartsWith(urnAaa)), requestSecurityTokenResponse).Returns(cookieContainer);

      var workflow = new ADFSWorkflow(stsEndpoint);

      // Act
      CookieContainer retContainer = workflow.GetAuthenticationCookies(urnAaa, networkCredential);

      // Assert
      retContainer.Should().Be(cookieContainer);
    }

    [Fact]
    public void DefaulTrustPath_should_be_trust()
    {
      const string TrustPath = "/_trust";
      const string StsUrn = "urn:aaa";

      var workflow = new ADFSWorkflow(string.Empty);
      
      // Act
      workflow.GetAuthenticationCookies(StsUrn, new NetworkCredential());

      // Assert
      this.tokenGetter.Received().GetToken(Arg.Any<string>(), Arg.Any<TrustVersion>(), StsUrn + TrustPath, Arg.Any<NetworkCredential>());
      this.cookieGetter.Received().GetCookieOnPremises(StsUrn + TrustPath, Arg.Any<RequestSecurityTokenResponse>());
    }
  }
}
