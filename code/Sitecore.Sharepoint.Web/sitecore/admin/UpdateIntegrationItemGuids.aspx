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
<%@ Import Namespace="Sitecore.Sharepoint.Installer" %>

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
            row["state"] = new UpdateGuidsPostStepAction().UpdateItemsGuid(item) ? "Success" : "Fail";
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

