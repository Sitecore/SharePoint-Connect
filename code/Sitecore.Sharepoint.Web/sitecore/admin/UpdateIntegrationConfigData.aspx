<%-- This page updates integration configuration data fild List --%>
<%@ Page Language="C#" AutoEventWireup="true"  %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="Sitecore.Sharepoint.Data.Providers.IntegrationConfig" %>
<%@ Import Namespace="Sitecore.Diagnostics" %>
<%@ Import Namespace="Sitecore.Sharepoint.Data.Providers" %>
<%@ Import Namespace="Sitecore.Sharepoint.Data.Providers.Cache" %>
<%@ Import Namespace="Sitecore.Sharepoint.ObjectModel" %>
<%@ Import Namespace="Sitecore.Sharepoint.ObjectModel.Entities.Lists" %>
<%@ Import Namespace="Sitecore" %>

<script runat=server>
  DataTable table;
  string db = "master";
  protected void Page_Load(object sender, EventArgs e)
  {
    table = new DataTable();
    table.Columns.Add("name");
    table.Columns.Add("state");

    using (new IntegrationDisabler())
    {
      string query = string.Format("fast://*[@@templateid = '{0}']", Sitecore.Sharepoint.Common.TemplateIDs.IntegrationConfig);
      foreach (Item item in Sitecore.Data.Database.GetDatabase(db).SelectItems(query))
      {
        IntegrationConfigData integrationData = IntegrationConfigDataProvider.GetFromItem(item);
        if (integrationData == null)
        {
          continue;
        }
        DataRow row = this.table.NewRow();
        row["name"] = integrationData.Server + integrationData.Web + integrationData.List;
        this.table.Rows.Add(row);
      }
    }

    this.Bind();
  }

  private void Bind()
  {
      this.Gridview1.DataSource = this.table;
      this.Gridview1.DataBind();
  }
  protected bool UpdateIntegrationConfigData(Item item)
  {
    IntegrationConfigData integrationData = null;
    try
    {
        integrationData = IntegrationConfigDataProvider.GetFromItem(item);
        if (integrationData == null)
        {
            return false;
        }

        SpContext context = SpContextProviderBase.Instance.CreateDataContext(integrationData);

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
  
  protected void Button1_Click(object sender, EventArgs e)
  {
    table.Clear();

    using (new IntegrationDisabler())
    {
        string query = string.Format("fast://*[@@templateid = '{0}']", Sitecore.Sharepoint.Common.TemplateIDs.IntegrationConfig);
        foreach (Item item in Sitecore.Data.Database.GetDatabase(db).SelectItems(query))
        {
            IntegrationConfigData integrationData = IntegrationConfigDataProvider.GetFromItem(item);
            if (integrationData == null)
            {
                continue;
            }
            DataRow row = this.table.NewRow();
            this.table.Rows.Add(row);
            row["name"] = integrationData.Server + integrationData.Web + integrationData.List;
            row["state"] = this.UpdateIntegrationConfigData(item) ? "Success" : "Fail";
        }

        query = string.Format("fast://*[@@templateid = '{0}']", Sitecore.Sharepoint.Common.TemplateIDs.IntegrationFolder);
        foreach (Item item in Sitecore.Data.Database.GetDatabase(db).SelectItems(query))
        {
            this.UpdateIntegrationConfigData(item);
        }
    }
    this.Bind();
  }
</script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h2>Integration items data update</h2>
        <asp:gridview ID="Gridview1" runat="server" AutoGenerateColumns="False" EnableModelValidation="True" CellPadding="4" ForeColor="#333333" GridLines="None">
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            <Columns>
                <asp:BoundField DataField="name" HeaderText="List" />
                <asp:BoundField DataField="state" HeaderText="List State" />
            </Columns>
            <EditRowStyle BackColor="#999999" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        </asp:gridview>
        <p></p>
        <asp:Button ID="Button1" runat="server" Text="Update" OnClick="Button1_Click" />
    </div>
    </form>
</body>
</html>

