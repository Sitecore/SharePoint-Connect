namespace Sitecore.Sharepoint.ObjectModel.Actions
{
  using System.Web.UI;
  using Sitecore.Sharepoint.ObjectModel.Entities.Items;

  /// <summary>
  /// Action class.
  /// </summary>
  public abstract class Action
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Action"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="displayName">The display name.</param>
    /// <param name="ownerItem">The owner item.</param>
    /// <param name="args">The arguments.</param>
    protected Action(string key, string displayName, BaseItem ownerItem, params object[] args)
    {
      Key = key;
      DisplayName = displayName;
      OwnerItem = ownerItem;
      Arguments = args;
      IsDefault = false;
    }

    /// <summary>
    /// Gets or sets the owner item.
    /// </summary>
    /// <value>The owner item.</value>
    public BaseItem OwnerItem
    {
      get; private set;
    }

    /// <summary>
    /// Gets or sets the arguments.
    /// </summary>
    /// <value>The arguments.</value>
    public object[] Arguments
    {
      get; private set;
    }

    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    /// <value>The key.</value>
    public string Key
    {
      get; private set;
    }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    /// <value>The display name.</value>
    public string DisplayName
    {
      get; private set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is default.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is default; otherwise, <c>false</c>.
    /// </value>
    public bool IsDefault
    {
      get; set;
    }

    /// <summary>
    /// Gets the control.
    /// </summary>
    /// <returns>Control with handler.</returns>
    public abstract Control GetControl();
  }
}
