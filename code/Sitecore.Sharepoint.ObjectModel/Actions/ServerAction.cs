namespace Sitecore.Sharepoint.ObjectModel.Actions
{
  using System;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Globalization;

  /// <summary>
  /// ServerAction class.
  /// </summary>
  public class ServerAction : Action, IControlable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerAction"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="displayName">The display name.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="ownerItem">The owner item.</param>
    /// <param name="args">The arguments.</param>
    public ServerAction(string key, string displayName, EventHandler handler, BaseItem ownerItem, params object[] args)
      : base(key, displayName, ownerItem, args)
    {
      this.Handler = handler;
    }

    /// <summary>
    /// Gets or sets the IActionController
    /// </summary>
    /// <value>Instance of IActionController</value>
    public IActionController ActionController
    {
      get;
      set;
    }

    /// <summary>
    /// Gets the handler.
    /// </summary>
    /// <value>The handler.</value>
    public EventHandler Handler
    {
      get; private set;
    }

    /// <summary>
    /// Gets the control.
    /// </summary>
    /// <returns>Control with handler.</returns>
    public override Control GetControl()
    {
      LinkButton actionBtn = new LinkButton();
      actionBtn.Click += this.ActionBtnClick;
      WebControl internalControl = new WebControl(HtmlTextWriterTag.Span);
      internalControl.Controls.Add(new Literal
      {
        Text = Translate.Text(DisplayName)
      });

      actionBtn.Controls.Add(internalControl);
      return actionBtn;
    }

    /// <summary>
    /// Method for call action
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e    .
    /// </param>
    private void ActionBtnClick(object sender, EventArgs e)
    {
      if (this.Handler != null)
      {
        try
        {
          this.Handler(sender, e);
          if (this.ActionController != null)
          {
            this.ActionController.Rebind();
          }
        }
        catch (Exception ex)
        {
          if (this.ActionController != null)
          {
            this.ActionController.ShowError(ex);
          }
        }
      }
    }
  }
}