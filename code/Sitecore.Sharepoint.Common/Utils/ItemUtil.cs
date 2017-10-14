// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemUtil.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ItemUtil class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Utils
{
  using Microsoft.Extensions.DependencyInjection;

  using Sitecore.Abstractions;
  using Sitecore.Data.Items;
  using Sitecore.DependencyInjection;
  using Sitecore.Diagnostics;

  public class ItemUtil
  {
    private static BaseTemplateManager templateManager;

    internal static BaseTemplateManager TemplateManager
    {
      get
      {
        return templateManager
               ?? (templateManager = ServiceLocator.ServiceProvider.GetRequiredService<BaseTemplateManager>());
      }

      set
      {
        templateManager = value;
      }
    }

    public static bool IsContentItem([NotNull] Item item)
    {
      Assert.ArgumentNotNull(item, "item");

      return !ItemUtil.TemplateManager.IsTemplate(item) &&
             !ItemUtil.TemplateManager.IsStandardValuesHolder(item) &&
             !ItemUtil.TemplateManager.IsTemplatePart(item);
    }
  }
}
