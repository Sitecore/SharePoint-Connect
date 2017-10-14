namespace Sitecore.Sharepoint.ObjectModel.Actions
{
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;
  using Sitecore.Globalization;

  /// <summary>
  /// ClientAction class.
  /// </summary>
  public class ClientAction : Action
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientAction"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="displayName">The display name.</param>
    /// <param name="script">The script.</param>
    /// <param name="ownerItem">The owner item.</param>
    /// <param name="args">The arguments.</param>
    public ClientAction(string key, string displayName, string script, BaseItem ownerItem, params object[] args)
      : base(key, displayName, ownerItem, args)
    {
      RawScript = script;
    }

    /// <summary>
    /// Gets or sets the raw script (with parameters placeholders).
    /// </summary>
    /// <value>The raw script.</value>
    public string RawScript
    {
      get; private set;
    }

    /// <summary>
    /// Gets the script.
    /// </summary>
    /// <value>The script.</value>
    public string Script
    {
      get
      {
        return string.Format(RawScript, Arguments);
      }
    }

    /// <summary>
    /// Gets the control.
    /// </summary>
    /// <returns>Control with handler.</returns>
    public override Control GetControl()
    {
      LinkButton actionBtn = new LinkButton
      {
        OnClientClick = string.Format(this.RawScript, this.Arguments)
      };
      WebControl internalSpan = new WebControl(HtmlTextWriterTag.Span);
      internalSpan.Controls.Add(new Literal
      {
        Text = Translate.Text(DisplayName)
      });

      actionBtn.Controls.Add(internalSpan);
      return actionBtn;
    }
  }
}