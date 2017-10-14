// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerEntryCollectionTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ServerEntryCollectionTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Common.Configuration
{
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;
  using System.Reflection;

  using FluentAssertions;

  using Sitecore.Sharepoint.Common.Configuration;

  using Xunit;
  using Xunit.Extensions;

  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
  public class ServerEntryCollectionTests
  {
    private readonly ServerEntryCollection entryCollection;

    public ServerEntryCollectionTests()
    {
      this.entryCollection = new ServerEntryCollection();
    }

    [Theory]
    [InlineData("http://server", "http://server/")]
    [InlineData("http://server/", "http://server")]
    [InlineData("http://server/web", "http://server/web/")]
    [InlineData("http://server/web/", "http://server/web")]
    public void ShouldHandleSlashes(string entryUrl, string url)
    {
      // Arrange
      var expectedEntry = new ServerEntry(entryUrl, "user", "password", "context");
      this.FillEntries(
        this.entryCollection, 
        new List<ServerEntry> { expectedEntry });

      // Act
      var entry = this.entryCollection.GetFirstEntry(url, "context");

      // Assert
      entry.Should().BeSameAs(expectedEntry);
    }

    [Fact]
    public void ShouldMatchLongestUrl()
    {
      // Arrange
      var expectedEntry = new ServerEntry("http://server1/web1/subweb1/", "user", "password", "context");
      this.FillEntries(
        this.entryCollection, 
        new List<ServerEntry>
          {
            new ServerEntry("http://server1/", "user", "password", "context"), 
            new ServerEntry("http://server1/web1/", "user", "password", "context"), 
            expectedEntry
          });

      // Act
      var entry = this.entryCollection.GetFirstEntry("http://server1/web1/subweb1/subweb2", "context");

      // Assert
      entry.Should().BeSameAs(expectedEntry);
    }

    [Fact]
    public void ShouldHandleContexts()
    {
      // Arrange
      const string Url = "http://server1/";
      this.FillEntries(
        this.entryCollection, 
        new List<ServerEntry> { new ServerEntry(Url, "userName", "password", "Context1") });

      // Act 
      var entry = this.entryCollection.GetFirstEntry(Url, "Context2");

      // Assert
      entry.Should().BeNull();
    }

    [Theory]
    [InlineData("Any")]
    [InlineData("anY")]
    public void ShouldHandleAnyContext(string anyContext)
    {
      // Arrange
      const string Url = "http://server1/";
      var expectedEntry = new ServerEntry(Url, "userName", "password", anyContext);
      this.FillEntries(
        this.entryCollection, 
        new List<ServerEntry> { expectedEntry });

      // Act 
      var entry = this.entryCollection.GetFirstEntry(Url, "Context2");

      // Assert
      entry.Should().BeSameAs(expectedEntry);
    }

    [Fact]
    public void ShouldHandleUrlAndContextCase()
    {
      const string Url = "http://server1/";
      const string Context = "Context1";
      var expectedEntry = new ServerEntry(Url, "userName", "password", Context);
      this.FillEntries(
        this.entryCollection,
        new List<ServerEntry> { expectedEntry });

      // Act 
      var entry = this.entryCollection.GetFirstEntry(Url.ToUpper(), Context.ToUpper());

      // Assert
      entry.Should().BeSameAs(expectedEntry);
    }

    /// <summary>Fills private entries field.</summary>
    /// <param name="collection">The target collection.</param>
    /// <param name="entries">The entries to fill.</param>
    private void FillEntries(ServerEntryCollection collection, List<ServerEntry> entries)
    {
      typeof(ServerEntryCollection)
        .GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance)
        .SetValue(collection, entries);
    }
  }
}