// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnnouncementItem.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the AnnouncementItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Items
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;

  /// <summary>
  /// Represents SharePoint announcement
  /// </summary>
  public class AnnouncementItem : ListItem
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AnnouncementItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="list">The list.</param>
    /// <param name="context">The context.</param>
    public AnnouncementItem([NotNull] EntityProperties property, [NotNull] BaseList list, [NotNull] SpContext context)
      : base(property, list, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(context, "context");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnnouncementItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="listName">The list name.</param>
    /// <param name="webUrl">The url od Web.</param>
    /// <param name="context">The context.</param>
    public AnnouncementItem([NotNull] EntityProperties property, [NotNull] string listName, [NotNull] Uri webUrl, [NotNull] SpContext context)
      : base(property, listName, webUrl, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(listName, "listName");
      Assert.ArgumentNotNull(webUrl, "webUrl");
      Assert.ArgumentNotNull(context, "context");
    }

    /// <summary>
    /// Gets announcement body.
    /// </summary>
    [CanBeNull]
    public string Body
    {
      get
      {
        return this["ows_Body"];
      }
    }

    /// <summary>
    /// Gets date when announcement is expires.
    /// </summary>
    public DateTime Expires
    {
      get
      {
        DateTime expireDate;
        DateTime.TryParse(this["ows_Expires"], out expireDate);
        return expireDate;
      }
    }

    /// <summary>
    /// Gets date when announcement was modified.
    /// </summary>
    public DateTime Modified
    {
      get
      {
        DateTime modifiedDate;
        DateTime.TryParse(this["ows_Modified"], out modifiedDate);
        return modifiedDate;
      }
    }
  }
}