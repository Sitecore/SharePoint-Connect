namespace Sitecore.Sharepoint.Tests.Unit
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Reflection;
  using Sitecore.Sharepoint.Common.Configuration;

  public class ServerEntrieCollectionSubstitute : IEnumerable, IDisposable
  {
    private static FieldInfo predefinedServerEntries;
    private readonly List<ServerEntry> entries = new List<ServerEntry>();

    static ServerEntrieCollectionSubstitute()
    {
      predefinedServerEntries = typeof(Settings).GetField("predefinedServerEntries", BindingFlags.NonPublic | BindingFlags.Static);
    }

    public ServerEntrieCollectionSubstitute()
    {
      var serverEntryCollection = new ServerEntryCollection();
      typeof(ServerEntryCollection)
        .GetField("entries", BindingFlags.NonPublic | BindingFlags.Instance)
        .SetValue(serverEntryCollection, this.entries);

      predefinedServerEntries.SetValue(null, serverEntryCollection);
    }

    public ServerEntry this[int index]
    {
      get
      {
        return this.entries[index];
      }

      set
      {
        this.entries[index] = value;
      }
    }

    public void Add(ServerEntry entry)
    {
      this.entries.Add(entry);
    }

    public IEnumerator GetEnumerator()
    {
      return this.entries.GetEnumerator();
    }

    public void Dispose()
    {
      predefinedServerEntries.SetValue(null, null);
    }
  }
}