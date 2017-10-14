// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LibraryItem.cs" company="Sitecore A/S">
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
  using Sitecore.IO;
  using Sitecore.Resources;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;
  using Sitecore.Sharepoint.ObjectModel.Utils;
  using Sitecore.Web;

  /// <summary>
  /// Represents SharePoint item from list.
  /// </summary>
  public class LibraryItem : BaseItem
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="LibraryItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="list">The list.</param>
    /// <param name="context">The context.</param>
    public LibraryItem([NotNull] EntityProperties property, [NotNull] BaseList list, [NotNull] SpContext context)
      : base(property, list, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(list, "list");
      Assert.ArgumentNotNull(context, "context");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LibraryItem"/> class.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <param name="listName">The list name.</param>
    /// <param name="webUrl">The url of Web.</param>
    /// <param name="context">The context.</param>
    public LibraryItem([NotNull] EntityProperties property, [NotNull] string listName, [NotNull] Uri webUrl, [NotNull] SpContext context)
      : base(property, listName, webUrl, context)
    {
      Assert.ArgumentNotNull(property, "property");
      Assert.ArgumentNotNull(listName, "listName");
      Assert.ArgumentNotNull(webUrl, "webUrl");
      Assert.ArgumentNotNull(context, "context");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LibraryItem"/> class.
    /// </summary>
    /// <param name="listName">The list name.</param>
    /// <param name="webUrl">The url of Web.</param>
    /// <param name="context">The context.</param>
    protected LibraryItem([NotNull] string listName, [NotNull] Uri webUrl, [NotNull] SpContext context)
      : base(listName, webUrl, context)
    {
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
        return string.Format("{0}/{1}/Forms/DispForm.aspx?ID={2}", this.WebUrl.GetLeftPart(UriPartial.Authority), StringUtil.RemovePrefix("/", StringUtil.RemovePostfix("/", this.RootFolder)), this.ID);
      }
    }

    /// <summary>
    /// Gets title of ListItem.
    /// </summary>
    [CanBeNull]
    public override string Title
    {
      get
      {
        return this["ows_FileLeafRef"];
      }
    }

    /// <summary>
    /// Gets the thumbnail.
    /// </summary>
    /// <value>The thumbnail.</value>
    [CanBeNull]
    public virtual string Thumbnail
    {
      get
      {
        string thumbnailUrl;
        string fileExtension = this["ows_DocIcon"];
        string sharepointIcon = string.Format(
          "{0}/_layouts/images/ic{1}.gif", WebUtil.GetServerUrl(this.WebUrl, false), fileExtension);
        if (SharepointUtils.IconExists(sharepointIcon))
        {
          thumbnailUrl = sharepointIcon;
        }
        else
        {
          if (string.IsNullOrEmpty(fileExtension))
          {
            thumbnailUrl = "/~/icon/Applications/16x16/window.png.aspx";
          }
          else
          {
            thumbnailUrl = FileUtil.UnmapPath(FileIcon.GetFileIcon(fileExtension, 16, 16, System.Drawing.Color.Empty));
          }
        }

        return thumbnailUrl;
      }
    }
  }
}