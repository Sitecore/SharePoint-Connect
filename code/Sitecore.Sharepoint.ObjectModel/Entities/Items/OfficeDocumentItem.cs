// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OfficeDocumentItem.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   OfficeDocumentItem class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Items
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Texts;
  using Sitecore.Sharepoint.ObjectModel.Actions;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;

  /// <summary>
  /// OfficeDocumentItem class.
  /// </summary>
  public class OfficeDocumentItem : DocumentItem
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="OfficeDocumentItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="list">The list.</param>
    /// <param name="context">The context.</param>
    public OfficeDocumentItem([NotNull] EntityProperties property, [NotNull] BaseList list, [NotNull] SpContext context)
      : base(property, list, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(context, "context");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OfficeDocumentItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="listName">The list name.</param>
    /// <param name="webUrl">The url of Web.</param>
    /// <param name="context">The context.</param>
    public OfficeDocumentItem([NotNull] EntityProperties property, [NotNull] string listName, [NotNull] Uri webUrl, [NotNull] SpContext context)
      : base(property, listName, webUrl, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(listName, "listName");
      Assert.ArgumentNotNull(webUrl, "webUrl");
      Assert.ArgumentNotNull(context, "context");
    }

    /// <summary>
    /// Gets the actions.
    /// </summary>
    /// <returns>List item actions.</returns>
    [NotNull]
    public override ActionCollection GetActions()
    {
      ActionCollection baseActions = base.GetActions();
      baseActions["open"] = new ClientAction(
        "open",
        UIMessages.OpenFile, 
        "openSharepointDocument('{0}'); return false;", 
        this, 
        this.OpenItemUrl)
      {
        IsDefault = true
      };

      return baseActions;
    }
  }
}