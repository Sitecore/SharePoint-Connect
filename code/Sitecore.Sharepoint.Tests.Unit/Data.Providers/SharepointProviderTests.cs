// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharepointProviderTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SharepointProviderTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers
{
  using System.Linq;
  using FluentAssertions;

  using NSubstitute;

  using Ploeh.AutoFixture.Xunit;

  using Sitecore.Abstractions;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Eventing;
  using Sitecore.Jobs;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Sharepoint.Data.Providers.Pipelines;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;

  using Xunit;
  using Xunit.Extensions;

  public class SharepointProviderTests
  {
    private readonly BaseFactory factory = Substitute.For<BaseFactory>();

    public SharepointProviderTests()
    {
      SharepointProvider.Factory = this.factory;
    }

    [Fact]
    public void should_process_tree_via_pipeline()
    {
      // Arrange
      using (var pipelines = new PipelinesHandler())
      {
        var options = ProcessIntegrationItemsOptions.DefaultOptions;
        options.Force = true;
        Item item = new ItemMock().AsConfigurationItem();

        // Act
        SharepointProvider.ProcessTree(item, options);

        // Assert
        pipelines.ShouldReceiveCall<SynchronizeTreeArgs>(PipelineNames.SynchronizeTree, x => true);
      }
    }

    [Fact]
    public void should_process_tree_for_queued_events()
    {
      // Arrange
      Item item = new ItemMock().AsConfigurationItem();
      IntegrationCache.RemoveIntegrationConfigData(item.ID);

      string jobName = string.Format("SharePoint_Integration_{0}", item.ID);

      SharepointProvider.Instance.Should().NotBeNull();

      this.factory.GetDatabase(item.Database.Name).Returns(item.Database);

      // Act
      EventManager.RaiseEvent(new ProcessTreeRemoteEvent(item.ID, item.Database.Name));

      // Assert
      JobManager.GetJobs().Any(x => x.Name == jobName).Should().BeTrue();
    }

    [Theory, AutoData]
    public void should_not_log_error_if_item_is_not_exists(string databaseName)
    {
      // Arrange
      SharepointProvider.Instance.Should().NotBeNull();

      var isErrorLogged = false;

      this.factory.GetDatabase(databaseName).Returns(Substitute.For<Database>());

      LogNotification.Notification +=
        (level, message, exception) => { isErrorLogged = level == LogNotificationLevel.Error; };

      // Act
      EventManager.RaiseEvent(new ProcessTreeRemoteEvent(ID.NewID, databaseName));

      // Assert
      isErrorLogged.Should().BeFalse();
    }
  }
}
