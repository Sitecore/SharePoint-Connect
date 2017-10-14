// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpUiContext.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SpUiContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Web
{
  using System;
  using System.Net;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Configuration;
  using Sitecore.Sharepoint.ObjectModel;

  /// <summary>
  /// Class uses for connecting to Sharepoint services.
  /// Use this class for UI controls.
  /// </summary>
  [Obsolete("The class is obsolete.")]
  public class SpUiContext : SpContext
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SpUiContext"/> class.
    /// </summary>
    /// <param name="userName">Name of the user to access to Sharepoint server.</param>
    /// <param name="password">The user password.</param>
    [Obsolete("Not default constructors are deprecated.Please use SpContextProvider.Instance.CreateContext methods or default constructor and set appropriate properties.")]
    public SpUiContext([NotNull] string userName, [NotNull] string password)
      : base(userName, password)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpUiContext"/> class.
    /// </summary>
    /// <param name="credentials">The credentials.</param>
    [Obsolete("Not default constructors are deprecated.Please use SpContextProvider.Instance.CreateContext methods or default constructor and set appropriate properties.")]
    public SpUiContext([NotNull] ICredentials credentials)
      : base(credentials)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpUiContext"/> class.
    /// </summary>
    /// <param name="serverUrl">The server URL.</param>
    [Obsolete("Not default constructors are deprecated.Please use SpContextProvider.Instance.CreateContext methods or default constructor and set appropriate properties.")]
    public SpUiContext([NotNull] string serverUrl)
      : base(serverUrl)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpUiContext"/> class.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <param name="credentials">The credentials.</param>
    [Obsolete("Not default constructors are deprecated.Please use SpContextProvider.Instance.CreateContext methods or default constructor and set appropriate properties.")]
    public SpUiContext([NotNull] string url, [NotNull] ICredentials credentials)
      : base(url, credentials)
    {
    }

    [Obsolete("The method is obsolete and will be removed in future versions.")]
    protected override ServerEntry GetPredefinedServerEntry(string serverUrl)
    {
      Assert.ArgumentNotNull(serverUrl, "serverUrl");

      return Settings.PredefinedServerEntries.GetFirstEntry(serverUrl, "Webcontrol");
    }
  }
}