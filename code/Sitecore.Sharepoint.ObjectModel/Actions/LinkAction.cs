// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkAction.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   ClientAction class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Actions
{
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Globalization;

  /// <summary>
  /// ClientAction class.
  /// </summary>
  public class LinkAction : Action
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="LinkAction"/> class. 
    /// </summary>
    /// <param name="key">
    /// The   key.
    /// </param>
    /// <param name="displayName">
    /// The display name.
    /// </param>
    /// <param name="link">
    /// The navigation link.
    /// </param>
    /// <param name="target">
    /// The navigation target.
    /// </param>
    /// <param name="ownerItem">
    /// The owner item.
    /// </param>
    /// <param name="args">
    /// The arguments.
    /// </param>
    public LinkAction(string key, string displayName, string link, string target, BaseItem ownerItem, params object[] args)
      : base(key, displayName, ownerItem, args)
    {
      this.Link = link;
      this.Target = target;
    }

    /// <summary>
    /// Gets the navigation target.
    /// </summary>
    /// <value>The navigation target.</value>
    public string Target
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets the navigation link.
    /// </summary>
    /// <value>The navigation link.</value>
    public string Link
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets the control for showing.
    /// </summary>
    /// <returns>Adjusted control.</returns>
    public override Control GetControl()
    {
      HyperLink lnk = new HyperLink()
      {
        Target = this.Target,
        NavigateUrl = this.Link,
      };
      lnk.Attributes["nolock"] = "true";
      WebControl internalControl = new WebControl(HtmlTextWriterTag.Span);
      internalControl.Controls.Add(new Literal
      {
        Text = Translate.Text(DisplayName)
      });

      lnk.Controls.Add(internalControl);
      return lnk;
    }
  }
}