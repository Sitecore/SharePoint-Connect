namespace Sitecore.Sharepoint.Common
{
  using System;
  using System.Net;

  public abstract class SpContextBase
  {
    /// <summary>
    /// Gets the URL to the Sharepoint server.
    /// </summary>
    public abstract string Url { get; set; }

    /// <summary>
    /// Gets Credentials to be used to connect to Sharepoint server
    /// </summary>
    public abstract ICredentials Credentials { get; set; }

    /// <summary>
    /// Gets the hash to be used in HashTable
    /// </summary>
    public abstract int Hash { get; }

    /// <summary>
    /// Gets or sets a value indicating whether Claims-Based authentication should be used.
    /// </summary>
    [Obsolete("IsSharepointOnline property is obsolete.")]
    public abstract bool IsSharepointOnline { get; set; }

    public abstract string ConnectionConfiguration { get; set; }
  }
}