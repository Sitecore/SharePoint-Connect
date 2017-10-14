// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventItem.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the EventItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Sitecore.Sharepoint.ObjectModel.Entities.Items
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;

  /// <summary>
  /// Represents SharePoint event.
  /// </summary>
  public class EventItem : ListItem
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="EventItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="list">The list.</param>
    /// <param name="context">The context.</param>
    public EventItem([NotNull] EntityProperties property, [NotNull] BaseList list, [NotNull] SpContext context)
      : base(property, list, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(context, "context");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="listName">The list Name.</param>
    /// <param name="webUrl">The url of Web.</param>
    /// <param name="context">The context.</param>
    public EventItem([NotNull] EntityProperties property, [NotNull] string listName, [NotNull] Uri webUrl, [NotNull] SpContext context)
      : base(property, listName, webUrl, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(listName, "listName");
      Assert.ArgumentNotNull(webUrl, "webUrl");
      Assert.ArgumentNotNull(context, "context");
    }

    /// <summary>
    /// Gets description of event.
    /// </summary>
    [CanBeNull]
    public string Description
    {
      get
      {
        return this["ows_Description"];
      }
    }

    /// <summary>
    /// Gets end date of event.
    /// </summary>
    [CanBeNull]
    public string EndDate
    {
      get
      {
        return this["ows_EndDate"];
      }
    }

    /// <summary>
    /// Gets date of event.
    /// </summary>
    [CanBeNull]
    public string EventDate
    {
      get
      {
        return this["ows_EventDate"];
      }
    }
  }
}