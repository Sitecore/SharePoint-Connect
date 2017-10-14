// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchResult.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SearchResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Search
{
  using System;
  using System.Drawing;
  using System.Xml;
  using Sitecore.IO;
  using Sitecore.Resources;
  using Sitecore.Sharepoint.ObjectModel.Entities;
  using Sitecore.Sharepoint.ObjectModel.Utils;

  /// <summary>
  /// Represents SharePoint search result.
  /// </summary>
  public class SearchResult : CommonEntity
  {
    public SearchResult(XmlNode node, SpContext context)
      : base(XmlUtils.LoadProperties(node), context)
    {
    }

    public string Thumbnail
    {
      get
      {
        string iconUrl = this["PictureThumbnailURL"];
        if (string.IsNullOrEmpty(iconUrl))
        {
          Uri documentPath = new Uri(this["Path"]);
          string iconExt = FileUtil.GetExtension(documentPath.AbsolutePath);
          iconUrl = string.Format("{0}/_layouts/images/ic{1}.gif", documentPath.GetLeftPart(UriPartial.Authority), iconExt);
          if (String.IsNullOrEmpty(iconExt) && this["SiteName"] == this["Path"])
          {
            iconUrl = string.Format("{0}/_layouts/images/ic{1}.gif", documentPath.GetLeftPart(UriPartial.Authority), "spweb");
          }

          if (!SharepointUtils.IconExists(iconUrl))
          {
            if (!string.IsNullOrEmpty(iconExt))
            {
              iconUrl = FileIcon.GetFileIcon(iconExt, 16, 16, Color.Empty);
            }
            else
            {
              iconUrl = string.Format("{0}/_layouts/images/ic{1}.gif", documentPath.GetLeftPart(UriPartial.Authority), "gen");
            }
          }
        }
        return iconUrl;
      }
    }
  }
}
