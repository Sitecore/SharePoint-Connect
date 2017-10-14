// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharepointAnnouncements.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines SharepointAnnouncements web part.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Intranet.Sharepoint.Web.Layouts.WebParts
{
  public class SharepointAnnouncements : SharepointListBase
  {
    protected override string InnerControlVirtualPath
    {
      get
      {
        return "/layouts/Sharepoint/SharepointAnnouncements.ascx";
      }
    }
  }
}