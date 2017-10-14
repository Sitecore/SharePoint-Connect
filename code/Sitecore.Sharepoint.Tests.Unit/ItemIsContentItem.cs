namespace Sitecore.Sharepoint.Tests.Unit
{
  using System;

  using NSubstitute;

  using Sitecore.Abstractions;
  using Sitecore.Data.Items;

  using ItemUtil = Sitecore.Sharepoint.Common.Utils.ItemUtil;

  public class ItemIsContentItem : IDisposable
  {
    public ItemIsContentItem(params Item[] items)
    {
      var templateManager = Substitute.For<BaseTemplateManager>();
      foreach (var item in items)
      {
        templateManager.IsTemplate(item).Returns(false);
      }

      ItemUtil.TemplateManager = templateManager;
    }

    public void Dispose()
    {
      ItemUtil.TemplateManager = null;
    }
  }
}