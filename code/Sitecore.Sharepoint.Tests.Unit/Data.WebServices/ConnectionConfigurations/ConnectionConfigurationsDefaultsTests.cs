// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionConfigurationsDefaultsTests.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ConnectionConfigurationsDefaultsTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.WebServices.ConnectionConfigurations
{
  using FluentAssertions;
  using Sitecore.Sharepoint.Common.Caches;
  using Sitecore.Sharepoint.Data.WebServices.ConnectionConfigurations;
  using Xunit;

  public class ConnectionConfigurationsDefaultsTests
  {
    [Fact]
    public void should_return_right_default_CookieContainerCache()
    {
      // Act
      var containerCache = ConnectionConfigurationsDefaults.Instance.GetContainerCache();

      // Assert
      containerCache.Should().BeOfType<CookieContainerCache>();
    }
  }
}
