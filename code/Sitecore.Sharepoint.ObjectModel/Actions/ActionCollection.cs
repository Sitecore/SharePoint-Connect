namespace Sitecore.Sharepoint.ObjectModel.Actions
{
  using System.Collections.Generic;

  /// <summary>
  /// ActionsList class.
  /// </summary>
  public class ActionCollection : List<Action>
  {
    /// <summary>
    /// Gets the default.
    /// </summary>
    /// <returns>Default action.</returns>
    public Action GetDefault()
    {
      return this.Find(action => action.IsDefault);
    }

    /// <summary>
    /// Gets the <see cref="Sitecore.Sharepoint.ObjectModel.Actions.Action"/> with the specified action key.
    /// </summary>
    /// <value>Action.</value>
    public Action this[string actionKey]
    {
      get
      {
        return this.Find(action => action.Key == actionKey);
      }
      set
      {
        int currentActionIndex = this.FindIndex(action => action.Key == actionKey);
        if (currentActionIndex > -1)
        {
          this.RemoveAt(currentActionIndex);
          this.Insert(currentActionIndex, value);
        }
        else
        {
          this.Add(value);
        }
      }
    }
  }
}