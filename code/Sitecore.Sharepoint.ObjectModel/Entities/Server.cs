// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Server.cs" company="Sitecore A/S">
//   Copyright (C) 2010 by Sitecore A/S
// </copyright>
// <summary>
//   Defines the Server type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Sitecore.Sharepoint.ObjectModel.Entities
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Xml;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Connectors;
  using Sitecore.Sharepoint.ObjectModel.Search;
  using Sitecore.Sharepoint.ObjectModel.Utils;

  /// <summary>
  /// Represents SharePoint server.
  /// </summary>
  public class Server : Web
  {
    /// <summary>
    /// Key for store Server object in SiteCore cache.
    /// </summary>
    private const string CacheKey = "SharepointServers";

    /// <summary>
    /// Indicate that property for Server loaded before.
    /// </summary>
    private bool propertyIsLoaded;

    /// <summary>
    /// Initializes a new instance of the <see cref="Server"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    protected Server([NotNull] SpContext context)
      : base(context)
    {
      Assert.ArgumentNotNull(context, "context");
    }

    public override string Url
    {
      get
      {
        return this.ServerUrl;
      }
    }

    /// <summary>
    /// Gets or sets Properties.
    /// </summary>
    /// <value>The properties.</value>
    [NotNull]
    protected override Common.EntityProperties Properties
    {
      get
      {
        this.LoadProperty();
        return base.Properties;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.LoadProperty();
        base.Properties = value;
      }
    }

    /// <summary>
    /// Return instance of object Server.
    /// </summary>
    /// <param name="context">The SharePoint context.</param>
    /// <returns>Instance of object Server</returns>
    [CanBeNull]
    public static Server Connect([NotNull] SpContext context)
    {
      Assert.ArgumentNotNull(context, "context");

      Dictionary<int, Server> cache = Sitecore.Context.Items[CacheKey] as Dictionary<int, Server>;
      if (cache == null)
      {
        cache = new Dictionary<int, Server>();
        Sitecore.Context.Items[CacheKey] = cache;
      }

      if (cache.ContainsKey(context.Hash))
      {
        return cache[context.Hash];
      }

      Server server = new Server(context);
      cache[context.Hash] = server;
      return server;
    }

    /// <summary>
    /// Method for loading property for server.
    /// </summary>
    private void LoadProperty()
    {
      if (this.propertyIsLoaded == false)
      {
        this.FillProperty(new WebConnector(this.Context, new Uri(this.Url)).GetWeb().Properties);
        this.propertyIsLoaded = true;
      }
    }
  }
}