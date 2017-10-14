// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventList.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   EventList class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Sitecore.Sharepoint.ObjectModel.Entities.Lists
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;

  /// <summary>
  /// EventList class.
  /// </summary>
  public class EventList : List
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="EventList"/> class.
    /// </summary>
    /// <param name="propertyValues">
    /// The property Values.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="webUrl">
    /// The url of Web.
    /// </param>
    public EventList([NotNull] EntityValues propertyValues, [NotNull] SpContext context, [NotNull] Uri webUrl)
      : base(propertyValues, context, webUrl)
    {
      Assert.ArgumentNotNull(propertyValues, "propertyValues");
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");
    }
  }
}