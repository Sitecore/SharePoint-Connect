// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionConfigurationsFactoryTests.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ConnectionConfigurationsFactoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.WebServices
{
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Common.Wrappers;
  using Sitecore.Sharepoint.Data.WebServices;
  using Xunit;

  public class ConnectionConfigurationsFactoryTests
  {
    private SpContextBase context;
    private SitecoreFactory sitecoreFactory;
    private ConnectionConfigurationsFactory factory;

    public ConnectionConfigurationsFactoryTests()
    {
      this.context = Substitute.For<SpContextBase>();
      this.sitecoreFactory = Substitute.For<SitecoreFactory>();
      this.factory = new ConnectionConfigurationsFactory(this.sitecoreFactory);
    }

    [Fact]
    public void should_create_instance()
    {
      // Arrange
      this.context.ConnectionConfiguration.Returns("SomeType");
      
      // Act
      this.factory.CreateInstance(this.context);

      // Assert
      this.sitecoreFactory.Received().CreateObject("/sitecore/sharepoint/connectionConfigurations/SomeType", true);
    }

    [Fact]
    public void should_create_ClassicConfiguration_when_it_is_not_set()
    {
      // Act
      this.factory.CreateInstance(this.context);

      // Assert
      this.sitecoreFactory.Received().CreateObject("/sitecore/sharepoint/connectionConfigurations/Default", true);
    }

    [Fact]
    public void should_return_sigle_instance_ClassicConfiguration_when_it_is_not_set()
    {
      // Arrange
      this.context.ConnectionConfiguration.Returns(string.Empty);

      // Act
      var initializer = this.factory.CreateInstance(this.context);
      var initializer2 = this.factory.CreateInstance(this.context);

      // Assert
      initializer.Should().Be(initializer2);
    }
  }
}
