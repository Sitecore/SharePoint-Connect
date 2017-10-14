// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebCollection.cs" company="Sitecore A/S">
//   Copyright (C) 2011 by Sitecore A/S
// </copyright>
// <summary>
//   Defines WebCollection class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Sharepoint.ObjectModel.Entities.Collections
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Connectors;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;

  public class WebCollection : BaseCollection<Web>
  {
    [Obsolete("It is never used and will be removed in future versions.")]
    protected readonly WebCollectionConnector Connector;
    protected readonly SpContext Context;
    protected readonly Uri WebUrl;

    public WebCollection([NotNull] SpContext context, [NotNull] Uri webUrl)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");

      this.Connector = new WebCollectionConnector(context, webUrl);
      this.Context = context;
      this.WebUrl = webUrl;
    }

    [NotNull]
    protected override List<Web> GetEntities()
    {
      if (this.Entities == null)
      {
        EntityValues webValuesCollection = new WebCollectionConnector(this.Context, this.WebUrl).GetWebs();

        this.Entities = new List<Web>();
        foreach (EntityValues webValues in webValuesCollection["Webs"])
        {
          this.Entities.Add(new Web(webValues, this.Context));
        }
      }

      return this.Entities;
    }
  }
}
