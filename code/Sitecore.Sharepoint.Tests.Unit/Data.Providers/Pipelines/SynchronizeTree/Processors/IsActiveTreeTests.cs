// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsActiveTreeTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the IsActiveTree type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.Pipelines.SynchronizeTree.Processors
{
  using System;
  using FluentAssertions;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.Pipelines.SynchronizeTree;
  using Xunit;
  using Xunit.Extensions;

  public class IsActiveTreeTests
  {
    [Fact]
    public void should_throw_if_args_is_not_set()
    {
      // Arrange
      var processor = new IsActiveTree();

      // Act
      Action action = () => processor.Process(null);

      // Assert
      action.ShouldThrow<ArgumentException>().WithMessage("Value cannot be null.\r\nParameter name: args");
    }

    [Fact]
    public void should_throw_if_context_is_not_set()
    {
      // Arrange
      var processor = new IsActiveTree();

      // Act
      Action action = () => processor.Process(new SynchronizeTreeArgs());

      // Assert
      action.ShouldThrow<InvalidOperationException>().WithMessage("Value can't be null: args.Context");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void should_abort_pipeline_if_config_item_is_disabled(bool isEnabled)
    {
      // Arrange
      var configurationItem = new ItemMock().AsConfigurationItem(null, string.Empty, isEnabled);

      var args = new SynchronizeTreeArgs { Context = new SynchContext(configurationItem) };

      var processor = new IsActiveTree();

      // Act
      processor.Process(args);

      // Assert
      args.Aborted.Should().Be(!isEnabled);
    }

    [Theory]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    public void should_abort_pipeline_if_parent_is_disabled(bool rootParent, bool directParent, bool pipelineAborted)
    {
      // Arrange
      var configurationItem = this.PrepareDbWithThreeLevelTree(rootParent, directParent, true);

      var args = new SynchronizeTreeArgs { Context = new SynchContext(configurationItem) };

      var processor = new IsActiveTree();

      // Act
      processor.Process(args);

      // Assert
      args.Aborted.Should().Be(pipelineAborted);
    }

    private ItemMock PrepareDbWithThreeLevelTree(bool rootEnabled, bool folderEnabled, bool subFolderEnabled)
    {
      var root =
        new ItemMock { { FieldIDs.IsIntegrationItem, rootEnabled ? "1" : "0" } }
          .WithTemplate(TemplateIDs.IntegrationConfig);

      var folder =
        new ItemMock { { FieldIDs.IsIntegrationItem, folderEnabled ? "1" : "0" } }
          .WithTemplate(TemplateIDs.IntegrationConfig)
          .WithParent(root);


      var subfolder = new ItemMock { { FieldIDs.IsIntegrationItem, subFolderEnabled ? "1" : "0" } }
        .WithTemplate(TemplateIDs.IntegrationConfig)
         .WithParent(folder);

      return subfolder.PutConfigDataToCache();
    }
  }
}
