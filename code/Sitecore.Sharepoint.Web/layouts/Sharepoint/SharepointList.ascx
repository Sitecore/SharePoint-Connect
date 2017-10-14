<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharepointList.ascx.cs"
   Inherits="Sitecore.Sharepoint.Web.SharepointList" %>
<%@ Import Namespace="Sitecore.Resources"%>
<%@ Import Namespace="Sitecore.Sharepoint.Common.Texts" %>
<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
   Namespace="System.Web.UI" TagPrefix="asp" %>
<script src="/sitecore/shell/Controls/Lib/jQuery/jquery.js" type="text/javascript"></script>
<script src="/script/jquery.hoverIntent.minified.js" type="text/javascript"></script>
<script src="/script/sharepoint.js" type="text/javascript"></script>
<link href="/css/sharepoint.css" rel="stylesheet" type="text/css" />

<div class="control-wrapper sharepoint-list">
    <asp:UpdatePanel ID="spGridViewUpdatePanel" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="folderPath" runat="server" />
            <asp:Label ID="errorLbl" runat="server" Visible="false" />

            <asp:Label ID="LabelSharepointSite" runat="server" CssClass="genericGrid_breadcrumb" Text=""></asp:Label>

            <h3 class="controlHeader">
                <span><%= List != null ? List.Name : Sitecore.Globalization.Translate.Text(UIMessages.UnknownList) %></span>
            </h3>

            <div ID="labelErrorMessageDiv" runat="server" Visible="False">
              <h3>
                <asp:Label ID="labelErrorMessage" runat="server"/>
              </h3>
            </div>

            <div id="menubarDiv" runat="server" class="genericGrid_Menubar">
                <span class="btn btnOpen">
                    <a onclick='<%="javascript:window.open(\"" + this.SharepointViewUrl + "\"); return false;"%>' href="#"><%= Sitecore.Globalization.Translate.Text(UIMessages.Open) %></a>
                </span>
                <span class="btn btnNew">
                    <a onclick='<%="javascript:window.open(\"" + this.GetNewFormUrl() + "\"); return false;"%>' href="#"><%= Sitecore.Globalization.Translate.Text(UIMessages.New) %></a>
                </span>
                <div class="genericGrid_Views">
                    <span><%= Sitecore.Globalization.Translate.Text(UIMessages.Views_Colon) %></span>
                    <asp:DropDownList ID="spViewsList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="spViewsList_IndexChanged" class="viewsList" />
                </div>
            </div>

            <div id="dataDiv" runat="server" class="genericGrid_datacontainer">
                <asp:GridView ID="spGridView" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                    UseAccessibleHeader="true" OnSorting="spGridView_Sorting" PageSize="2" Style="width: 100%;"
                    GridLines="None" class="genericGrid_data" CellSpacing="-1"
                    OnRowDataBound="spGridView_RowDataBound" EnableSortingAndPagingCallbacks="true">
                    <RowStyle CssClass="genericGrid_data_row" />
                    <AlternatingRowStyle CssClass="genericGrid_data_alt_row" />
                    <SelectedRowStyle CssClass="genericGrid_data_selected_row" />
                    <HeaderStyle HorizontalAlign="Left" ForeColor="Gray" />
                </asp:GridView>
            </div>

            <div id="controlButtonsDiv" runat="server" class="genericGrid_controlButtons">
               <div class="height-spike"></div>
               <div class="navigationButtons">
                   <asp:LinkButton ID="spPrevBtn" runat="server" OnClick="spPrevBtn_Click"><%= Sitecore.Globalization.Translate.Text(UIMessages.Previous) %></asp:LinkButton>
                   <asp:LinkButton ID="spNextBtn" runat="server" OnClick="spNextBtn_Click"><%= Sitecore.Globalization.Translate.Text(UIMessages.Next) %></asp:LinkButton>
               </div>
               <asp:label runat="server" Class="paging" ID="lblItems" />
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:UpdateProgress AssociatedUpdatePanelID="spGridViewUpdatePanel" ID="spUpdateProgress" runat="server">
        <ProgressTemplate>
            <div class="progressBackgroundFilter"></div>
                <div class="processMessage">
                <img alt="<%= Sitecore.Globalization.Translate.Text(UIMessages.Loading) %>" src="/sitecore/shell/Themes/Standard/Images/loading15x15.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

   <div class="disableScreen"></div>
</div>