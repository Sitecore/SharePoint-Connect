// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Library.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the Library class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Lists
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;

  /// <summary>
  /// DocumentList class.
  /// </summary>
  public class Library : BaseList
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Library"/> class.
    /// </summary>
    /// <param name="propertyValues">The property Values.</param>
    /// <param name="context">The context.</param>
    /// <param name="webUrl">The url of Web.</param>
    public Library([NotNull] EntityValues propertyValues, [NotNull] SpContext context, [NotNull] Uri webUrl)
      : base(propertyValues, context, webUrl)
    {
      Assert.ArgumentNotNull(propertyValues, "propertyValues");
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");
    }
  }
}