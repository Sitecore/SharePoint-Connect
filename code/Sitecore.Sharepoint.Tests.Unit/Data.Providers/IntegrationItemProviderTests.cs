// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationItemProviderTests.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Tests.Unit.Data.Providers
{
  using System;
  using FluentAssertions;
  using NSubstitute;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Items;
  using Sitecore.Sharepoint.ObjectModel;
  using Xunit;

  /// <summary>
  /// The integration item provider tests.
  /// </summary>
  public class IntegrationItemProviderTests
  {
    /// <summary>
    /// The context.
    /// </summary>
    private readonly SpContext context = Substitute.For<SpContext>();

    /// <summary>
    /// The source item.
    /// </summary>
    private readonly ObjectModel.Entities.Items.BaseItem sourceSharepointItem;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationItemProviderTests"/> class.
    /// </summary>
    public IntegrationItemProviderTests()
    {
      this.sourceSharepointItem = Substitute.For<ObjectModel.Entities.Items.BaseItem>(string.Empty, new Uri("http://uri.uri"), this.context);
    }

    /// <summary>
    /// The should_fill_guid_field.
    /// </summary>
    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void UpdateFields_should_update_guid_field()
    {
      // Arrange
      this.sourceSharepointItem.GUID.Returns("SomeGuid");
      Item targetIntegrationItem = new ItemMock().AsConfigurationItem();

      // Act
      IntegrationItemProvider.UpdateFields(
        targetIntegrationItem,
        this.sourceSharepointItem,
        new SynchContext(targetIntegrationItem));

      // Assert
      targetIntegrationItem.Fields[FieldNames.GUID].Value.Should().Be("SomeGuid");
    }

    /// <summary>
    /// The update fields_should_set_ is integration_if_guid_is_empty.
    /// </summary>
    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void UpdateFields_should_set_IsIntegration_if_guid_is_empty()
    {
      // Arrange
      Item targetIntegrationItem = new ItemMock().AsConfigurationItem();
      using (new EditContext(targetIntegrationItem))
      {
        new CheckboxField(targetIntegrationItem.Fields[FieldIDs.IsIntegrationItem]).Checked = false;
      }

      // Act
      IntegrationItemProvider.UpdateFields(
        targetIntegrationItem,
        this.sourceSharepointItem,
        new SynchContext(targetIntegrationItem));

      // Assert
      new CheckboxField(targetIntegrationItem.Fields[FieldIDs.IsIntegrationItem]).Checked.Should().BeTrue();
    }

    /// <summary>
    /// The update fields_should_not_set_ is integration_if_guid_is_not_empty.
    /// </summary>
    [Fact(Skip = "Requires SharePoint Integration Framework license file.")]
    public void UpdateFields_should_not_set_IsIntegration_if_guid_is_not_empty()
    {
      // Arrange
      Item targetIntegrationItem = new ItemMock().AsConfigurationItem();
      new IntegrationItem(targetIntegrationItem).GUID = "some not empty GUID";
      using (new EditContext(targetIntegrationItem))
      {
        new CheckboxField(targetIntegrationItem.Fields[FieldIDs.IsIntegrationItem]).Checked = false;
      }

      // Act
      IntegrationItemProvider.UpdateFields(
        targetIntegrationItem,
        this.sourceSharepointItem,
        new SynchContext(targetIntegrationItem));

      // Assert
      new CheckboxField(targetIntegrationItem.Fields[FieldIDs.IsIntegrationItem]).Checked.Should().BeFalse();
    }
  }
}
