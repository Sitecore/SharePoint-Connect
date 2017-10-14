namespace Sitecore.Sharepoint.ObjectModel.Entities.Collections
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Connectors;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Factories;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;

  public class ListCollection : BaseCollection<BaseList>
  {
    [Obsolete("It is never used and will be removed in future versions.")]
    protected readonly ListCollectionConnector Connector;
    protected readonly SpContext Context;
    protected readonly Uri WebUrl;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListCollection"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="webUrl">The web URL.</param>
    public ListCollection([NotNull] SpContext context, [NotNull] Uri webUrl)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");

      this.Connector = new ListCollectionConnector(context, webUrl);
      this.Context = context;
      this.WebUrl = webUrl;
    }

    protected override List<BaseList> GetEntities()
    {
      if (this.Entities == null)
      {
        EntityValues listValuesCollection = new ListCollectionConnector(this.Context, this.WebUrl).GetLists();

        this.Entities = new List<BaseList>();
        foreach (EntityValues listValues in listValuesCollection["Lists"])
        {
          this.Entities.Add(ListFactory.CreateListObject(listValues, this.WebUrl, this.Context));
        }
      }

      return this.Entities;
    }
  }
}
