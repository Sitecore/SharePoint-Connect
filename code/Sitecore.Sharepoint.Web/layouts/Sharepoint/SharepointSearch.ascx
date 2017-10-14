<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharepointSearch.ascx.cs"
   Inherits="Sitecore.Sharepoint.Web.SharepointSearch" %>
<%@ Import Namespace="Sitecore.Sharepoint.Common.Texts" %>
<link href="/css/sharepoint.css" rel="stylesheet" type="text/css" />
<script src="/sitecore/shell/Controls/Lib/jQuery/jquery.js" type="text/javascript"></script>
<script src="/script/jquery.hoverIntent.minified.js" type="text/javascript"></script>
<script src="/script/sharepoint.js" type="text/javascript"></script>

<style type="text/css">
   .list
   {
      font: 10px Arial;
      width: 110px;
   }
</style>
<script language="javascript" >
   function checkKey(keyCode, buttonId) {
      if (!keyCode) {
         keyCode = event.keyCode;
      }
      if (keyCode == 13) {
         var button = document.getElementById(buttonId);
         if (button) {
            button.click();
         }
         return false;
      }
   }
</script>

<div class="control-wrapper sharepoint-search">
<asp:UpdatePanel ID="spSearchUpdatePanel" runat="server">
    <ContentTemplate>
        <table class="tableSerach">
            <tr>
                <td colspan="2">
                    <asp:LinkButton ID="advancedSearchBtn" runat="server" OnClick="advancedSearchBtn_Click" class="advancedSearchLink" ><%= "»&nbsp;"+Sitecore.Globalization.Translate.Text(UIMessages.AdvancedSearch) %></asp:LinkButton>
                </td>
            </tr>
            <tr class="simpleSearch">
                <td width="128">
                    <asp:TextBox ID="searchBox" runat="server" class="TopSearchField" onkeypress='<%# "javascript:return checkKey(event.which, \"" + searchBtn.ClientID + "\");" %>'></asp:TextBox>
                </td>
                <td>
                    <button runat="server" ID="searchBtn" OnServerClick="searchBtn_Click" class="spSearchBtn"><span><%= Sitecore.Globalization.Translate.Text(UIMessages.Search) %></span></button>
                </td>
            </tr>
            <tr id="advancedSearchSitesPane" class="advancedSearch siteSearch" runat="server" visible="false">
               <td width="128">
                  <asp:Label runat="server"><%= Sitecore.Globalization.Translate.Text(UIMessages.SiteToSearch) %></asp:Label>
               </td>
               <td>
                  <asp:DropDownList ID="sitesList" runat="server" class="list" OnSelectedIndexChanged="sitesList_IndexChanged" AutoPostBack="true"  />
               </td>
            </tr>
            <tr id="advancedSearchListsPane" class="advancedSearch listSearch" runat="server" visible="false">
               <td width="128">
                  <asp:Label runat="server" ><%= Sitecore.Globalization.Translate.Text(UIMessages.ListsToSearch) %></asp:Label>
               </td>
               <td>
                  <asp:DropDownList ID="listsList" runat="server" class="list" />
               </td>
            </tr>
        </table>

      <asp:Label ID="errorLbl" runat="server" Visible="false" />
      <div id="resultsDiv" runat="server">
        <table class="search-result">
            <tr class="header">
                <th class="type">Type</th>
                <th><%= Sitecore.Globalization.Translate.Text(UIMessages.Name) %></th>
                <th><%= Sitecore.Globalization.Translate.Text(UIMessages.File_Size) %></th>
                <th><%= Sitecore.Globalization.Translate.Text(UIMessages.Created_By) %></th>
            </tr>
            <asp:Repeater ID="searchResults" runat="server" OnDataBinding="searchResults_dataBinding">
            <ItemTemplate>
            <tr class="genericGrid_data_row">
                <td><a href="<%# GetUrl(Container.DataItem) %>" target="_blank"><img src="<%# GetIconUrl(Container.DataItem) %>" width="16" height="16" alt="<%# GetTitle(Container.DataItem) %>" /></a></td>
                <td class="docName"><a href="<%# GetUrl(Container.DataItem) %>" target="_blank"><%# GetTitle(Container.DataItem) %></a></td>
                <td><%# GetSize(Container.DataItem, false) %></td>
                <td><%# GetAuthor(Container.DataItem, false) %></td>
            </tr>
            </ItemTemplate>

            <AlternatingItemTemplate>
            <tr class="genericGrid_data_alt_row">
                <td><a href="<%# GetUrl(Container.DataItem) %>" target="_blank"><img src="<%# GetIconUrl(Container.DataItem) %>" width="16" height="16" alt="<%# GetTitle(Container.DataItem) %>" /></a></td>
                <td class="docName"><a href="<%# GetUrl(Container.DataItem) %>" target="_blank"><%# GetTitle(Container.DataItem) %></a></td>
                <td><%# GetSize(Container.DataItem, false) %></td>
                <td><%# GetAuthor(Container.DataItem, false) %></td>
            </tr>
            </AlternatingItemTemplate>
            </asp:Repeater>
        </table>

        <div class="pagingbar">
            <asp:Repeater ID="pagingRepeater" runat="server">
                <ItemTemplate>
                    <asp:LinkButton ID="pageLinkBtn" runat="server" CommandArgument="<%# ((SearchPageItem)Container.DataItem).Index %>"
                        Enabled="<%# !((SearchPageItem)Container.DataItem).Current%>" OnClick="pageLinkButton_click">
                        <%# Eval("Title") %>
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:Repeater>
        </div>

      </div>
   </ContentTemplate>
</asp:UpdatePanel>
   <asp:UpdateProgress AssociatedUpdatePanelID="spSearchUpdatePanel" ID="spUpdateProgress" runat="server">
      <ProgressTemplate>
         <div class="progressBackgroundFilter"></div>
         <div class="processMessage">
            <img alt="<%= Sitecore.Globalization.Translate.Text(UIMessages.Loading) %>" src="/sitecore/shell/Themes/Standard/Images/loading15x15.gif" />
         </div>
      </ProgressTemplate>
   </asp:UpdateProgress>
<div class="disableScreen"></div>
</div>
