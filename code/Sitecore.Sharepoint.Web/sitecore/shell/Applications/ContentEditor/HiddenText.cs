// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HiddenText.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines HiddenText class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Web.Shell.Applications.ContentEditor
{
  using System.Web.UI;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
  using Sitecore.Sharepoint.Common.Texts;
  using Sitecore.Shell.Applications.ContentEditor;

  public class HiddenText : Text
  {
    protected override void DoRender([NotNull] HtmlTextWriter output)
    {
      Assert.ArgumentNotNull(output, "output");

      this.Disabled = true;
      this.ReadOnly = true;
      this.Value = Translate.Text(UIMessages.Data);

      base.DoRender(output);
    }

    protected override bool LoadPostData([CanBeNull] string value)
    {
      return false;
    }
  }
}