// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetItemTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the GetItemTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.Pipelines.ProcessIntegrationItem.Common.Processors
{
  using System;
  using FluentAssertions;

  using NSubstitute;

  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.Pipelines.ProcessIntegrationItem;
  using Sitecore.StringExtensions;
  using Xunit;

  public class GetItemTests
  {
    private readonly GetItem processor;

    public GetItemTests()
    {
      this.processor = new GetItem();
    }

    [Fact]
    public void should_throw_if_args_is_not_set()
    {
      // Act

      // ReSharper disable AssignNullToNotNullAttribute
      Action action = () => this.processor.Process(null);
      // ReSharper restore AssignNullToNotNullAttribute

      // Assert
      action.ShouldThrow<ArgumentException>().WithMessage("Value cannot be null.\r\nParameter name: args");
    }

    [Fact]
    public void should_throw_if_item_id_is_not_set()
    {
      // Act
      Action action = () => this.processor.Process(new ProcessIntegrationItemArgs());

      // Assert
      action.ShouldThrow<InvalidOperationException>().WithMessage("args.IntegrationItemID");
    }

    [Fact]
    public void should_throw_if_context_is_not_set()
    {
      // Act
      Action action = () => this.processor.Process(new ProcessIntegrationItemArgs
      {
        IntegrationItemID = ID.NewID
      });

      // Assert
      action.ShouldThrow<InvalidOperationException>().WithMessage("args.SynchContext");
    }

    [Fact]
    public void should_get_item_from_db()
    {
      // Arrange
      var database = Substitute.For<Database>();
      Item expectedItem = database.CreateIntegrationItem();

      var args = new ProcessIntegrationItemArgs
                   {
                     IntegrationItemID = expectedItem.ID,
                     SynchContext = this.CreateSynchContext(database)
                   };

      // Act
      this.processor.Process(args);

      // Assert
      args.IntegrationItem.Should().Be(expectedItem).And.NotBeNull();
    }

    [Fact]
    public void should_abort_pipeline_if_item_is_null()
    {
      // Arrange
      var args = new ProcessIntegrationItemArgs
                   {
                     IntegrationItemID = ID.NewID,
                     SynchContext = this.CreateSynchContext()
                   };

      // Act
      this.processor.Process(args);

      // Assert
      args.Aborted.Should().BeTrue();
    }

    [Fact]
    public void should_log_if_item_is_null()
    {
      // Arrange
      var args = new ProcessIntegrationItemArgs
                   {
                     IntegrationItemID = ID.NewID,
                     SynchContext = this.CreateSynchContext()
                   };

      // Act
      var logWatcher = new LogWatcher(() => this.processor.Process(args));

      // Assert
      logWatcher.Ensure()
        .LevelIs(LogNotificationLevel.Warning)
        .MessageIs(
          "Can't get item '{0}' from database '{1}'".FormatWith(args.IntegrationItemID, args.SynchContext.Database));
    }

    private SynchContext CreateSynchContext(Database database = null)
    {
      var item = database.CreateConfigurationItem();
      return new SynchContext(item);
    }
  }
}
