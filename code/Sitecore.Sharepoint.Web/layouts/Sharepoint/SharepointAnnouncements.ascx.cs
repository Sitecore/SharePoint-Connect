// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharepointAnnouncements.ascx.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SharepointAnnouncements type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Web
{
  using System;
  using System.Linq;
  using Sitecore.Sharepoint.Common.Configuration;
  using Sitecore.Sharepoint.Common.Texts;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;
  using Sitecore.Sharepoint.ObjectModel.Options;

  public partial class SharepointAnnouncements : SharepointGridViewListBase
  {
    public override void ReloadData()
    {
      string folderPathValue = this.folderPath.Value;

      BaseList list = LoadDataSource();
      if (list != null)
      {
        var listSelected = from l in list.GetItems(ItemsRetrievingOptions.DefaultOptions)
                    where ((AnnouncementItem)l).Expires >= DateTime.Today || ((AnnouncementItem)l).Expires == DateTime.MinValue
                    select l;

        documentsList.DataSource = listSelected;
      }
      else
      {
        this.Visible = true;
        this.labelErrorMessage.Visible = true;
        this.labelErrorMessage.Text = string.IsNullOrEmpty(this.ListName) ? UIMessages.PleaseSelectASharePointListFromTheSPIFControlProperties : UIMessages.ConnectionToSharePointServerFailedPleaseCheckYourSharePointServerConfigurationDetails;
      }

      this.LabelSharepointSite.Text = this.GetBreadcrumbValue(folderPathValue);
    }
  }
}