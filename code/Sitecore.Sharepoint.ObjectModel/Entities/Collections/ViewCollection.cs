namespace Sitecore.Sharepoint.ObjectModel.Entities.Collections
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.ObjectModel.Connectors;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;

  public class ViewCollection : BaseCollection<View>
  {
    [Obsolete("It is never used and will be removed in future versions.")]
    protected readonly ViewCollectionConnector Connector;
    protected readonly SpContext Context;
    protected readonly BaseList List;

    public ViewCollection([NotNull] SpContext context, [NotNull] BaseList list)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(list, "list");

      this.Connector = new ViewCollectionConnector(context, list.WebUrl);
      this.Context = context;
      this.List = list;
    }

    [NotNull]
    protected override List<View> GetEntities()
    {
      if (this.Entities == null)
      {
        EntityValues viewValuesCollection = new ViewCollectionConnector(this.Context, this.List.WebUrl).GetViews(this.List.ID);

        this.Entities = new List<View>();
        foreach (EntityValues viewValues in viewValuesCollection["Views"])
        {
          this.Entities.Add(new View(viewValues, this.List, this.Context));
        }
      }

      return this.Entities;
    }
  }
}
