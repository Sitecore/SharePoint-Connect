// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveExtensionTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ResolveNameTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers.Pipelines.Create.Processors
{
  using System;
  using FluentAssertions;

  using Sitecore.Configuration;
  using Sitecore.Data.Items;
  using Sitecore.Sharepoint.Pipelines.CreateSharepointItem;
  using Sitecore.Sharepoint.Pipelines.ProcessSharepointItem;
  using Xunit;
  using Xunit.Extensions;

  public class ResolveExtensionTests
  {
    [Fact]
    public void should_throw_if_args_is_not_set()
    {
      // Arrange
      var processor = new ResolveExtension();

      // Act
      Action action = () => processor.Process(null);

      // Assert
      action.ShouldThrow<ArgumentException>().WithMessage("Value cannot be null.\r\nParameter name: args");
    }

    [Fact]
    public void should_throw_if_SourceIntegrationItem_and_SourceIntegrationItemName_are_not_set()
    {
      // Arrange
      var processor = new ResolveExtension();

      // Act
      Action action = () => processor.Process(new ProcessSharepointItemArgs());

      // Assert
      action.ShouldThrow<InvalidOperationException>().WithMessage("args.SourceIntegrationItemName");
    }

    [Fact]
    public void shold_do_noting_if_item_is_not_set()
    {
      // Arrange
      var args = new ProcessSharepointItemArgs
      {
        SourceIntegrationItemName = "some name"
      };
      
      var processor = new ResolveExtension();

      // Act
      processor.Process(args);

      // Assert
      args.SourceIntegrationItemName.Should().Be("some name");
    }

    [Theory]
    [InlineData("fileName", "ext", "fileName.ext")]
    [InlineData("fileName", "", "fileName")]
    [InlineData("fileName.ext", "ext", "fileName.ext.ext")]
    [InlineData("fileName ext", "ext", "fileName ext.ext")]
    public void should_add_extension_if_IncludeExtensionsInItemNames_is_disabled(string itemName, string extensions, string expectedName)
    {
      // Arrange
      using (new SettingsSwitcher("Media.IncludeExtensionsInItemNames", bool.FalseString))
      {
        Item sourceItem = new ItemMock().AsIntegrationItem().WithField("Extension", extensions);

        var args = new ProcessSharepointItemArgs
        {
          SourceIntegrationItemName = itemName,
          SourceIntegrationItem = sourceItem,
        };

        var processor = new ResolveExtension();

        // Act
        processor.Process(args);

        // Assert
        args.SourceIntegrationItemName.Should().Be(expectedName);
      }
    }

    [Theory]
    [InlineData("file", "ext", "file.ext")]
    [InlineData("file ", "ext", "file .ext")]
    [InlineData("fileext", "ext", "fileext.ext")]
    [InlineData("file ext", "ext", "file.ext")]
    [InlineData("file ext ext", "ext", "file ext.ext")]
    [InlineData("file ext", "ext1", "file ext.ext1")]
    [InlineData("file ext1 ext", "ext1", "file ext1 ext.ext1")]
    [InlineData("file", "", "file")]
    [InlineData("file.ext", "", "file.ext")]
    public void should_fix_name_if_IncludeExtensionsInItemNames_is_enabled(string itemName, string extensions, string expectedName)
    {
      // Arrange
      using (new SettingsSwitcher("Media.IncludeExtensionsInItemNames", bool.TrueString))
      {
        Item sourceItem = itemName.CreateIntegrationItem().WithField("Extension", extensions);
        var args = new ProcessSharepointItemArgs
        {
          SourceIntegrationItemName = sourceItem.Name,
          SourceIntegrationItem = sourceItem,
        };

        var processor = new ResolveExtension();

        // Act
        processor.Process(args);

        // Assert
        args.SourceIntegrationItemName.Should().Be(expectedName);
      }
    }

    [Theory]
    [InlineData("file*ext", "*", "file.ext")]
    [InlineData("file*ext", "!", "file*ext.ext")]
    public void should_respect_WhitespaceReplacement_setting(string itemName, string whitespaceReplacement, string expectedName)
    {
      using (new SettingsSwitcher("Media.IncludeExtensionsInItemNames", bool.TrueString))
      using (new SettingsSwitcher("Media.WhitespaceReplacement", whitespaceReplacement))
      {
        Item sourceItem = itemName.CreateIntegrationItem().WithField("Extension", "ext");
        var args = new ProcessSharepointItemArgs
        {
          SourceIntegrationItemName = sourceItem.Name,
          SourceIntegrationItem = sourceItem,
        };

        var processor = new ResolveExtension();

        // Act
        processor.Process(args);

        // Assert
        args.SourceIntegrationItemName.Should().Be(expectedName);
      }
    }
  }
}
