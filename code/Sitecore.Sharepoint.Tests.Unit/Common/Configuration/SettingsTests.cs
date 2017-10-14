// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SettingsTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Common.Configuration
{
  using FluentAssertions;

  using Ploeh.AutoFixture.Xunit;

  using Sitecore.Configuration;

  using Xunit;
  using Xunit.Extensions;

  public class SettingsTests
  {
    [Theory, AutoData]
    public void should_return_instance_name(string instanceName)
    {
      // Arrange
      using (new SettingsSwitcher("Sharepoint.IntegrationInstance", instanceName))
      {
        // Act
        var integrationInstance = Sharepoint.Common.Configuration.Settings.IntegrationInstance;

        // Assert
        integrationInstance.Should().Be(instanceName);
      }
    }

    [Fact]
    public void should_return_current_instance_name_if_empty()
    {
      // Arrange
      using (new SettingsSwitcher("Sharepoint.IntegrationInstance", string.Empty))
      {

        // Act
        var integrationInstance = Sharepoint.Common.Configuration.Settings.IntegrationInstance;

        // Assert
        integrationInstance.Should().Be(Settings.InstanceName);
      }
    }

    [Fact]
    public void should_return_current_instance_name_if_white_spaced()
    {
      // Arrange
      using (new SettingsSwitcher("Sharepoint.IntegrationInstance", string.Empty.PadLeft(5)))
      {
        // Act
        var integrationInstance = Sharepoint.Common.Configuration.Settings.IntegrationInstance;

        // Assert
        integrationInstance.Should().Be(Settings.InstanceName);
      }
    }
  }
}
