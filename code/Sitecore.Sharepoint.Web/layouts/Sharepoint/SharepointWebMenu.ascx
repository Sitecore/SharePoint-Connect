<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharepointWebMenu.ascx.cs" Inherits="Sitecore.Intranet.Sharepoint.Web.Layouts.SharepointWebMenu" %>
<%@ Import Namespace="Sitecore.Intranet.Sharepoint.Web.Layouts" %>
<link href='<%="/css/sharepoint.css?"+DateTime.Now.ToString("ddmmss")%>' rel="stylesheet" type="text/css" />

<style type="text/css">

* html div.subWebContainer li  {
    border: none!important;
}

</style>

   <div id="submenucontainer">
      <asp:Repeater ID="webList" runat="server" >
         <HeaderTemplate>
              <ul id="submenumain">
         </HeaderTemplate>
         <ItemTemplate>
            <li class="<%#((MenuRenderingInfo)Container.DataItem).Selected?"submenulinkon":"submenulinkoff" %>" >
              <a class="<%#((MenuRenderingInfo)Container.DataItem).Selected?"submenulinkon":"submenulinkoff" %>"
              href='<%#((MenuRenderingInfo)Container.DataItem).Link %>'>
               <%# ((MenuRenderingInfo)Container.DataItem).Title%>
              </a>
            </li>
         </ItemTemplate>
         <FooterTemplate>
             </ul>
         </FooterTemplate>
      </asp:Repeater>
 </div>
