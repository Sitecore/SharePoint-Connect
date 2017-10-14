// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenGetter.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the TokenGetter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Authentication.Workflows.Helpers
{
  using System.IdentityModel.Protocols.WSTrust;
  using System.Net;
  using System.ServiceModel;
  using System.ServiceModel.Security;
  using Sitecore.Diagnostics;

  public class TokenGetter
  {
    protected readonly WSTrustChannelFactoryWrapper ChannelFactoryWrapper;

    public TokenGetter([NotNull] WSTrustChannelFactoryWrapper channelFactoryWrapper)
    {
      Assert.ArgumentNotNull(channelFactoryWrapper, "channelFactoryWrapper");
      
      this.ChannelFactoryWrapper = channelFactoryWrapper;
    }

    [NotNull]
    public virtual RequestSecurityTokenResponse GetToken([NotNull] string stsEndpoint, [NotNull] TrustVersion trustVersion, [NotNull] string applyToUri, [NotNull] NetworkCredential credential)
    {
      Assert.ArgumentNotNull(stsEndpoint, "stsEndpoint");
      Assert.ArgumentNotNull(trustVersion, "trustVersion");
      Assert.ArgumentNotNull(applyToUri, "applyToUri");
      Assert.ArgumentNotNull(credential, "credential");
      
      var factory = this.ChannelFactoryWrapper.CreateFactory(trustVersion);
      try
      {
        this.InitFactory(factory, credential);
        var channel = factory.CreateChannel(new EndpointAddress(stsEndpoint));
        Assert.IsNotNull(channel, "Channel should not be null");
        RequestSecurityTokenResponse rstr;
        channel.Issue(this.GetRequestSecurityToken(applyToUri), out rstr);
        return rstr;
      }
      finally
      {
        this.ChannelFactoryWrapper.CloseFactory(factory);
      }
    }

    protected virtual void InitFactory([NotNull] ChannelFactory<IWSTrustChannelContract> factory, [NotNull] NetworkCredential credential)
    {
      Assert.ArgumentNotNull(factory, "factory");
      Assert.ArgumentNotNull(credential, "credential");
      
      factory.Endpoint.Binding = this.GetBinding();
      this.SetCredentials(factory, credential);
    }

    protected virtual void SetCredentials([NotNull] ChannelFactory<IWSTrustChannelContract> factory, [NotNull] NetworkCredential credential)
    {
      Assert.ArgumentNotNull(factory, "factory");
      Assert.ArgumentNotNull(credential, "credential");

      Assert.IsNotNull(factory.Credentials, "factory.Credentials should not be null");
      
      factory.Credentials.UserName.UserName = credential.UserName;
      factory.Credentials.UserName.Password = credential.Password;
    }

    [NotNull]
    protected virtual RequestSecurityToken GetRequestSecurityToken([NotNull] string applyToUri)
    {
      Assert.ArgumentNotNull(applyToUri, "applyToUri");
      
      return new RequestSecurityToken
      {
        RequestType = RequestTypes.Issue,
        AppliesTo = new EndpointReference(applyToUri),
        KeyType = KeyTypes.Bearer,
      };
    }

    [NotNull]
    protected virtual WSHttpBinding GetBinding()
    {
      var binding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
      binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
      binding.Security.Message.EstablishSecurityContext = false;
      binding.Security.Message.NegotiateServiceCredential = false;
      binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
      return binding;
    }
  }
}