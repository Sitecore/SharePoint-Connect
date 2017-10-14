// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="Sitecore A/S">
//   Copyright (C) 2013 by Sitecore A/S
// </copyright>
// <summary>
//   Defines Settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Common.Configuration
{
  using System;
  using System.Collections.Generic;
  using System.Xml;
  using Sitecore.Diagnostics;
  using Sitecore.Xml;

  public static class Settings
  {
    #region Private fields

    private static ServerEntryCollection predefinedServerEntries;

    private static List<EntityTypeDefinition> itemTypeDefinitions;

    private static List<EntityTypeDefinition> listTypeDefinitions;

    private static uint? itemRetrievingLimit;

    private static Dictionary<string, ConnectionConfigurationEntry> connectionConfigurations;
    private static IEnumerable<string> requiredFields;

    #endregion Private fields

    #region Public properties

    [NotNull]
    public static ServerEntryCollection PredefinedServerEntries
    {
      get
      {
        return predefinedServerEntries ?? (predefinedServerEntries = ServerEntryCollection.LoadPredefinedServerEntries());
      }
    }

    /// <summary>
    /// Gets the connection configurations collection.
    ///   key   - name of the Connection configuration
    ///   value - the Connection Configuration entry <see cref="ConnectionConfigurationEntry"/>
    /// </summary>
    [NotNull]
    public static Dictionary<string, ConnectionConfigurationEntry> ConnectionConfigurations
    {
      get
      {
        if (connectionConfigurations == null)
        {
          connectionConfigurations = new Dictionary<string, ConnectionConfigurationEntry>();

          var configurationsNode = Sitecore.Configuration.Factory.GetConfigNode("/sitecore/sharepoint/connectionConfigurations");
          if (configurationsNode != null)
          {
            foreach (XmlNode node in configurationsNode.ChildNodes)
            {
              if (node.NodeType == XmlNodeType.Comment)
              {
                continue;
              }

              var entry = new ConnectionConfigurationEntry
              {
                Name = node.Name,
                DisplayName = node.Attributes != null && node.Attributes["displayName"] != null ? node.Attributes["displayName"].Value : node.Name
              };
              connectionConfigurations.Add(entry.Name, entry);
            }
          }
        }

        return connectionConfigurations;
      }
    }

    [NotNull]
    public static ConnectionConfigurationEntry DefaultConfigurator
    {
      get
      {
        return ConnectionConfigurations["Default"];
      }
    }

    [NotNull]
    public static List<EntityTypeDefinition> ItemTypeDefinitions
    {
      get
      {
        return itemTypeDefinitions ?? (itemTypeDefinitions = LoadEntityTypeDefinitions("/sitecore/sharepoint/itemTypeDefinitions"));
      }
    }

    [NotNull]
    public static List<EntityTypeDefinition> ListTypeDefinitions
    {
      get
      {
        return listTypeDefinitions ?? (listTypeDefinitions = LoadEntityTypeDefinitions("/sitecore/sharepoint/listTypeDefinitions"));
      }
    }

    [NotNull]
    public static string DefaultSharepointServer
    {
      get
      {
        return Sitecore.Configuration.Settings.GetSetting("Sharepoint.DefaultServer", string.Empty);
      }
    }

    [NotNull]
    public static string InitializationVector
    {
      get
      {
        return Sitecore.Configuration.Settings.GetSetting("Sharepoint.InitializationVector", "FR+kDoetP5UbWjo9ZvgfVw==");
      }
    }

    public static uint ItemRetrievingLimit
    {
      get
      {
        if (!itemRetrievingLimit.HasValue)
        {
          string strItemLimit = Sitecore.Configuration.Settings.GetSetting("Sharepoint.ItemRetrievingLimit");
          uint itemLimit;
          if (uint.TryParse(strItemLimit, out itemLimit))
          {
            itemRetrievingLimit = itemLimit;
          }
        }

        return itemRetrievingLimit.GetValueOrDefault(100);
      }
    }

    public static long CookiesCacheSize
    {
      get
      {
        return StringUtil.ParseSizeString(Sitecore.Configuration.Settings.GetSetting("Sharepoint.Caching.CookiesCache", "2MB"));
      }
    }

    public static IEnumerable<string> RequiredFields
    {
      get
      {
        return requiredFields 
               ?? (requiredFields = Sitecore.Configuration.Settings.GetSetting("Sharepoint.RequiredFields")
                 .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
      }
    }

    /// <summary>
    /// Gets the integration instance name.
    /// </summary>
    /// <value>The integration name.</value>
    public static string IntegrationInstance
    {
      get
      {
        var instanceName = Sitecore.Configuration.Settings.GetSetting("Sharepoint.IntegrationInstance");
        return !string.IsNullOrWhiteSpace(instanceName) ? instanceName : Sitecore.Configuration.Settings.InstanceName;
      }
    }

    #endregion Public properties

    #region Private methods

    [NotNull]
    private static List<EntityTypeDefinition> LoadEntityTypeDefinitions([NotNull] string configNodeXPath)
    {
      Assert.ArgumentNotNull(configNodeXPath, "configNodeXPath");

      var entityTypeDefinitions = new List<EntityTypeDefinition>();

      XmlNode entityTypeDefinitionsNode = Sitecore.Configuration.Factory.GetConfigNode(configNodeXPath);
      foreach (XmlNode entityTypeDefinitionNode in entityTypeDefinitionsNode)
      {
        if (!XmlUtil.HasAttribute("type", entityTypeDefinitionNode))
        {
          continue;
        }

        var entityTypeDefinition = new EntityTypeDefinition();
        entityTypeDefinitions.Add(entityTypeDefinition);

        foreach (XmlAttribute entityTypeDefinitionAttribute in entityTypeDefinitionNode.Attributes)
        {
          if (entityTypeDefinitionAttribute.Name == "type")
          {
            entityTypeDefinition.Type = entityTypeDefinitionAttribute.Value;
          }
          else
          {
            entityTypeDefinition.Properties[entityTypeDefinitionAttribute.Name] = StringUtil.Split(entityTypeDefinitionAttribute.Value, '|', true);
          }
        }
      }

      return entityTypeDefinitions;
    }

    #endregion Private methods
  }
}
