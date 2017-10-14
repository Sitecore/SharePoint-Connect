namespace Sitecore.Sharepoint.Tests.Unit
{
  using Sitecore.Data;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;

  public static class ItemMockExtensions
  {
    public static ItemMock WithGuidField(this ItemMock item, string value = "")
    {
      return item.WithField(FieldNames.GUID, value);
    }

    public static ItemMock WithIsIntegrationItemField(this ItemMock item, string value = "")
    {
      return item.WithField("active", FieldIDs.IsIntegrationItem, value);
    }

    public static ItemMock WithIntegrationFields(
      this ItemMock item,
      string guidValue = "",
      bool isIntegrationValue = false)
    {
      return item.WithGuidField(guidValue).WithIsIntegrationItemField(isIntegrationValue ? "1" : "0");
    }

    public static ItemMock AsIntegrationItem(this ItemMock item, string guidValue = "", bool isIntegrationValue = false)
    {
      return item.WithIntegrationFields(guidValue, isIntegrationValue).WithTemplate(TemplateIDs.IntegrationBase);
    }

    public static ItemMock WithDataModifiedField(this ItemMock item)
    {
      return item.WithField(FieldNames.SharepointDataModified, string.Empty);
    }

    public static ItemMock CreateIntegrationItem(
      this Database database,
      string guidValue = "",
      bool isIntegrationValue = false)
    {
      return new ItemMock(ID.NewID, database).AsIntegrationItem(guidValue, isIntegrationValue);
    }

    public static ItemMock CreateIntegrationItem(
      this string itemName,
      string guidValue = "",
      bool isIntegrationValue = false)
    {
      return new ItemMock(ID.NewID).WithName(itemName).AsIntegrationItem(guidValue, isIntegrationValue);
    }

    public static ItemMock AsConfigurationItem(
      this ItemMock item,
      IntegrationConfigData configData = null,
      string guidValue = "",
      bool isIntegrationValue = true)
    {
      return
        item
          .WithIntegrationFields(guidValue, isIntegrationValue)
          .WithTemplate(TemplateIDs.IntegrationConfig)
          .PutConfigDataToCache(configData);
    }

    public static ItemMock CreateConfigurationItem(
      this Database database,
      string guidValue = "",
      bool isIntegrationValue = true,
      IntegrationConfigData configData = null)
    {
      return new ItemMock(ID.NewID, database).AsConfigurationItem(configData, guidValue, isIntegrationValue);
    }

    public static ItemMock CreateConfigurationFolder(
      this Database database,
      string guidValue = "",
      bool isIntegrationValue = true,
      IntegrationConfigData configData = null)
    {
      var parent = database.CreateConfigurationItem(string.Empty, isIntegrationValue, configData);

      return
        new ItemMock(ID.NewID, database).WithIntegrationFields(guidValue, isIntegrationValue)
          .WithTemplate(TemplateIDs.IntegrationFolder)
          .WithParent(parent)
          .PutConfigDataToCache(configData);
    }

    public static ItemMock PutConfigDataToCache(this ItemMock item, IntegrationConfigData configData = null)
    {
      IntegrationCache.AddIntegrationConfigData(
        item.ID,
        configData ?? new IntegrationConfigData("server", "list", "templateID"),
        60);
      return item;
    }
  }
}