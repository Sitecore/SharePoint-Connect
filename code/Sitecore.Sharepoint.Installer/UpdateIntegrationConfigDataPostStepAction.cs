namespace Sitecore.Sharepoint.Installer
{
  using System;
  using System.Collections.Specialized;
  using System.Text;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Install.Framework;
  using Sitecore.Sharepoint.Common;
  using Sitecore.Sharepoint.Data.Providers;
  using Sitecore.Sharepoint.Data.Providers.Cache;
  using Sitecore.Sharepoint.Data.Providers.IntegrationConfig;
  using Sitecore.Sharepoint.ObjectModel;
  using Sitecore.Sharepoint.ObjectModel.Entities.Lists;

  public class UpdateIntegrationConfigDataPostStepAction : IPostStep
  {
    public void Run(ITaskOutput output, NameValueCollection metaData)
    {
      bool isSuccessful = true;
      using (new IntegrationDisabler())
      {
        string query = string.Format("fast://*[@@templateid = '{0}']", TemplateIDs.IntegrationConfig);
        foreach (Item item in Context.ContentDatabase.SelectItems(query))
        {
          isSuccessful = this.UpdateIntegrationConfigData(item) && isSuccessful;
        }

        query = string.Format("fast://*[@@templateid = '{0}']", TemplateIDs.IntegrationFolder);
        foreach (Item item in Context.ContentDatabase.SelectItems(query))
        {
          isSuccessful = this.UpdateIntegrationConfigData(item) && isSuccessful;
        }
      }

      output.Alert(isSuccessful ? "All integration config items has been updated successfully." : "Some integration config items haven't been updated. See log for details.");
    }

    protected bool UpdateIntegrationConfigData(Item item)
    {
      IntegrationConfigData integrationData = null;
      try
      {
        integrationData = IntegrationConfigDataProvider.GetFromItem(item);
        if (integrationData == null)
        {
          return true;
        }

        var context = SpContextProviderBase.Instance.CreateDataContext(integrationData);

        string listId;
        listId = BaseList.GetList(integrationData.Web, integrationData.List, context).ID;

        Assert.IsNotNullOrEmpty(listId, "listId");

        if (integrationData.List == listId)
        {
          return true;
        }
        integrationData.List = listId;

        IntegrationConfigDataProvider.SaveToItem(integrationData, item);
        IntegrationCache.RemoveIntegrationConfigData(item.ID);

        return true;
      }
      catch (Exception exception)
      {
        var errorMessage = new StringBuilder("Updating integration config data has been failed.");
        errorMessage.AppendLine(string.Format("Integration config item: {0}", item.ID));
        if (integrationData != null)
        {
          errorMessage.AppendLine(string.Format("SharePoint list: {0}{1}{2}", integrationData.Server.Trim('/'), StringUtil.EnsurePrefix('/', StringUtil.EnsurePostfix('/', integrationData.Web)), integrationData.List.Trim('/')));
        }

        Log.Error(errorMessage.ToString(), exception, this);

        return false;
      }
    }
  }
}