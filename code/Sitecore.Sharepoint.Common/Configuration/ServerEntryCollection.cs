// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerEntryCollection.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines ServerEntryCollection class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Configuration
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;
  using Sitecore.Diagnostics;
  
  public class ServerEntryCollection : IEnumerable<ServerEntry>
  {
    private List<ServerEntry> entries;

    public ServerEntryCollection()
    {
      this.entries = new List<ServerEntry>();
    }

    [NotNull]
    public static ServerEntryCollection LoadPredefinedServerEntries()
    {
      var predefinedServerEntries = new ServerEntryCollection();

      XmlNodeList serverEntryNodes = Sitecore.Configuration.Factory.GetConfigNodes("/sitecore/sharepoint/servers/server");
      if (serverEntryNodes != null)
      {
        foreach (XmlNode serverEntryNode in serverEntryNodes)
        {
          string url = GetAttributeValue(serverEntryNode, "url");
          if (string.IsNullOrEmpty(url))
          {
            continue;
          }

          string userName = GetAttributeValue(serverEntryNode, "username");
          string password = GetAttributeValue(serverEntryNode, "password");

          if (userName == null ^ password == null)
          {
            Log.Warn("Credentials for the server node '{0}' are invalid. The server node is skipped.", serverEntryNode.Name);
            continue;
          }

          string context = GetAttributeValue(serverEntryNode, "context");
          if (string.IsNullOrEmpty(context))
          {
            continue;
          }

          string connectionConfiguration = GetAttributeValue(serverEntryNode, "connectionConfiguration");
          
          bool isSharepointOnline;
          bool.TryParse(GetAttributeValue(serverEntryNode, "sharepointOnline", false), out isSharepointOnline);

          var serverEntry = new ServerEntry(url, userName, password, context, isSharepointOnline, connectionConfiguration);
          predefinedServerEntries.entries.Add(serverEntry);
        }
      }

      return predefinedServerEntries;
    }

    /// <summary>
    /// Return first ServerEntry meets passed parameters.
    /// If ServerEntry.Context equals to "Any" then parameter context is not taken into account
    /// </summary>
    /// <param name="url">Server url to search</param>
    /// <param name="context">Context to search (if serverEntry has Context "Any" it may be return as well)</param>
    /// <returns>ServerEntry meets the conditions</returns>
    [CanBeNull]
    public ServerEntry GetFirstEntry([NotNull] string url, [NotNull] string context)
    {
      Assert.ArgumentNotNull(url, "url");
      Assert.ArgumentNotNull(context, "context");

      url = StringUtil.EnsurePostfix('/', url);

      var serverEntries = this.entries.Where(
        x =>
        x.Context.Equals(context, StringComparison.InvariantCultureIgnoreCase)
        || x.Context.Equals("Any", StringComparison.InvariantCultureIgnoreCase))
        .OrderByDescending(x => x.Url.Length);

      return serverEntries.FirstOrDefault(x => url.StartsWith(x.Url, StringComparison.InvariantCultureIgnoreCase));
    }

    #region Implementation of IEnumerable

    [NotNull]
    public IEnumerator<ServerEntry> GetEnumerator()
    {
      return this.entries.GetEnumerator();
    }

    [NotNull]
    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion Implementation of IEnumerable

    [CanBeNull]
    private static string GetAttributeValue([NotNull] XmlNode node, [NotNull] string attributeName, bool required = true)
    {
      Assert.ArgumentNotNull(node, "node");
      Assert.ArgumentNotNullOrEmpty(attributeName, "attributeName");

      XmlAttribute attribute = node.Attributes[attributeName];
      if (attribute != null)
      {
        return attribute.Value;
      }

      string message = required
        ? string.Format(Texts.LogMessages.CanNotFindRequired0AttributeOfSharePointServerSetting, attributeName) 
        : string.Format(Texts.LogMessages.CanNotFind0AttributeOfSharePointServerSettingDefaultValueWillBeUsed, attributeName);
      Log.Warn(message, node);

      return null;
    }
  }
}