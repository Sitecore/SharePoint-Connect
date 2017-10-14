// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateExpirationIntervalTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the UpdateExpirationIntervalTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.Pipelines.SynchronizeTree.Processors
{
  using System;
  using FluentAssertions;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;
  using Xunit;

  public class UpdateExpirationIntervalTests
  {
    private readonly SynchContext synchContext;
    private readonly ID itemId;

    public UpdateExpirationIntervalTests()
    {
      Item item = new ItemMock().AsConfigurationItem();
      this.itemId = item.ID;
      this.synchContext = new SynchContext(item);
    }

    [Fact]
    public void should_throw_if_args_is_null()
    {
      // Arrange
      var processor = new UpdateExpirationInterval();

      // Act
      Action action = () => processor.Process(null);

      // Assert
      action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: args");
    }

    [Fact]
    public void should_update_exired_interval()
    {
      // Arrange
      var processor = new UpdateExpirationInterval();
      var args = new SynchronizeTreeArgs
      {
        Context = this.synchContext
      };
      var configData = IntegrationCache.GetIntegrationConfigData(this.itemId);
      configData.ExpirationDate = DateTime.Now.AddDays(-1);
      configData.IsExpired.Should().BeTrue();

      // Act
      processor.Process(args);

      // Assert
      IntegrationCache.GetIntegrationConfigData(this.itemId).IsExpired.Should().BeFalse();
    }
  }
}
