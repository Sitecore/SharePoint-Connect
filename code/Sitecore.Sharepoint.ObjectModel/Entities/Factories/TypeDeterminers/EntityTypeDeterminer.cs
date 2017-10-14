// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityTypeDeterminer.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines EntityTypeDeterminer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Factories.TypeDeterminers
{
  using System.Collections.Generic;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Configuration;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;

  public class EntityTypeDeterminer
  {
    [CanBeNull]
    public virtual string GetEntityType([NotNull] List<EntityTypeDefinition> entityTypeDefinitions, [NotNull] EntityProperties entityProperties)
    {
      Assert.ArgumentNotNull(entityTypeDefinitions, "entityTypeDefinitions");
      Assert.ArgumentNotNull(entityProperties, "entityProperties");

      foreach (EntityTypeDefinition entityTypeDefinition in entityTypeDefinitions)
      {
        if (this.IsProperEntityType(entityTypeDefinition, entityProperties))
        {
          return entityTypeDefinition.Type;
        }
      }

      return null;
    }

    protected virtual bool IsProperEntityType([NotNull] EntityTypeDefinition entityTypeDefinition, [NotNull] EntityProperties entityProperties)
    {
      Assert.ArgumentNotNull(entityTypeDefinition, "entityTypeDefinition");
      Assert.ArgumentNotNull(entityProperties, "entityProperties");

      foreach (string entityPropertyName in entityTypeDefinition.Properties.Keys)
      {
        if (!this.VerifyEntityTypeCondition(entityPropertyName, entityTypeDefinition, entityProperties))
        {
          return false;
        }
      }

      return true;
    }

    protected virtual bool VerifyEntityTypeCondition(
                                                     [NotNull] string entityPropertyName,
                                                     [NotNull] EntityTypeDefinition entityTypeDefinition,
                                                     [NotNull] EntityProperties entityProperties)
    {
      Assert.ArgumentNotNull(entityPropertyName, "entityPropertyName");
      Assert.ArgumentNotNull(entityTypeDefinition, "entityTypeDefinition");
      Assert.ArgumentNotNull(entityProperties, "entityProperties");

      EntityPropertyValue entityProperty = entityProperties[entityPropertyName];
      if (entityProperty == null)
      {
        return false;
      }

      string entityPropertyValue = entityProperty.Value;
      if (string.IsNullOrEmpty(entityPropertyValue))
      {
        return false;
      }

      foreach (string entityTypePropertyValue in entityTypeDefinition.Properties[entityPropertyName])
      {
        if (entityPropertyValue == entityTypePropertyValue)
        {
          return true;
        }
      }

      return false;
    }
  }
}
