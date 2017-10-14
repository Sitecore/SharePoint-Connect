// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityPropertyValue.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the EntityPropertyValue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Common
{
  using Sitecore.Diagnostics;

  /// <summary>
  /// Represents property of SharePoint entities.
  /// </summary>
  public class EntityPropertyValue
  {
    /// <summary>
    /// Old value of property
    /// </summary>
    public readonly string OldValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityPropertyValue"/> class. 
    /// </summary>
    public EntityPropertyValue()
    {
      this.OldValue = null;
      this.Value = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityPropertyValue"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public EntityPropertyValue([NotNull] string value)
    {
      Assert.ArgumentNotNull(value, "value");

      this.OldValue = value;
      this.Value = value;
    }

    /// <summary>
    /// Gets or sets current value of property
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets a value indicating whether that property is new.
    /// </summary>
    public bool IsNew
    {
      get
      {
        return this.OldValue == null;
      }
    }

    /// <summary>
    /// Gets a value indicating whether that property is new.
    /// </summary>
    public bool IsChanged
    {
      get
      {
        return this.OldValue != this.Value;
      }
    }


    /// <summary>
    /// Gets a value indicating whether that property is new.
    /// </summary>
    public bool IsDelete
    {
      get
      {
        return this.OldValue != null && this.Value == null;
      }
    }
  }
}
