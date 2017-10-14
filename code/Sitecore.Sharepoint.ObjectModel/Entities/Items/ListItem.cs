// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListItem.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the ListItem type.
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
  /// Base class for all items from List.
  /// </summary>
  public class ListItem : BaseItem
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ListItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="list">The list.</param>
    /// <param name="context">The context.</param>
    public ListItem([NotNull] EntityProperties property, [NotNull] BaseList list, [NotNull] SpContext context)
      : base(property, list, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(context, "context");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ListItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="listName">The list name.</param>
    /// <param name="webUrl">The url of Web.</param>
    /// <param name="context">The context.</param>
    public ListItem([NotNull] EntityProperties property, [NotNull] string listName, [NotNull] Uri webUrl, [NotNull] SpContext context)
      : base(property, listName, webUrl, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(listName, "listName");
      Assert.ArgumentNotNull(webUrl, "webUrl");
      Assert.ArgumentNotNull(context, "context");
    }

    /// <summary>
    /// Gets the URL of the form that displays the current list item on the SharePoint server.
    /// </summary>
    public override string PropertiesUrl
    {
      get
      {
        return string.Format("{0}/{1}/DispForm.aspx?ID={2}", this.WebUrl.GetLeftPart(UriPartial.Authority), StringUtil.RemovePrefix("/", StringUtil.RemovePostfix("/", this.RootFolder)), this.ID);
      }
    }

    public override ActionCollection GetActions()
    {
      ActionCollection actions = base.GetActions();

      var action = new ClientAction("viewitem", UIMessages.ViewItem, "window.open('{0}');return false;", this, this.PropertiesUrl)
      {
        IsDefault = true
      };
      actions.Add(action);

      return actions;
    }
  }
}