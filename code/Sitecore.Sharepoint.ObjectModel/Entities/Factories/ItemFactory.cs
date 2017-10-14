// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemFactory.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ItemFactory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Factories
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.Reflection;
  using Sitecore.Sharepoint.Common.Configuration;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Factories.TypeDeterminers;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;

  public static class ItemFactory
  {
    static ItemFactory()
    {
      Error.AssertLicense("Sitecore.Sharepoint", "SharePoint Integration Framework");
    }

    private static EntityTypeDeterminer itemTypeDeterminer;

    [NotNull]
    private static EntityTypeDeterminer ItemTypeDeterminer
    {
      get
      {
        return itemTypeDeterminer ?? (itemTypeDeterminer = new ItemTypeDeterminer());
      }
    }

    /// <exception cref="Exception"><c>Throws exception if SharePoint list item object could not be created.</c>.</exception>
    [NotNull]
    public static BaseItem CreateItemObject([NotNull] EntityValues itemValues, [NotNull] BaseList list, [NotNull] SpContext context)
    {
      Assert.ArgumentNotNull(itemValues, "itemValues");
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(context, "context");

      string itemType = ItemTypeDeterminer.GetEntityType(Settings.ItemTypeDefinitions, itemValues.Properties);
      if (!string.IsNullOrEmpty(itemType))
      {
        var resultItem = ReflectionUtil.CreateObject(itemType, new object[] { itemValues.Properties, list, context }) as BaseItem;
        if (resultItem != null)
        {
          return resultItem;
        }

        string message = string.Format("Could not create SharePoint list item object of \"{0}\" type", itemType);
        throw new Exception(message);
      }

      return new BaseItem(itemValues.Properties, list, context);
    }
  }
}
