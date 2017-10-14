// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchContext.cs" company="Sitecore A/S">
//   Copyright (C) 2014 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the SynchContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.Data.Providers
{
  using System;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.ObjectModel;

  /// <summary>
  /// This class is used to pass all necessary information to synchronize Sitecore item tree with Sharepoint List.
  /// </summary>
  public class SynchContext
  {
    private Item parentItem;
    private ID parentID;
    private Database database;
    private SpContext context;

    /// <summary>
    /// Gets or sets SharePointSystemData.
    /// </summary>
    public IntegrationConfigData IntegrationConfigData { get; protected set; }

    /// <summary>
    /// Gets or sets the item for which the synchronization is making.
    /// </summary>
    [NotNull]
    public Item ParentItem
    {
      get
      {
        if (this.parentItem == null)
        {
          using (new IntegrationDisabler())
          {
            this.parentItem = this.Database.GetItem(this.ParentID);
          }
        }

        return this.parentItem;
      }

      protected set
      {
        Assert.ArgumentNotNull(value, "value");

        this.parentItem = value;
      }
    }

    /// <summary>
    /// Gets or sets parent id of item for which the synchronization is making.
    /// </summary>
    [NotNull]
    public ID ParentID
    {
      get
      {
        return this.parentID;
      }

      protected set
      {
        Assert.ArgumentNotNull(value, "value");

        this.parentID = value;
      }
    }

    /// <summary>
    /// Gets or sets current database.
    /// </summary>
    [NotNull]
    public Database Database
    {
      get
      {
        return this.database;
      }

      protected set
      {
        Assert.ArgumentNotNull(value, "value");

        this.database = value;
      }
    }

    [NotNull]
    public SpContext Context
    {
      get
      {
        if (this.context == null)
        {
          this.context = SpContextProviderBase.Instance.CreateDataContext(this.IntegrationConfigData);
        }

        return this.context;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SynchContext"/> class.
    /// </summary>
    /// <param name="parentID">The id of item for which the synchronization is making.</param>
    /// <param name="database">The database.</param>
    public SynchContext([NotNull] ID parentID, [NotNull] Database database)
    {
      Assert.ArgumentNotNull(parentID, "parentID");
      Assert.ArgumentNotNull(database, "database");

      this.ParentID = parentID;
      this.Database = database;

      this.Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SynchContext"/> class.
    /// </summary>
    /// <param name="parent">The item for which the synchronization is making.</param>
    public SynchContext([NotNull] Item parent)
    {
      Assert.ArgumentNotNull(parent, "parent");

      this.ParentItem = parent;
      this.ParentID = parent.ID;
      this.Database = parent.Database;

      this.Initialize();
    }

    /// <exception cref="NullReferenceException">Throws <c>NullReferenceException</c> if Integration Config Data of the current Integration item is not specified.</exception>
    protected void Initialize()
    {
      CacheableIntegrationConfigData integrationConfigData = IntegrationCache.GetIntegrationConfigData(this.ParentID);
      if (integrationConfigData != null)
      {
        this.IntegrationConfigData = integrationConfigData.Data;
      }
      else
      {
        this.IntegrationConfigData = IntegrationConfigDataProvider.GetFromItem(this.ParentItem);
      }

      if (this.IntegrationConfigData == null)
      {
        string message = string.Format("Integration Config Data of Integration item \"{0}\" is not specified.", this.ParentItem.Paths.FullPath);
        throw new NullReferenceException(message);
      }
    }
  }
}
