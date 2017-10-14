// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsIntegrationInstanceTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the IntegrationInstanceTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.Pipelines.SynchronizeTree.Processors
{
  using System;
  using FluentAssertions;
  using NSubstitute;

  using Sitecore.Configuration;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Sharepoint.Common.Wrappers;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Azure;
  using Sitecore.Sharepoint.Data.Providers.Options;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;
  using Xunit;

  public class IsIntegrationInstanceTests
  {
    private readonly SynchContext synchContext;
    private readonly AzureEnvironmentBase azureEnvironment = Substitute.For<AzureEnvironmentBase>();

    public IsIntegrationInstanceTests()
    {
      Item item = new ItemMock().AsConfigurationItem();
      this.synchContext = new SynchContext(item);
    }

    [Fact]
    public void should_throw_if_args_is_null()
    {
      // Arrange
      var processor = new IsIntegrationInstance(new SitecoreEventManager(), this.azureEnvironment);

      // Act
      Action action = () => processor.Process(null);

      // Assert
      action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: args");
    }

    [Fact]
    public void should_abort_pipeline_if_not_integration_instance_not_azure()
    {
      // Arrange
      var processor = new IsIntegrationInstance(Substitute.For<SitecoreEventManager>(), this.azureEnvironment);
      var args = new SynchronizeTreeArgs
      {
        Context = this.synchContext,
        Options = ProcessIntegrationItemsOptions.DefaultOptions
      };
      this.azureEnvironment.IsAzure.Returns(false);
      this.azureEnvironment.IsIntegrationInstance.Returns(true);

      // Act
      using (new SettingsSwitcher("Sharepoint.IntegrationInstance", ID.NewID.ToString()))
      {
        processor.Process(args);
      }

      // Assert
      args.Aborted.Should().BeTrue();
    }

    [Fact]
    public void should_abort_pipeline_if_not_integration_instance_in_azure()
    {
      // Arrange
      var processor = new IsIntegrationInstance(Substitute.For<SitecoreEventManager>(), this.azureEnvironment);
      var args = new SynchronizeTreeArgs
      {
        Context = this.synchContext,
        Options = ProcessIntegrationItemsOptions.DefaultOptions
      };
      this.azureEnvironment.IsAzure.Returns(true);
      this.azureEnvironment.IsIntegrationInstance.Returns(false);

      // Act
      using(new SettingsSwitcher("Sharepoint.IntegrationInstance", Settings.InstanceName))
      {
        processor.Process(args);
      }

      // Assert
      args.Aborted.Should().BeTrue();
    }

    [Fact]
    public void should_queue_event_if_IsEven_is_false()
    {
      // Arrange
      var eventManager = Substitute.For<SitecoreEventManager>();
      var processor = new IsIntegrationInstance(eventManager, this.azureEnvironment);
      var args = new SynchronizeTreeArgs
      {
        Context = this.synchContext,
        Options = ProcessIntegrationItemsOptions.DefaultOptions
      };
      args.Options.IsEvent = false;
      
      // Act
      processor.Process(args);

      // Assert
      eventManager.Received().QueueEvent(Arg.Is<ProcessTreeRemoteEvent>(x => x.DatabaseName == args.Context.Database.Name && x.Id == args.Context.ParentID && x.EventName == "spif:processTree"));
    }

    [Fact]
    public void should_not_queue_event_if_IsEven_is_true()
    {
      // Arrange
      var eventManager = Substitute.For<SitecoreEventManager>();
      var processor = new IsIntegrationInstance(eventManager, this.azureEnvironment);
      var args = new SynchronizeTreeArgs
      {
        Context = this.synchContext,
        Options = ProcessIntegrationItemsOptions.DefaultOptions
      };
      args.Options.IsEvent = true;

      // Act
      processor.Process(args);

      // Assert
      eventManager.DidNotReceiveWithAnyArgs().QueueEvent(Arg.Any<ProcessTreeRemoteEvent>());
    }
  }
}