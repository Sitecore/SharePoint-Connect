// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityTypeDefinition.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines EntityTypeDefinition class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Configuration
{
  using Sitecore.Collections;

  public class EntityTypeDefinition
  {
    public EntityTypeDefinition()
    {
      this.Properties = new SafeDictionary<string, string[]>();
    }

    [NotNull]
    public string Type { get; set; }

    [NotNull]
    public SafeDictionary<string, string[]> Properties { get; set; }
  }
}
