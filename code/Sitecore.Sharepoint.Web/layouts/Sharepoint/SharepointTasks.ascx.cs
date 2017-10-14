// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharepointTasks.ascx.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SharepointTasks type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Web
{
  using Sitecore.Sharepoint.Common.Texts;

  public partial class SharepointTasks : SharepointGridViewListBase
  {
    public override void ReloadData()
    {
      base.ReloadData();
      if (documentsList.DataSource == null)
      {
        this.Visible = true;
        this.labelErrorMessage.Visible = true;
        this.labelErrorMessage.Text = string.IsNullOrEmpty(this.ListName) ? UIMessages.PleaseSelectASharePointListFromTheSPIFControlProperties : UIMessages.ConnectionToSharePointServerFailedPleaseCheckYourSharePointServerConfigurationDetails;
      }
    }
  }
}