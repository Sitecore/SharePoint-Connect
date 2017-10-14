// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IList.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the IList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Lists
{
  using Sitecore.Sharepoint.ObjectModel.Entities.Collections;
  using Sitecore.Sharepoint.ObjectModel.Options;

  public interface IList
  {
    ItemCollection GetItems([NotNull] ItemsRetrievingOptions options);
  }
}
