// --------------------------------------------------------------------------------------------------------------------
// <copyright file="List.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the List type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Lists
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;

  /// <summary>
  /// Represents a SharePoint list.
  /// </summary>
  public class List : BaseList
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="List"/> class.
    /// </summary>
    /// <param name="propertyValues">The property values.</param>
    /// <param name="context">The context.</param>
    /// <param name="webUrl">The url of Web.</param>
    public List([NotNull] EntityValues propertyValues, [NotNull] SpContext context, [NotNull] Uri webUrl)
      : base(propertyValues, context, webUrl)
    {
      Assert.ArgumentNotNull(propertyValues, "propertyValues");
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");
    }
  }
}
