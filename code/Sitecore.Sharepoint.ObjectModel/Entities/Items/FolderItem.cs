// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderItem.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Folder item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Items
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.Resources;
  using Sitecore.Sharepoint.ObjectModel.Entities.Collections;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;
  using Sitecore.Sharepoint.ObjectModel.Options;
  using Sitecore.Sharepoint.ObjectModel.Utils;
  using Sitecore.Web;
  using Sitecore.Web.UI;

  /// <summary>
  /// Folder item.
  /// </summary>
  public class FolderItem : LibraryItem, IList
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FolderItem"/> class.
    /// </summary>
    /// <param name="folderName">The folder name.</param>
    /// <param name="listName">Name of the list.</param>
    /// <param name="webUrl">The url of Web.</param>
    /// <param name="context">The context.</param>
    public FolderItem([NotNull] string folderName, String parentFolder, [NotNull] string listName, [NotNull] Uri webUrl, [NotNull] SpContext context)
      : base(listName, webUrl, context)
    {
      Assert.ArgumentNotNull(folderName, "folderName");
      Assert.ArgumentNotNull(listName, "listName");
      Assert.ArgumentNotNull(webUrl, "webUrl");
      Assert.ArgumentNotNull(context, "context");

      this["ows_BaseName"] = StringUtil.EnsurePostfix('/', parentFolder) + folderName;
      this["ows_FSObjType"] = "1";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FolderItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="list">The list.</param>
    /// <param name="context">The context.</param>
    public FolderItem([NotNull] EntityProperties property, [NotNull] BaseList list, [NotNull] SpContext context)
      : base(property, list, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(context, "context");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FolderItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="listName">The list name.</param>
    /// <param name="webUrl">The url of Web.</param>
    /// <param name="context">The context.</param>
    public FolderItem([NotNull] EntityProperties property, [NotNull] string listName, [NotNull] Uri webUrl, [NotNull] SpContext context)
      : base(property, listName, webUrl, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(listName, "listName");
      Assert.ArgumentNotNull(webUrl, "webUrl");
      Assert.ArgumentNotNull(context, "context");
    }

    /// <summary>
    /// Gets or sets the thumbnail.
    /// </summary>
    /// <value>The thumbnail.</value>
    [NotNull]
    public override string Thumbnail
    {
      get
      {
        string folderIcon = string.Format(
          "{0}/_layouts/images/folder.gif", WebUtil.GetServerUrl(this.WebUrl, false));
        if (!SharepointUtils.IconExists(folderIcon))
        {
          folderIcon = Images.GetThemedImageSource("Applications/16x16/folder.png", ImageDimension.id16x16);
        }

        return folderIcon;
      }
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>List items.</returns>
    [NotNull]
    public ItemCollection GetItems([NotNull]ItemsRetrievingOptions options)
    {
      Assert.ArgumentNotNull(options, "options");
      var optionIn = options.Clone();

      string folder = string.Empty;

      if (!string.IsNullOrEmpty(this.Folder))
      {
        folder = StringUtil.EnsurePostfix('/', this.Folder) + this.Title;
      }
      else
      {
        folder = this.Title;
      }

      optionIn.Folder = StringUtil.EnsurePostfix('/', folder) + optionIn.Folder;

      var itemCollection = new ItemCollection(this.Context, this.List, optionIn);

      options.CopyFrom(optionIn);

      return itemCollection;
    }
  }
}