<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharepointTasks.ascx.cs" Inherits="Sitecore.Sharepoint.Web.SharepointTasks" %>
<%@ Import Namespace="Sitecore.Sharepoint.ObjectModel.Entities.Items" %>
<%@ Import Namespace="Sitecore.Sharepoint.Common.Texts" %>
<link href="/css/sharepoint.css" rel="stylesheet" type="text/css" />

<div class="control-wrapper task-list">
    <asp:HiddenField ID="folderPath" runat="server" />
    <asp:Label ID="LabelSharepointSite" runat="server" CssClass="genericGrid_breadcrumb" Text=""></asp:Label>

    <h3 class="controlHeader">
        <span><%= Sitecore.Globalization.Translate.Text(UIMessages.Tasks) %></span>
    </h3>
  
    <h3>
      <asp:Label ID="labelErrorMessage" runat="server" Visible="False" />
    </h3> 

    <div class="tasks">
         <asp:GridView ID="documentsList" runat="server" AutoGenerateColumns="false" Style="width: 100%;
            padding: 10px;" GridLines="None" ShowHeader="false">
            <RowStyle CssClass="genericGrid_data_row" />
            <AlternatingRowStyle CssClass="genericGrid_data_alt_row" />
            <Columns>
               <asp:TemplateField>
                  <ItemTemplate>
                     <a style="text-decoration: none;" href="<%# (Container.DataItem as TaskItem).PropertiesUrl %>" target="_blank">
                        <span class="downloadheadline">
                           <%# (Container.DataItem as TaskItem).Title%>
                        </span></a>
                  </ItemTemplate>
               </asp:TemplateField>
               <asp:TemplateField>
                  <ItemTemplate>
                     <%# (Container.DataItem as TaskItem).AssignedTo %>
                  </ItemTemplate>
               </asp:TemplateField>
               <asp:TemplateField>
                  <ItemTemplate>
                     <%#
                        (Container.DataItem as TaskItem).DueDate !=  DateTime.MinValue?Sitecore.Sharepoint.ObjectModel.Utils.DateUtil.FormatShortDate((Container.DataItem as TaskItem).DueDate):""
                              %>
                  </ItemTemplate>
               </asp:TemplateField>
               <asp:TemplateField>
                  <ItemTemplate>
                     <%# string.Format("{0:p}", (Container.DataItem as TaskItem).Complete) %>
                  </ItemTemplate>
               </asp:TemplateField>
            </Columns>
         </asp:GridView>
    </div>
    <div class="disableScreen"></div>
</div>