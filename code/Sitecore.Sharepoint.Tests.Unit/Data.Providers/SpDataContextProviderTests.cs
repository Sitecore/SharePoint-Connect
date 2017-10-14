// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpDataContextProviderTests.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the SpDataContextProviderTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers
{
  using System;
  using System.Diagnostics.CodeAnalysis;
  using System.Net;
  using FluentAssertions;
  using NSubstitute;

  using Sitecore.Data;
  using Sitecore.Sharepoint.Common.Configuration;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.ObjectModel;
  using Xunit;
  using Xunit.Extensions;

  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
  public class SpDataContextProviderTests
  {
    private const string ConnectionConfiguration = "some connection configuration";

    private const string SharepointServerUrl = "http://someServer";

    public enum ContextType
    {
      /// <summary>The data context.</summary>
      Data,

      /// <summary>The UI context.</summary>
      Ui
    }

    [Theory]
    [InlineData(ContextType.Data)]
    [InlineData(ContextType.Ui)]
    public void should_fill_data_context(ContextType type)
    {
      // Arrange
      var provider = new SpContextProvider();
      var credentials = Substitute.For<ICredentials>();

      // Act
      var context = this.CreateContext(provider, type, SharepointServerUrl, string.Empty, credentials, ConnectionConfiguration);

      // Assert
      context.Url.Should().BeEquivalentTo(SharepointServerUrl);
      context.Credentials.Should().Be(credentials);
      context.ConnectionConfiguration.Should().Be(ConnectionConfiguration);
    }

    [Theory]
    [InlineData("", ContextType.Data)]
    [InlineData("", ContextType.Ui)]
    [InlineData(null, ContextType.Data)]
    [InlineData(null, ContextType.Ui)]
    public void should_use_default_current_server_if_url_is_not_set(string emptyUrl, ContextType type)
    {
      // Arrange
      var id = ID.NewID;
      const string CurrentServer = "current server";
      Context.Item = new ItemMock { { "Server", CurrentServer } };

      var provider = new SpContextProvider();

      // Act
      var context = this.CreateContext(provider, type, emptyUrl, string.Empty);

      // Assert
      context.Url.Should().Be(CurrentServer);
    }

    [Theory]
    [InlineData(ContextType.Data)]
    [InlineData(ContextType.Ui)]
    public void should_not_throw_if_url_is_not_correct(ContextType type)
    {
      // Arrange
      var provider = new SpContextProvider();

      // Act
      Action action = () => this.CreateContext(provider, type, "wrong url", string.Empty);

      // Assert
      action.ShouldNotThrow();
    }

    [Theory]
    [InlineData(ContextType.Data)]
    [InlineData(ContextType.Ui)]
    public void create_date_context_should_use_data_from_a_predefined_server(ContextType type)
    {
      // Arrange
      const string MyWeb = "MyWeb";
      var serverEntry = new ServerEntry(SharepointServerUrl + "/" + MyWeb, "UserName", "Password", type == ContextType.Data ? "Provider" : "Webcontrol", false, ConnectionConfiguration);
      using (new ServerEntrieCollectionSubstitute { serverEntry })
      {
        var provider = new SpContextProvider();

        // Act
        var context = this.CreateContext(provider, type, SharepointServerUrl, MyWeb);

        // Assert
        context.Credentials.Should().Be(serverEntry.Credentials);
        context.ConnectionConfiguration.Should().Be(serverEntry.ConnectionConfiguration);
      }
    }

    [Theory]
    [InlineData(ContextType.Data)]
    [InlineData(ContextType.Ui)]
    public void should_load_default_credentials_if_they_not_defined(ContextType type)
    {
      // Arrange
      var provider = new SpContextProvider();

      // Act
      var context = this.CreateContext(provider, type, SharepointServerUrl, string.Empty);

      // Assert
      context.Credentials.Should().Be(CredentialCache.DefaultNetworkCredentials);
    }

    [Fact]
    public void should_fill_data_context_from_config_data()
    {
      // Arrange
      var provider = new SpContextProvider();
      var configData = new IntegrationConfigData(SharepointServerUrl, "list", "template")
      {
        ConnectionConfiguration = ConnectionConfiguration,
      };
      configData.SetCredentials("UserName", "Password");

      // Act
      var context = provider.CreateDataContext(configData);

      // Assert
      context.ConnectionConfiguration.Should().Be(configData.ConnectionConfiguration);
      context.Credentials.Should().Be(configData.Credentials);
      context.Url.Should().BeEquivalentTo(configData.Server);
    }

    [Theory]
    [InlineData(ContextType.Data)]
    [InlineData(ContextType.Ui)]
    public void should_be_possible_to_set_default_credentials(ContextType type)
    {
      // Arrange
      var defaultCredentials = Substitute.For<ICredentials>();
      var provider = new SpContextProvider
      {
        DefaultCredentials = defaultCredentials
      };

      // Act
      var context = this.CreateContext(provider, type, SharepointServerUrl, string.Empty);

      // Assert
      context.Credentials.Should().Be(defaultCredentials);
    }

    /// <summary>Creates UI or Data context depend on parameters.</summary>
    /// <param name="provider">The provider.</param>
    /// <param name="type">The type.</param>
    /// <param name="serverUrl">The server url.</param>
    /// <param name="web">The web.</param>
    /// <param name="credentials">The credentials.</param>
    /// <param name="connectionConfiguration">The connection configuration.</param>
    /// <returns>The <see cref="SpContext"/>.</returns>
    private SpContext CreateContext(SpContextProvider provider, ContextType type, string serverUrl, string web, ICredentials credentials = null, string connectionConfiguration = null)
    {
      switch (type)
      {
        case ContextType.Data:
          return provider.CreateDataContext(serverUrl, web, credentials, connectionConfiguration);
        case ContextType.Ui:
          return provider.CreateUIContext(serverUrl, web, credentials, connectionConfiguration);
        default:
          throw new InvalidOperationException("Unknown context type");
      }
    }
  }
}
