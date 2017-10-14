// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpDataContext.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SpDataContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers
{
  using System;
  using System.Net;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Common.Configuration;
  using Sitecore.Sharepoint.ObjectModel;

  /// <summary>
  /// Class uses for connecting to Sharepoint services.
  /// </summary>
  [Obsolete("The class is obsolete.")]
  public class SpDataContext : SpContext
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SpDataContext"/> class.
    /// </summary>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    [Obsolete("Not default constructors are deprecated.Please use SpContextProvider.Instance.CreateContext methods or default constructor and set appropriate properties.")]
    public SpDataContext([NotNull] string userName, [NotNull] string password) : base(userName, password)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpDataContext"/> class.
    /// </summary>
    /// <param name="credentials">
    /// The credentials.
    /// </param>
    [Obsolete("Not default constructors are deprecated.Please use SpContextProvider.Instance.CreateContext methods or default constructor and set appropriate properties.")]
    public SpDataContext([NotNull] ICredentials credentials) : base(credentials)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpDataContext"/> class.
    /// </summary>
    /// <param name="serverUrl">
    /// The server url.
    /// </param>
    [Obsolete("Not default constructors are deprecated.Please use SpContextProvider.Instance.CreateContext methods or default constructor and set appropriate properties.")]
    public SpDataContext([NotNull] string serverUrl) : base(serverUrl)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpDataContext"/> class.
    /// </summary>
    /// <param name="url">
    /// The url.
    /// </param>
    /// <param name="credentials">
    /// The credentials.
    /// </param>
    [Obsolete("Not default constructors are deprecated.Please use SpContextProvider.Instance.CreateContext methods or default constructor and set appropriate properties.")]
    public SpDataContext([NotNull] string url, ICredentials credentials) : base(url, credentials)
    {
    }

    [Obsolete("The method is obsolete and will be removed in future versions.")]
    protected override ServerEntry GetPredefinedServerEntry(string serverUrl)
    {
      Assert.ArgumentNotNull(serverUrl, "serverUrl");

      return Settings.PredefinedServerEntries.GetFirstEntry(serverUrl, "Provider");
    }
  }
}