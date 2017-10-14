<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharepointAnnouncements.ascx.cs"
   Inherits="Sitecore.Sharepoint.Web.SharepointAnnouncements" %>
<%@ Import Namespace="Sitecore" %>
<%@ Import Namespace="Sitecore.Sharepoint.ObjectModel.Entities.Items" %>
<%@ Import Namespace="Sitecore.Sharepoint.Common.Texts" %>
<link href="/css/sharepoint.css" rel="stylesheet" type="text/css" />

<div class="control-wrapper">
    <asp:HiddenField ID="folderPath" runat="server" />
    <asp:Label ID="LabelSharepointSite" runat="server" CssClass="genericGrid_breadcrumb" Text=""></asp:Label>

    <h3 class="controlHeader">
        <div><%= Sitecore.Globalization.Translate.Text(UIMessages.Announcements) %></div>
    </h3>
  
    <h3>
      <asp:Label ID="labelErrorMessage" runat="server" Visible="False" />
    </h3>  
    
    <div class="width-limiter"></div>

    <table class="announcements">
        <tr>
            <td>
                <asp:GridView ID="documentsList" runat="server" AutoGenerateColumns="false" GridLines="None" ShowHeader="false">
                    <RowStyle CssClass="genericGrid_data_row" />
                    <AlternatingRowStyle CssClass="genericGrid_data_alt_row" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                    <a class="lnk-title" href="<%# (Container.DataItem as AnnouncementItem).PropertiesUrl %>" target="_blank">
                                        <span><%# (Container.DataItem as AnnouncementItem).Title %></span>
                                    </a>
                                    <div class="item-wrapper">
                                        <%# StringUtil.Clip(StringUtil.RemoveTags((Container.DataItem as AnnouncementItem).Body), 200, true) %>
                                        <span class="date">
                                            <%# (Container.DataItem as AnnouncementItem).Modified != DateTime.MinValue ? Sitecore.Sharepoint.ObjectModel.Utils.DateUtil.FormatShortDate((Container.DataItem as AnnouncementItem).Modified):""%>
                                        </span>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <div class="disableScreen"></div>
</div>