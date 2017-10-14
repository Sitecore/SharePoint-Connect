// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListFactory.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ListFactory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Factories
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.Reflection;
  using Sitecore.SecurityModel.License;
  using Sitecore.Sharepoint.Common.Configuration;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Factories.TypeDeterminers;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;

  public static class ListFactory
  {
    static ListFactory()
    {
      Error.AssertLicense("Sitecore.Sharepoint", "SharePoint Integration Framework");
    }

    private static EntityTypeDeterminer listTypeDeterminer;

    [NotNull]
    private static EntityTypeDeterminer ListTypeDeterminer
    {
      get
      {
        return listTypeDeterminer ?? (listTypeDeterminer = new EntityTypeDeterminer());
      }
    }

    /// <exception cref="Exception"><c>Throws exception if SharePoint list object could not be created.</c>.</exception>
    [NotNull]
    public static BaseList CreateListObject([NotNull] EntityValues listValues, [NotNull] Uri webUrl, [NotNull] SpContext context)
    {
      Assert.ArgumentNotNull(listValues, "listValues");
      Assert.ArgumentNotNull(webUrl, "webUrl");
      Assert.ArgumentNotNull(context, "context");

      string listType = ListTypeDeterminer.GetEntityType(Settings.ListTypeDefinitions, listValues.Properties);
      if (!string.IsNullOrEmpty(listType))
      {
        var resultList = ReflectionUtil.CreateObject(listType, new object[] { listValues, context, webUrl }) as BaseList;
        if (resultList != null)
        {
          return resultList;
        }

        string message = string.Format("Could not create SharePoint list object of \"{0}\" type", listType);
        throw new Exception(message);
      }

      return new BaseList(listValues, context, webUrl);
    }
  }
}
