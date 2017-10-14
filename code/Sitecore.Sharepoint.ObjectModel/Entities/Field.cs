// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Field.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the Field type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities
{
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;

  /// <summary>
  /// Represents SharePoint fields.
  /// </summary>
  public class Field : CommonEntity
  {
    public Field(EntityProperties properties, SpContext context)
      : base(properties, context)
    {
    }

    /// <summary>
    /// Gets value indicating that field is Content Field.
    /// </summary>
    public bool IsContentField
    {

      get
      {
        return this["FromBaseType"] != "TRUE";
      }
    }

    /// <summary>
    /// Gets or sets name of field.
    /// </summary>
    [CanBeNull]
    public string Name
    {
      get
      {
        return this["Name"];
      }
    }

    /// <summary>
    /// Gets or sets type of field.
    /// </summary>
    [CanBeNull]
    public string Type
    {
      get
      {
        return this["Type"];
      }
    }

    /// <summary>
    /// Gets a value indicating that field is required.
    /// </summary>
    public bool Required
    {
      get
      {
        return this["Required"] == "TRUE";
      }
    }

    /// <summary>
    /// Gets display name of field.
    /// </summary>
    public string DisplayName
    {
      get
      {
        return this["DisplayName"];
      }
    }

    /// <summary>
    /// Gets a value indicating that field is  visible.
    /// </summary>
    public bool Visible
    {
      get
      {
        return this["Hidden"] != "TRUE";
      }
    }

    /// <summary>
    /// Gets header image of field.
    /// </summary>
    public string HeaderImage
    {
      get
      {
        if (this.Type == "Attachments")
        {
          return "attachhd.gif";
        }

        return this["HeaderImage"];
      }
    }

    /// <summary>
    /// Gets display image of field.
    /// </summary>
    public string DisplayImage
    {
      get
      {
        if (this.Type == "Attachments")
        {
          return "attach.gif";
        }

        return this["DisplayImage"];
      }
    }
  }
}