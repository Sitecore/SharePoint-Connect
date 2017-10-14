// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationConfigData.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines IntegrationConfigData class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers.IntegrationConfig
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Xml;
  using Sitecore.Diagnostics;
  using Sitecore.Reflection;
  using Sitecore.Sharepoint.Common.Security.Cryptography;

  /// <summary>
  /// Class with settings for filling Sharepoint Root Item.
  /// </summary>
  public class IntegrationConfigData
  {
    #region Private constants

    private const string Text0NodeIsRequired = "{0} node is required.";

    private const string Text0ValueIsRequired = "{0} value is required.";

    private const string TheTargetXmlDocumentHasIncorrectFormat = "The target xml document has incorrect format.";

    #region Constants that define names of nodes in serialization

    private const string RootNodeName = "IntegrationConfigData";

    private const string ServerNodeName = "Server";

    private const string WebNodeName = "Web";

    private const string ListNodeName = "List";

    private const string FolderNodeName = "Folder";

    private const string ViewNodeName = "View";

    private const string UserNameNodeName = "UserName";

    private const string PasswordNodeName = "Password";

    private const string ItemLimitNodeName = "ItemLimit";

    private const string ExpirationIntervalNodeName = "ExpirationInterval";

    private const string TemplateIDNodeName = "TemplateID";

    private const string FieldMappingsNodeName = "FieldMappings";

    private const string ConnectionConfigurationNodeName = "ConnectionConfiguration";

    #endregion Constants that define names of nodes in serialization

    #endregion Private constants

    #region Fields

    private string server;

    private string list;

    private string templateID;

    private List<FieldMapping> fieldMappings;

    #endregion Fields

    #region Constructors

    public IntegrationConfigData([NotNull] string server, [NotNull] string list, [NotNull] string templateID)
      : this()
    {
      Assert.ArgumentNotNullOrEmpty(server, "server");
      Assert.ArgumentNotNullOrEmpty(list, "list");
      Assert.ArgumentNotNullOrEmpty(templateID, "templateID");

      this.Server = server;
      this.List = list;
      this.TemplateID = templateID;
    }

    public IntegrationConfigData([NotNull] string xmlData, [CanBeNull] string encryptionKey)
      : this()
    {
      Assert.ArgumentNotNullOrEmpty(xmlData, "xmlData");

      this.DeserializeFromString(xmlData, encryptionKey);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationConfigData"/> class.
    /// </summary>
    protected IntegrationConfigData()
    {
      this.FieldMappings = new List<FieldMapping>();
      this.IsSharepointOnline = false;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets or sets name of SharePoint Server.
    /// </summary>
    [NotNull]
    public string Server
    {
      get
      {
        return this.server;
      }

      set
      {
        Assert.ArgumentNotNullOrEmpty(value, "value");

        this.server = value;
      }
    }

    /// <summary>
    /// Gets or sets Web of SharePoint Server.
    /// </summary>
    public string Web { get; set; }

    /// <summary>
    /// Gets or sets List of SharePoint Web.
    /// </summary>
    [NotNull]
    public string List
    {
      get
      {
        return this.list;
      }

      set
      {
        Assert.ArgumentNotNullOrEmpty(value, "value");

        this.list = value;
      }
    }

    /// <summary>
    /// Gets or sets Folder of SharePoint List.
    /// </summary>
    public string Folder { get; set; }

    /// <summary>
    /// Gets or sets View of SharePoint List.
    /// </summary>
    public string View { get; set; }

    public ICredentials Credentials { get; protected set; }

    public uint ItemLimit { get; set; }

    /// <summary>
    /// Gets or sets the expiration interval for Sharepoint items in CMS.
    /// </summary>
    public ulong ExpirationInterval { get; set; }

    /// <summary>
    /// Gets or sets TemplateID for Sharepoint items in CMS.
    /// </summary>
    [NotNull]
    public string TemplateID
    {
      get
      {
        return this.templateID;
      }

      set
      {
        Assert.ArgumentNotNullOrEmpty(value, "value");

        this.templateID = value;
      }
    }

    /// <summary>
    /// Gets or sets list of field mapping from SharePoint to CMS items.
    /// </summary>
    [NotNull]
    public List<FieldMapping> FieldMappings
    {
      get
      {
        return this.fieldMappings;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.fieldMappings = value;
      }
    }

    public bool ScheduledBlobTransfer { get; set; }

    public bool BidirectionalLink { get; set; }

    [Obsolete("IsSharepointOnline property is obsolete.")]
    public bool IsSharepointOnline { get; set; }

    public string ConnectionConfiguration { get; set; }

    #endregion Properties

    #region Public methods

    public void SetCredentials([CanBeNull] string userName, [CanBeNull] string password)
    {
      if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
      {
        string domain = StringUtil.GetPrefix(userName, '\\');
        userName = StringUtil.GetPostfix(userName, '\\', userName);

        this.Credentials = new NetworkCredential(userName, password, domain);
      }
      else
      {
        this.Credentials = null;
      }
    }

    #region Serialization methods

    [NotNull]
    public XmlDocument Serialize([CanBeNull] string encryptionKey)
    {
      Assert.IsNotNullOrEmpty(this.Server, "Server");
      Assert.IsNotNullOrEmpty(this.List, "List");
      Assert.IsNotNullOrEmpty(this.TemplateID, "TemplateID");

      var xmlDocument = new XmlDocument();

      XmlNode rootNode = xmlDocument.CreateElement(RootNodeName);
      xmlDocument.AppendChild(rootNode);

      XmlNode node = xmlDocument.CreateElement(ServerNodeName);
      node.InnerXml = this.Server;
      rootNode.AppendChild(node);

      if (!string.IsNullOrEmpty(this.Web))
      {
        node = xmlDocument.CreateElement(WebNodeName);
        node.InnerXml = this.Web;
        rootNode.AppendChild(node);
      }

      node = xmlDocument.CreateElement(ListNodeName);
      node.InnerXml = this.List;
      rootNode.AppendChild(node);

      if (!string.IsNullOrEmpty(this.Folder))
      {
        node = xmlDocument.CreateElement(FolderNodeName);
        node.InnerXml = this.Folder;
        rootNode.AppendChild(node);
      }

      if (!string.IsNullOrEmpty(this.View))
      {
        node = xmlDocument.CreateElement(ViewNodeName);
        node.InnerXml = this.View;
        rootNode.AppendChild(node);
      }

      var credentials = this.Credentials as NetworkCredential;
      if (credentials != null)
      {
        Assert.IsNotNullOrEmpty(encryptionKey, "Integration Config Data that contains credentials could not be serialized without encryption key.");

        string userName = credentials.UserName;
        string domain = credentials.Domain;
        if (!string.IsNullOrEmpty(domain))
        {
          userName = StringUtil.EnsurePostfix('\\', domain) + userName;
        }

        node = xmlDocument.CreateElement(UserNameNodeName);
        node.InnerXml = userName;
        rootNode.AppendChild(node);

        node = xmlDocument.CreateElement(PasswordNodeName);
        node.InnerXml = CryptoManager.Encrypt(credentials.Password, encryptionKey);
        rootNode.AppendChild(node);
      }

      if (this.ItemLimit > 0)
      {
        node = xmlDocument.CreateElement(ItemLimitNodeName);
        node.InnerXml = this.ItemLimit.ToString();
        rootNode.AppendChild(node);
      }

      if (this.ExpirationInterval > 0)
      {
        node = xmlDocument.CreateElement(ExpirationIntervalNodeName);
        node.InnerXml = this.ExpirationInterval.ToString();
        rootNode.AppendChild(node);
      }

      node = xmlDocument.CreateElement(TemplateIDNodeName);
      node.InnerXml = this.TemplateID;
      rootNode.AppendChild(node);

      if (this.FieldMappings.Count > 0)
      {
        XmlNode fieldMappingsNode = xmlDocument.CreateElement(FieldMappingsNodeName);
        rootNode.AppendChild(fieldMappingsNode);

        foreach (FieldMapping fieldMapping in this.FieldMappings)
        {
          XmlNode fieldMappingNode = xmlDocument.ImportNode(fieldMapping.Serialize(), true);
          fieldMappingsNode.AppendChild(fieldMappingNode);
        }
      }

      if (!string.IsNullOrEmpty(this.ConnectionConfiguration))
      {
        node = xmlDocument.CreateElement(ConnectionConfigurationNodeName);
        node.InnerXml = this.ConnectionConfiguration;
        rootNode.AppendChild(node);
      }

      return xmlDocument;
    }

    [NotNull]
    public string SerializeToString([CanBeNull] string encryptionKey)
    {
      TextWriter stringWriter = new StringWriter();
      var xmlTextWriter = new XmlTextWriter(stringWriter);

      XmlDocument xmlDocument = this.Serialize(encryptionKey);
      xmlDocument.WriteTo(xmlTextWriter);

      return stringWriter.ToString();
    }

    public void Deserialize([NotNull] XmlDocument xmlDocument, [CanBeNull] string encryptionKey)
    {
      Assert.ArgumentNotNull(xmlDocument, "xmlDocument");

      XmlElement rootNode = xmlDocument.DocumentElement;
      Assert.AreEqual(rootNode.Name, RootNodeName, TheTargetXmlDocumentHasIncorrectFormat);

      XmlNode node = rootNode.SelectSingleNode(ServerNodeName);
      Assert.IsNotNull(node, string.Format(Text0NodeIsRequired, "Server"));
      Assert.IsNotNullOrEmpty(node.InnerXml, string.Format(Text0ValueIsRequired, "Server"));
      this.Server = node.InnerXml;

      node = rootNode.SelectSingleNode(ListNodeName);
      Assert.IsNotNull(node, string.Format(Text0NodeIsRequired, "List"));
      Assert.IsNotNullOrEmpty(node.InnerXml, string.Format(Text0ValueIsRequired, "List"));
      this.List = node.InnerXml;

      node = rootNode.SelectSingleNode(TemplateIDNodeName);
      Assert.IsNotNull(node, string.Format(Text0NodeIsRequired, "TemplateID"));
      Assert.IsNotNullOrEmpty(node.InnerXml, string.Format(Text0ValueIsRequired, "TemplateID"));
      this.TemplateID = node.InnerXml;

      string password = null;
      node = rootNode.SelectSingleNode(PasswordNodeName);
      if (node != null && !string.IsNullOrEmpty(node.InnerXml))
      {
        Assert.IsNotNullOrEmpty(encryptionKey, "Integration Config Data that contains credentials could not be deserialized without encryption key.");
        password = CryptoManager.Decrypt(node.InnerXml, encryptionKey);
      }

      string userName = null;
      node = rootNode.SelectSingleNode(UserNameNodeName);
      if (node != null && !string.IsNullOrEmpty(node.InnerXml))
      {
        userName = node.InnerXml;
      }

      this.SetCredentials(userName, password);

      node = rootNode.SelectSingleNode(WebNodeName);
      if (node != null && !string.IsNullOrEmpty(node.InnerXml))
      {
        this.Web = node.InnerXml;
      }
      else
      {
        this.Web = string.Empty;
      }

      node = rootNode.SelectSingleNode(FolderNodeName);
      if (node != null && !string.IsNullOrEmpty(node.InnerXml))
      {
        this.Folder = node.InnerXml;
      }
      else
      {
        this.Folder = string.Empty;
      }

      node = rootNode.SelectSingleNode(ViewNodeName);
      if (node != null && !string.IsNullOrEmpty(node.InnerXml))
      {
        this.View = node.InnerXml;
      }
      else
      {
        this.View = string.Empty;
      }

      node = rootNode.SelectSingleNode(ItemLimitNodeName);
      uint itemLimit;
      if (node != null && uint.TryParse(node.InnerXml, out itemLimit))
      {
        this.ItemLimit = itemLimit;
      }
      else
      {
        this.ItemLimit = 0;
      }

      node = rootNode.SelectSingleNode(ExpirationIntervalNodeName);
      ulong expirationInterval;
      if (node != null && ulong.TryParse(node.InnerXml, out expirationInterval))
      {
        this.ExpirationInterval = expirationInterval;
      }
      else
      {
        this.ExpirationInterval = 0;
      }

      this.FieldMappings = new List<FieldMapping>();
      XmlNode fieldMappingsNode = rootNode.SelectSingleNode(FieldMappingsNodeName);
      if (fieldMappingsNode != null)
      {
        foreach (XmlNode fieldMappingNode in fieldMappingsNode.ChildNodes)
        {
          this.FieldMappings.Add(new FieldMapping(fieldMappingNode));
        }
      }

      node = rootNode.SelectSingleNode(ConnectionConfigurationNodeName);
      this.ConnectionConfiguration = node != null ? node.InnerXml : null;
    }

    public void DeserializeFromString([NotNull] string xmlData, [CanBeNull] string encryptionKey)
    {
      Assert.ArgumentNotNullOrEmpty(xmlData, "xmlData");

      var xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(xmlData);

      this.Deserialize(xmlDocument, encryptionKey);
    }

    #endregion Serialization methods

    /// <summary>
    /// This method make clone of current instance.
    /// </summary>
    /// <returns>Clone of current instance.</returns>
    [NotNull]
    public IntegrationConfigData Clone()
    {
      var clone = new IntegrationConfigData(this.Server, this.List, this.TemplateID)
      {
        Web = this.Web,
        Folder = this.Folder,
        View = this.View,
        ItemLimit = this.ItemLimit,
        ExpirationInterval = this.ExpirationInterval,
        ScheduledBlobTransfer = this.ScheduledBlobTransfer,
        BidirectionalLink = this.BidirectionalLink,
        IsSharepointOnline = this.IsSharepointOnline,
        ConnectionConfiguration = this.ConnectionConfiguration
      };

      var credentials = this.Credentials as NetworkCredential;
      clone.Credentials = credentials == null ? null : new NetworkCredential(credentials.UserName, credentials.Password, credentials.Domain);

      foreach (FieldMapping fieldMapping in this.FieldMappings)
      {
        clone.FieldMappings.Add(fieldMapping.Clone());
      }

      return clone;
    }

    /// <summary>
    /// Get size of instance.
    /// </summary>
    /// <returns>
    /// Size of instance.
    /// </returns>
    public int GetDataLength()
    {
      return TypeUtil.SizeOfString(this.Server) +
             TypeUtil.SizeOfString(this.Web) +
             TypeUtil.SizeOfString(this.List) +
             TypeUtil.SizeOfString(this.Folder) +
             TypeUtil.SizeOfString(this.View) +
             TypeUtil.SizeOfObject() +
             TypeUtil.SizeOfInt32() +
             TypeUtil.SizeOfInt64() +
             TypeUtil.SizeOfString(this.TemplateID) +
             this.FieldMappings.Sum(mapping => TypeUtil.SizeOfString(mapping.Source) + TypeUtil.SizeOfString(mapping.Source)) +
             sizeof(bool) +
             sizeof(bool);
    }

    #endregion Public methods

    /// <summary>
    /// Class contains information about field mapping from Sharepoint to CMS.
    /// </summary>
    public class FieldMapping
    {
      #region Constants that define names of nodes in serialization

      private const string FieldMappingNodeName = "FieldMapping";

      private const string SourceNodeName = "Source";

      private const string TargetNodeName = "Target";

      #endregion Constants that define names of nodes in serialization

      #region Fields

      private string source;
      
      private string target;

      #endregion Fields

      #region Constructors

      public FieldMapping([NotNull] string source, [NotNull] string target)
      {
        Assert.ArgumentNotNullOrEmpty(source, "source");
        Assert.ArgumentNotNullOrEmpty(target, "target");

        this.Source = source;
        this.Target = target;
      }

      public FieldMapping([NotNull] XmlNode xmlNode)
      {
        Assert.ArgumentNotNull(xmlNode, "xmlNode");

        this.Deserialize(xmlNode);
      }

      #endregion Constructors

      #region Properties

      /// <summary>
      /// Gets or sets Source.
      /// Contains name of SharePoint field.
      /// </summary>
      [NotNull]
      public string Source
      {
        get
        {
          return this.source;
        } 

        set
        {
          Assert.ArgumentNotNullOrEmpty(value, "value");

          this.source = value;
        }
      }

      /// <summary>
      /// Gets or sets Target.
      /// Contains name of CMS field.
      /// </summary>
      [NotNull]
      public string Target
      {
        get
        {
          return this.target;
        }

        set
        {
          Assert.ArgumentNotNullOrEmpty(value, "value");

          this.target = value;
        }
      }

      #endregion Properties

      #region Public methods

      #region Serialization methods

      [NotNull]
      public XmlNode Serialize()
      {
        Assert.IsNotNullOrEmpty(this.Source, "Source");
        Assert.IsNotNullOrEmpty(this.Target, "Target");

        var xmlDocument = new XmlDocument();

        XmlNode rootNode = xmlDocument.CreateElement(FieldMappingNodeName);

        XmlNode node = xmlDocument.CreateElement(SourceNodeName);
        node.InnerXml = this.Source;
        rootNode.AppendChild(node);

        node = xmlDocument.CreateElement(TargetNodeName);
        node.InnerXml = this.Target;
        rootNode.AppendChild(node);

        return rootNode;
      }

      public void Deserialize([NotNull] XmlNode xmlNode)
      {
        Assert.ArgumentNotNull(xmlNode, "xmlNode");
        Assert.AreEqual(xmlNode.Name, FieldMappingNodeName, TheTargetXmlDocumentHasIncorrectFormat);

        XmlNode node = xmlNode.SelectSingleNode(SourceNodeName);
        Assert.IsNotNull(node, string.Format(Text0NodeIsRequired, "Source"));
        Assert.IsNotNullOrEmpty(node.InnerXml, string.Format(Text0ValueIsRequired, "Source"));
        this.Source = node.InnerXml;

        node = xmlNode.SelectSingleNode(TargetNodeName);
        Assert.IsNotNull(node, string.Format(Text0NodeIsRequired, "Target"));
        Assert.IsNotNullOrEmpty(node.InnerXml, string.Format(Text0ValueIsRequired, "Target"));
        this.Target = node.InnerXml;
      }

      #endregion Serialization methods

      [NotNull]
      public FieldMapping Clone()
      {
        return new FieldMapping(this.Source, this.Target);
      }

      #endregion Public methods
    }
  }
}