namespace Sitecore.Sharepoint.ObjectModel.Connectors
{
  using System;
  using System.Net;
  using System.Xml;
  using Sitecore.Diagnostics;
  using Sitecore.Sharepoint.Data.WebServices;
  using Sitecore.Sharepoint.Data.WebServices.SharepointViews;
  using Sitecore.Sharepoint.ObjectModel.Entities.Common;
  using Sitecore.Sharepoint.ObjectModel.Utils;

  public class ViewCollectionConnector
  {
    protected readonly Views ViewsWebService;

    public ViewCollectionConnector([NotNull] SpContext context, [NotNull] Uri webUrl)
    {
      Assert.ArgumentNotNull(context, "context");
      Assert.ArgumentNotNull(webUrl, "webUrl");

      this.ViewsWebService = new Views();
      this.ViewsWebService.SetServer(webUrl, context);
    }

    /// <summary>
    /// Gets list of views for the specified SharePoint list.
    /// </summary>
    /// <param name="listName">Name of the list.</param>
    /// <returns>List of view</returns>
    [NotNull]
    public EntityValues GetViews([NotNull] string listName)
    {
      Assert.ArgumentNotNull(listName, "listName");

      XmlNode viewsNode = this.ViewsWebService.GetViewCollection(listName);

      var views = new EntityValues();
      foreach (XmlNode viewNode in viewsNode.ChildNodes)
      {
        var view = new EntityValues
        {
          Properties = XmlUtils.LoadProperties(viewNode)
        };

        views.Add("Views", view);
      }

      return views;
    }
  }
}
