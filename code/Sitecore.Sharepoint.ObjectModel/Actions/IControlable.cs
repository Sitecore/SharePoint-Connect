using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitecore.Sharepoint.ObjectModel.Actions
{
  /// <summary>
  /// Interface for chouse 
  /// </summary>
  public interface IControlable
  {
    /// <summary>
    /// Gets or sets the IActionController
    /// </summary>
    /// <value>Instance of IActionController</value>
    IActionController ActionController
    {
      get;
      set;
    }
  }
}
