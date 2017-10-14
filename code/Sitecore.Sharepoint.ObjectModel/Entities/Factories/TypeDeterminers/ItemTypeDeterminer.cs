// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemTypeDeterminer.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ItemTypeDeterminer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Factories.TypeDeterminers
{
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Configuration;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;

  public class ItemTypeDeterminer : EntityTypeDeterminer
  {
    protected override bool VerifyEntityTypeCondition(
                                                      [NotNull] string entityPropertyName,
                                                      [NotNull] EntityTypeDefinition entityTypeDefinition, 
                                                      [NotNull] EntityProperties entityProperties)
    {
      Assert.ArgumentNotNull(entityPropertyName, "entityPropertyName");
      Assert.ArgumentNotNull(entityTypeDefinition, "entityTypeDefinition");
      Assert.ArgumentNotNull(entityProperties, "entityProperties");

      string entityPropertyValue;

      EntityPropertyValue entityProperty = entityProperties["ows_" + entityPropertyName];
      if (entityProperty != null)
      {
        entityPropertyValue = entityProperty.Value;
      }
      else
      {
        if (entityPropertyName != "ContentTypeId")
        {
          return false;
        }

        entityPropertyValue = entityProperties.GetMetaInfoProperty(entityPropertyName);
      }

      if (string.IsNullOrEmpty(entityPropertyValue))
      {
        return false;
      }

      foreach (string entityTypePropertyValue in entityTypeDefinition.Properties[entityPropertyName])
      {
        if (entityPropertyName == "ContentTypeId" ? entityPropertyValue.StartsWith(entityTypePropertyValue) : entityPropertyValue == entityTypePropertyValue)
        {
          return true;
        }
      }

      return false;
    }
  }
}
