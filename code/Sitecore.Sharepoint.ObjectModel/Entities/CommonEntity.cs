// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommonEntity.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the CommonEntity type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities
{
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;

  /// <summary>
  /// Base class for all objects in the Sharepoint Object Model.
  /// </summary>
  public abstract class CommonEntity
  {
    /// <summary>
    /// Gets or sets Context for Item.
    /// </summary>
    protected readonly SpContext Context;

    /// <summary>
    /// Gets or sets properties of ListItem.
    /// </summary>
    private EntityProperties properties;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonEntity"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    protected CommonEntity([NotNull] SpContext context)
    {
      Assert.ArgumentNotNull(context, "context");

      this.Context = context;
      this.FillProperty(new EntityProperties());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonEntity"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="context">The context.</param>
    protected CommonEntity([NotNull] EntityProperties property, [NotNull] SpContext context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(context, "context");

      this.Context = context;
      this.FillProperty(property);
    }

    [CanBeNull]
    public string ContentTypeId
    {
      get
      {
        string contentTypeIdValue = this["ows_ContentTypeId", true];
        if (!string.IsNullOrEmpty(contentTypeIdValue))
        {
          return contentTypeIdValue;
        }

        return this.Properties.GetMetaInfoProperty("ContentTypeId");
      }
    }

    /// <summary>
    /// Gets keys of all properties.
    /// </summary>
    /// <value>The keys of all properties.</value>
    [NotNull]
    public virtual string[] Keys
    {
      get
      {
        return this.Properties.ToKeyArray();
      }
    }

    /// <summary>
    /// Gets or sets properties of ListItem.
    /// </summary>
    /// <value>The properties.</value>
    [NotNull]
    protected virtual EntityProperties Properties
    {
      get
      {
        return this.properties;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.properties = value;
      }
    }

    /// <summary>
    /// Gets or sets properties of Item.
    /// </summary>
    /// <param name="key">
    /// The key  .
    /// </param>
    [NotNull]
    public virtual string this[[NotNull] string key]
    {
      get
      {
        return this[key, false];
      }
      set
      {
        Assert.ArgumentNotNull(key, "key");

        if (this.Properties[key] == null)
        {
          this.Properties[key] = new EntityPropertyValue();
        }

        this.Properties[key].Value = value;
      }
    }

    /// <summary>
    /// Gets properties of Item.
    /// </summary>
    /// <param name="key">
    /// The key    .
    /// </param>
    /// <param name="getOriginalVersion">
    /// The is original version.
    /// </param>
    [NotNull]
    public virtual string this[[NotNull] string key, bool getOriginalVersion]
    {
      get
      {
        var property = this.Properties[key];
        if (property != null)
        {
          string result = getOriginalVersion ? property.OldValue : property.Value;
          return result ?? string.Empty;
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Return value indicate that Property contains key.
    /// </summary>
    /// <param name="key">The key of property.</param>
    /// <returns>Value indicate that Property contains key.</returns>
    public virtual bool ContainsKey([NotNull] string key)
    {
      Assert.ArgumentNotNull(key, "key");

      return this.Properties.ContainsKey(key);
    }

    /// <summary>
    /// Fill property for instante of object.
    /// </summary>
    /// <param name="dic">The dictionary.</param>
    protected void FillProperty([NotNull] EntityProperties dic)
    {
      Assert.ArgumentNotNull(dic, "dic");

      this.properties = dic;
    }
  }
}