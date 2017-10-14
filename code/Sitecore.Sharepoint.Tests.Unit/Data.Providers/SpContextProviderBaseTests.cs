// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpContextProviderBaseTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SpContextProviderBaseTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers
{
  using FluentAssertions;
  using NSubstitute;

  using Sitecore.Abstractions;
  using Sitecore.Sharepoint.Data.Providers;
  using Xunit;

  public class SpContextProviderBaseTests
  {
    [Fact]
    public void should_return_instance_from_factory()
    {
      // Arrange
      try
      {
        var instance = Substitute.For<SpContextProviderBase>();
        var factory = Substitute.For<BaseFactory>();
        factory.CreateObject("SharepointContextProvider", true).Returns(instance);
        SpContextProviderBase.Factory = factory;

        // Act && Assert
        SpContextProviderBase.Instance.Should().BeSameAs(instance);
      }
      finally
      {
        SpContextProviderBase.Factory = null;
      }
    }
  }
}
